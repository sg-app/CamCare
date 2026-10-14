using System.Security.Claims;
using CamCare.Interfaces.Services;
using CamCare.Models;
using CamCare.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CamCare.Extensions;

public static class MapExtensions
{

    public static IEndpointRouteBuilder MapAuth(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/auth/login", async (HttpContext context, [FromBody] LoginRequest request, IUserGroupService userGroupService) =>
        {
            var userGroup = await userGroupService.ValidateAsync(request.Group, request.Password);
            if (userGroup is null)
                return Results.Unauthorized();

            var identity = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, userGroup.Name),
                    new Claim(ClaimTypes.Role, userGroup.Name)
                ],
                CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true
                });

            return Results.Json(new AuthResult { Redirect = "/" });
        });

        builder.MapPost("/api/auth/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Json(new AuthResult { Redirect = "/Login" });
        });
        return builder;
    }
    public static IEndpointRouteBuilder MapDatastores(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/datastores/{id:int}/file", async (int id, bool? download, IDataStoreService dataStoreService) =>
        {
            var response = await dataStoreService.GetFileAsync(id);
            if (!response.Success || response.Data is null)
                return Results.NotFound();

            return Results.File(
                response.Data.Content,
                response.Data.ContentType,
                (download ?? false) ? response.Data.Filename : null,
                enableRangeProcessing: true);
        }).RequireAuthorization();
        return builder;
    }

    public static IEndpointRouteBuilder MapUpload(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/u/{token}", async (string token, IUploadTokenService uploadTokenService,
            IRepairOrderService repairOrderService, IOptions<ObjectStorageOptions> objectStorageOptions) =>
        {
            if (string.IsNullOrWhiteSpace(token) || !uploadTokenService.TryValidate(token, out var repairOrderId))
                return UploadPageRenderer.NotFoundPage();

            var orderResponse = await repairOrderService.GetByIdAsync(repairOrderId);
            if (!orderResponse.Success || orderResponse.Data is null)
                return UploadPageRenderer.NotFoundPage();

            return UploadPageRenderer.RenderUploadPage(repairOrderId, token, objectStorageOptions.Value.MaxFileSizeBytes);
        });

        builder.MapPost("/api/upload/{token}", async (string token, HttpRequest request,
            IUploadTokenService uploadTokenService, IRepairOrderService repairOrderService,
            IDataStoreService dataStoreService, IRepairOrderChangeBroadcaster changeBroadcaster,
            IOptions<ObjectStorageOptions> objectStorageOptions, ILogger<Program> logger) =>
        {
            // Abgelaufene/ungültige Tokens und unbekannte Aufträge mit 404 beantworten.
            if (string.IsNullOrWhiteSpace(token) || !uploadTokenService.TryValidate(token, out var repairOrderId))
                return Results.NotFound();

            var orderResponse = await repairOrderService.GetByIdAsync(repairOrderId);
            if (!orderResponse.Success || orderResponse.Data is null)
                return Results.NotFound();

            var maxFileSizeBytes = objectStorageOptions.Value.MaxFileSizeBytes;

            // Kestrel-Body-Limit anheben (Standard ~30 MB), damit Dateien bis zur
            // konfigurierten MaxFileSizeBytes übertragen werden können.
            var sizeFeature = request.HttpContext.Features.Get<IHttpMaxRequestBodySizeFeature>();
            if (sizeFeature is { IsReadOnly: false })
                sizeFeature.MaxRequestBodySize = maxFileSizeBytes + (1024 * 1024);

            IFormCollection form;
            try
            {
                form = await request.ReadFormAsync();
            }
            catch (InvalidDataException ex)
            {
                logger.LogWarning(ex, "Upload abgelehnt: Request-Body überschreitet das Multipart-Limit.");
                return Results.BadRequest(new UploadResult { Errors = ["Die Datei(en) sind zu groß."] });
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Ungültiger Multipart-Request im Upload-Endpunkt.");
                return Results.BadRequest(new UploadResult { Errors = ["Ungültiger Upload-Request."] });
            }

            var result = new UploadResult();

            foreach (var file in form.Files)
            {
                if (!UploadEndpointHelpers.IsSupportedContentType(file.ContentType))
                {
                    result.Errors.Add($"\"{file.FileName}\" - kein unterstützter Dateityp (nur Bild-, PDF- und Videodateien).");
                    continue;
                }

                if (file.Length <= 0 || file.Length > maxFileSizeBytes)
                {
                    result.Errors.Add($"\"{file.FileName}\" - Datei ist zu groß (max. {maxFileSizeBytes / (1024 * 1024)} MB).");
                    continue;
                }

                try
                {
                    await using var stream = file.OpenReadStream();
                    using var mStream = new MemoryStream();
                    await stream.CopyToAsync(mStream);
                    var data = mStream.ToArray();

                    var item = new DataStoreVm
                    {
                        RepairOrderId = repairOrderId,
                        Type = file.ContentType,
                        Filename = file.FileName,
                        Data = data,
                        SizeBytes = data.LongLength
                    };

                    var response = await dataStoreService.CreateAsync(item);
                    if (response.Success && response.Data is not null)
                    {
                        result.Files.Add(file.FileName);
                        logger.LogDebug("Mobiler Upload (HTTP): {FileName} / {Size} bytes für Auftrag {RepairOrderId}",
                            file.FileName, file.Length, repairOrderId);
                    }
                    else
                    {
                        result.Errors.Add($"\"{file.FileName}\" - {response.Message ?? "Speichern fehlgeschlagen."}");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Fehler beim mobilen Upload (HTTP) von {FileName} für Auftrag {RepairOrderId}",
                        file.FileName, repairOrderId);
                    result.Errors.Add($"\"{file.FileName}\" - Upload fehlgeschlagen.");
                }
            }

            // Geöffnete Editor-Circuits über den In-Process-Change-Bus aktualisieren.
            if (result.Files.Count > 0)
            {
                await changeBroadcaster.PublishAsync(new RepairOrderChangedEvent(repairOrderId, RepairOrderChangeType.FileUploaded));
            }

            result.Success = result.Errors.Count == 0;
            return Results.Json(result);
        }).DisableAntiforgery();
        return builder;
    }

    /// <summary>
    /// Gemeinsame Hilfsfunktionen der Upload-Endpunkte.
    /// </summary>
    internal static class UploadEndpointHelpers
    {
        /// <summary>
        /// Erlaubte Content-Types: identisch zur bestehenden Logik der
        /// Blazor-Uploads (nur Bild-, PDF- und Videodateien).
        /// </summary>
        public static bool IsSupportedContentType(string? contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
                return false;

            return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                   || contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase)
                   || contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Liefert die nicht-interaktive Mobile-Upload-Seite (reines HTML + Vanilla-JS,
    /// kein Blazor-Circuit/WebSocket) sowie eine schlanke 404-Seite für ungültige/
    /// abgelaufene Upload-Links.
    /// </summary>
    internal static class UploadPageRenderer
    {
        public static IResult RenderUploadPage(int repairOrderId, string token, long maxFileSizeBytes)
        {
            var html = UploadPageTemplate
                .Replace("__ORDER_ID__", repairOrderId.ToString())
                .Replace("__TOKEN__", token)
                .Replace("__MAX_SIZE_BYTES__", maxFileSizeBytes.ToString())
                .Replace("__MAX_SIZE_MB__", (maxFileSizeBytes / (1024 * 1024)).ToString());

            return Results.Content(html, "text/html; charset=utf-8");
        }

        public static IResult NotFoundPage()
        {
            return Results.Content(NotFoundPageTemplate, "text/html; charset=utf-8", statusCode: StatusCodes.Status404NotFound);
        }

        private const string NotFoundPageTemplate = """
        <!DOCTYPE html>
        <html lang="de">
        <head>
            <meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <title>Link ungültig - CamCare</title>
            <style>
                body { margin: 0; padding: 24px 12px; font-family: system-ui, -apple-system, "Segoe UI", Roboto, sans-serif; background: #f5f6fa; color: #212529; display: flex; justify-content: center; }
                .card { max-width: 520px; width: 100%; background: #fff; border-radius: 12px; box-shadow: 0 2px 12px rgba(0,0,0,.08); padding: 28px 24px; }
                h1 { font-size: 1.25rem; margin: 0 0 8px; }
                p { color: #495057; line-height: 1.5; margin: 0; }
            </style>
        </head>
        <body>
            <div class="card">
                <h1>Ungültiger oder abgelaufener Link</h1>
                <p>Der Upload-Link ist ungültig oder abgelaufen.
                Bitte fordere über das CamCare-Büro einen neuen Link an.</p>
            </div>
        </body>
        </html>
        """;

        private static readonly string UploadPageTemplate = """
        <!DOCTYPE html>
        <html lang="de">
        <head>
            <meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <title>Datei-Upload - CamCare</title>
            <style>
                * { box-sizing: border-box; }
                body { margin: 0; padding: 24px 12px; font-family: system-ui, -apple-system, "Segoe UI", Roboto, sans-serif; background: #f5f6fa; color: #212529; display: flex; justify-content: center; }
                .card { max-width: 560px; width: 100%; background: #fff; border-radius: 12px; box-shadow: 0 2px 12px rgba(0,0,0,.08); padding: 28px 24px; }
                .brand { display: flex; align-items: center; gap: 10px; margin-bottom: 6px; }
                h1 { font-size: 1.25rem; margin: 0; }
                .lead { color: #6c757d; font-size: .95rem; margin: 10px 0 18px; line-height: 1.55; }
                .drop-zone { position: relative; border: 2px dashed #adb5bd; border-radius: 8px; padding: 30px 12px; text-align: center; color: #495057; cursor: pointer; transition: border-color .15s ease, background-color .15s ease; }
                .drop-zone:hover, .drop-zone.drag { border-color: #0d6efd; background-color: #eef4ff; }
                /* Das File-Input überdeckt die komplette Fläche (transparent), damit ein
                   Tippen auf der mobilen Seite den nativen Dateiauswahl-Dialog öffnet –
                   ein verstecktes (display:none) Input wird von <label> bzw. Klick benötigt. */
                .drop-zone input[type=file] { position: absolute; inset: 0; width: 100%; height: 100%; opacity: 0; cursor: pointer; }
                .drop-zone .icon { font-size: 1.9rem; line-height: 1; }
                .drop-zone .label { margin-top: 10px; font-size: .95rem; }
                .status { margin-top: 14px; }
                .progress { display: none; height: 6px; background: #e9ecef; border-radius: 99px; overflow: hidden; margin: 12px 0; }
                .progress .bar { height: 100%; width: 0; background: #0d6efd; transition: width .2s ease; }
                .uploading .progress { display: block; }
                .message { border-radius: 8px; padding: 12px 14px; margin: 8px 0; font-size: .92rem; line-height: 1.45; }
                .success { background: #d1e7dd; color: #0f5132; border: 1px solid #badbcc; }
                .error { background: #f8d7da; color: #842029; border: 1px solid #f5c2c7; }
                .message ul { margin: 6px 0 0; padding-left: 18px; }
                .message li { margin: 2px 0; }
            </style>
        </head>
        <body>
            <div class="card" id="card">
                <div class="brand">
                    <h1>Datei-Upload - CamCare</h1>
                </div>
                <p class="lead">
                    Lade Fotos, PDF- oder Videodateien für Auftrag <strong>#__ORDER_ID__</strong> hoch.
                    <br />
                    Max. <strong>__MAX_SIZE_MB__ MB</strong> pro Datei.
                </p>

                <div class="drop-zone" id="dropZone" role="button" tabindex="0" aria-label="Dateien auswählen">
                    <input type="file" id="fileInput" multiple accept="image/*,application/pdf,video/*" />
                    <div class="icon">&#128228;</div>
                    <div class="label">Antippen, um Dateien auszuw&auml;hlen</div>
                </div>

                <div class="progress"><div class="bar" id="progressBar"></div></div>
                <div class="status" id="status"></div>
            </div>
        """
            + """
        <script>
            (function () {
                "use strict";

                var input = document.getElementById("fileInput");
                var dropZone = document.getElementById("dropZone");
                var status = document.getElementById("status");
                var progressBar = document.getElementById("progressBar");
                var card = document.getElementById("card");

                var maxBytes = __MAX_SIZE_BYTES__;
                var apiUrl = "/api/upload/__TOKEN__";

                function showMessage(type, title, items) {
                    var div = document.createElement("div");
                    div.className = "message " + type;

                    var strong = document.createElement("strong");
                    strong.textContent = title;
                    div.appendChild(strong);

                    if (items && items.length) {
                        var ul = document.createElement("ul");
                        items.forEach(function (m) {
                            var li = document.createElement("li");
                            li.textContent = m;
                            ul.appendChild(li);
                        });
                        div.appendChild(ul);
                    }

                    status.appendChild(div);
                }

                function isSupported(type) {
                    return type && (type.indexOf("image/") === 0 || type.indexOf("video/") === 0 || type === "application/pdf");
                }

                function uploadFiles(files) {
                    status.innerHTML = "";
                    progressBar.style.width = "0%";
                    card.classList.add("uploading");

                    var done = 0;
                    var uploaded = [];
                    var errors = [];
                    var linkInvalid = false;

                    function finish() {
                        card.classList.remove("uploading");
                        input.disabled = false;
                        input.value = "";
                        if (uploaded.length) showMessage("success", "Erfolgreich hochgeladen:", uploaded);
                        if (errors.length) showMessage("error", "Folgende Dateien konnten nicht hochgeladen werden:", errors);
                        if (linkInvalid) showMessage("error", "Der Upload-Link ist ung\u00fcltig oder abgelaufen. Bitte fordere einen neuen Link \u00fcber das B\u00fcro an.");
                    }

                    files.forEach(function (file) {
                        if (!isSupported(file.type)) {
                            errors.push('"' + file.name + '" - kein unterst\u00fctzter Dateityp (nur Bild-, PDF- und Videodateien).');
                            done++;
                            if (done === files.length) finish();
                            return;
                        }
                        if (file.size > maxBytes) {
                            errors.push('"' + file.name + '" - Datei ist gr\u00f6\u00dfer als ' + Math.round(maxBytes / 1048576) + ' MB.');
                            done++;
                            if (done === files.length) finish();
                            return;
                        }

                        input.disabled = true;

                        var fd = new FormData();
                        fd.append("files", file, file.name);

                        fetch(apiUrl, { method: "POST", body: fd })
                            .then(function (res) {
                                if (res.status === 404) {
                                    linkInvalid = true;
                                    return { files: [], errors: [] };
                                }
                                return res.json().then(function (j) {
                                    return (j && typeof j === "object") ? j : { files: [], errors: [] };
                                });
                            })
                            .then(function (result) {
                                if (result.files) uploaded = uploaded.concat(result.files);
                                if (result.errors) errors = errors.concat(result.errors);
                            })
                            .catch(function () {
                                errors.push("Netzwerk- oder Serverfehler beim Upload von '" + file.name + "'.");
                            })
                            .then(function () {
                                done++;
                                progressBar.style.width = Math.round((done / files.length) * 100) + "%";
                                if (done === files.length) finish();
                            });
                    });
                }

                input.addEventListener("change", function () {
                    var files = Array.prototype.slice.call(input.files || []);
                    if (files.length) uploadFiles(files);
                });

                ["dragover", "dragenter"].forEach(function (ev) {
                    dropZone.addEventListener(ev, function (e) {
                        e.preventDefault();
                        dropZone.classList.add("drag");
                    });
                });
                ["dragleave", "drop"].forEach(function (ev) {
                    dropZone.addEventListener(ev, function (e) {
                        e.preventDefault();
                        dropZone.classList.remove("drag");
                    });
                });
                dropZone.addEventListener("drop", function (e) {
                    if (e.dataTransfer && e.dataTransfer.files && e.dataTransfer.files.length) {
                        uploadFiles(Array.prototype.slice.call(e.dataTransfer.files));
                    }
                });
                dropZone.addEventListener("keydown", function (e) {
                    if (e.key === "Enter" || e.key === " ") {
                        e.preventDefault();
                        input.click();
                    }
                });
            })();
        </script>
        </body>
        </html>
        """;
    }

}
