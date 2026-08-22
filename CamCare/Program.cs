using CamCare.Components;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Options;
using CamCare.Persistence;
using CamCare.Services;
using FluentValidation;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
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
builder.Services.AddPersistence(builder.Configuration);
builder.Services.Configure<ObjectStorageOptions>(builder.Configuration.GetSection(ObjectStorageOptions.SectionName));
builder.Services.Configure<AppVersionOptions>(builder.Configuration.GetSection(AppVersionOptions.SectionName));
builder.Services.AddSingleton<IObjectStorageService, MinioObjectStorageService>();
builder.Services.AddSingleton<IMapper, Mapper>();
builder.Services.AddSingleton<IMasterdataService, MasterdataService>();
builder.Services.AddScoped<IRepairOrderStatusService, RepairOrderStatusService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ILogisticProviderService, LogisticProviderService>();
builder.Services.AddScoped<IRepairPositionService, RepairPositionService>();
builder.Services.AddScoped<IDefectiveService, DefectiveService>();
builder.Services.AddScoped<IIncludedComponentService, IncludedComponentService>();
builder.Services.AddScoped<IRepairOrderService, RepairOrderService>();
builder.Services.AddScoped<IDataStoreService, DataStoreService>();
builder.Services.AddScoped<IAmicronDataService, AmicronDataService>();

builder.Services.AddScoped<IKrdDataService, KrdDataService>();

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapHealthChecks("/health");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

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
});

app.Run();
