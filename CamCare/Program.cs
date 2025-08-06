using CamCare.Components;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Persistence;
using CamCare.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddPersistence();
builder.Services.AddSingleton<IMapper, Mapper>();
builder.Services.AddScoped<IRepairOrderStatusService, RepairOrderStatusService>();
builder.Services.AddScoped<ICameraTypeService, CameraTypeService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICameraService, CameraService>();
builder.Services.AddScoped<ILogisticProviderService, LogisticProviderService>();
builder.Services.AddScoped<IRepairPositionService, RepairPositionService>();
builder.Services.AddScoped<IDefectiveService, DefectiveService>();
builder.Services.AddScoped<IRepairOrderService, RepairOrderService>();

var app = builder.Build();

AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    Console.WriteLine(e.ExceptionObject.ToString());
    };

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    var dbContextFactory = app.Services.GetRequiredService<IAppDbContextFactory>();
    using var dbContext = dbContextFactory.CreateDbContext();
    await dbContext.Database.MigrateAsync();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
