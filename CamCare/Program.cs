using CamCare.Components;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Persistence;
using CamCare.Services;
using FluentValidation;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using Radzen;
using System.Reflection;

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
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSingleton<IMapper, Mapper>();
builder.Services.AddSingleton<IMasterdataService, MasterdataService>();
builder.Services.AddScoped<IRepairOrderStatusService, RepairOrderStatusService>();
builder.Services.AddScoped<ICameraTypeService, CameraTypeService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ICameraService, CameraService>();
builder.Services.AddScoped<ILogisticProviderService, LogisticProviderService>();
builder.Services.AddScoped<IRepairPositionService, RepairPositionService>();
builder.Services.AddScoped<IDefectiveService, DefectiveService>();
builder.Services.AddScoped<IIncludedComponentService, IncludedComponentService>();
builder.Services.AddScoped<IRepairOrderService, RepairOrderService>();
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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
