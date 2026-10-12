using System.Security.Claims;
using CamCare.Components;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Options;
using CamCare.Persistence;
using CamCare.Security;
using CamCare.Services;
using FluentValidation;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddNLog();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(_Imports).Assembly);
#if DEBUG
    options.UseReduxDevTools();
#endif
});


builder.Services.AddRadzenComponents();
builder.Services.AddMemoryCache();
builder.Services.AddHealthChecks();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Authentifizierung & Autorisierung
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "CamCare.Auth";
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthPolicies.AdminOnly, policy =>
        policy.RequireClaim(ClaimTypes.Role, UserGroupRoles.Admin));
    options.AddPolicy(AuthPolicies.CanCreateOrders, policy =>
        policy.RequireClaim(ClaimTypes.Role, UserGroupRoles.Admin, UserGroupRoles.Buero));
});

builder.Services.AddPersistence(builder.Configuration);
builder.Services.Configure<ObjectStorageOptions>(builder.Configuration.GetSection(ObjectStorageOptions.SectionName));
builder.Services.Configure<AppVersionOptions>(builder.Configuration.GetSection(AppVersionOptions.SectionName));
builder.Services.Configure<UploadLinkOptions>(builder.Configuration.GetSection(UploadLinkOptions.SectionName));
builder.Services.AddSingleton<IObjectStorageService, MinioObjectStorageService>();
builder.Services.AddSingleton<IMapper, Mapper>();
builder.Services.AddSingleton<IMasterdataService, MasterdataService>();
builder.Services.AddSingleton<IUploadTokenService, UploadTokenService>();
builder.Services.AddScoped<IRepairOrderStatusService, RepairOrderStatusService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ILogisticProviderService, LogisticProviderService>();
builder.Services.AddScoped<IRepairPositionService, RepairPositionService>();
builder.Services.AddScoped<IDefectiveService, DefectiveService>();
builder.Services.AddScoped<IIncludedComponentService, IncludedComponentService>();
builder.Services.AddScoped<IRepairOrderService, RepairOrderService>();
builder.Services.AddSingleton<IRepairOrderChangeBroadcaster, RepairOrderChangeBroadcaster>();
builder.Services.AddScoped<IRepairOrderChangeSubscriber, RepairOrderChangeSubscriber>();
builder.Services.AddScoped<IDataStoreService, DataStoreService>();
builder.Services.AddScoped<IAmicronDataService, AmicronDataService>();

builder.Services.AddScoped<IKrdDataService, KrdDataService>();
builder.Services.AddScoped<IUserGroupService, UserGroupService>();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    logger.LogCritical((Exception)e.ExceptionObject, "Unhandled Exception thrown!");
};

using var scope = app.Services.CreateScope();
var dbContextFactory = scope.ServiceProvider.GetRequiredService<IAppDbContextFactory>();
using var dbContext = dbContextFactory.CreateDbContext();
await dbContext.Database.MigrateAsync();

var masterdataService = scope.ServiceProvider.GetRequiredService<IMasterdataService>();
await masterdataService.InitializeAsync();

var userGroupService = scope.ServiceProvider.GetRequiredService<IUserGroupService>();
await userGroupService.InitializeAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

}

// Forwarded-Header des Reverse-Proxys (Nginx Proxy Manager) auswerten, damit
// UseHttpsRedirection und generierte URLs (z. B. Datei-Downloads) hinter dem Proxy
// das korrekte Schema (https) und den Host verwenden.
// Vertraut den X-Forwarded-*-Headern aus privaten Docker-Netzwerken (Proxy-Manager,
// camcare-Netz) – in Produktion ggf. auf das tatsächliche Proxy-Subnetz einschränken.
{
    var forwardedHeadersOptions = new ForwardedHeadersOptions
    {
        ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
    };
    forwardedHeadersOptions.KnownIPNetworks.Add(System.Net.IPNetwork.Parse("10.0.0.0/8"));
    forwardedHeadersOptions.KnownIPNetworks.Add(System.Net.IPNetwork.Parse("172.16.0.0/12"));
    forwardedHeadersOptions.KnownIPNetworks.Add(System.Net.IPNetwork.Parse("192.168.0.0/16"));
    forwardedHeadersOptions.KnownIPNetworks.Add(System.Net.IPNetwork.Parse("127.0.0.0/8"));
    forwardedHeadersOptions.KnownIPNetworks.Add(System.Net.IPNetwork.Parse("::1/128"));
    app.UseForwardedHeaders(forwardedHeadersOptions);
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapHealthChecks("/health");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Anmeldung / Abmeldung per Cookie. Der Aufruf erfolgt per fetch() direkt aus dem Browser
// (siehe wwwroot/js/utils.js), damit der Auth-Cookie zuverlässig im Browser gesetzt wird.
// Nach Erfolg sorgt ein Full-Reload (Redirect) dafür, dass der frische Blazor-Circuit
// die neuen Claims aus dem Cookie übernimmt.
app.MapPost("/api/auth/login", async (HttpContext context, [FromBody] LoginRequest request, IUserGroupService userGroupService) =>
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

app.MapPost("/api/auth/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Json(new AuthResult { Redirect = "/Login" });
});

app.MapGet("/api/datastores/{id:int}/file", async (int id, bool? download, IDataStoreService dataStoreService) =>
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

app.Run();

/// <summary>
/// Anmelde-Daten des Login-Dialogs (POST /api/auth/login, JSON).
/// </summary>
internal sealed class LoginRequest
{
    public string Group { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Antwort der Auth-Endpunkte: Ziel-URL für den Full-Reload nach Login/Logout.
/// </summary>
internal sealed class AuthResult
{
    public string Redirect { get; set; } = "/";
}
