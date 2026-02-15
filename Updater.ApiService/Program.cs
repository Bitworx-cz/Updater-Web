using Updater.ApiService.Database;
using Microsoft.EntityFrameworkCore;
using Updater.ApiService.Services;
using Updater.ApiService.Cache;
using Updater.ApiService;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Enable MVC Controllers
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// 🔥 Add Swagger (debug only)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
});

// DI services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<SoftwareService>();
builder.Services.AddScoped<DeviceService>();
builder.Services.AddSingleton<UserCache>();
builder.Services.AddSingleton<SoftwareCache>();
builder.Services.AddSingleton<Configuration>();
builder.Services.AddTransient<Helpers>();

builder.Services.AddDbContext<Context>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseExceptionHandler();

// 🔥 Enable Swagger only in development (debug)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Updater API v1");
        options.DisplayRequestDuration();        // show timings
        options.EnableValidator();               // validate schemas
        options.DefaultModelsExpandDepth(-1);    // hide schema view for cleaner UI
    });
}

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();
