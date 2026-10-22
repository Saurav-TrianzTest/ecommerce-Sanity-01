using EcommerceWebApi;
using EcommerceWebApi.Authentication;
using EcommerceWebApi.Filters;
using EcommerceWebApi.Notification;
using EcommerceWebApi.Services;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Configure Serilog for .NET 8 using UseSerilog on the host builder
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.File(
        "Logs/log-.txt",
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(5),
        rollingInterval: RollingInterval.Day
    )
    .CreateLogger();

builder.Host.UseSerilog();

// Register controllers once with JSON options (duplicate AddControllers() removed)
builder.Services
    .AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();

// Swagger/OpenAPI configuration for .NET 8
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "EcommerceWebApi",
        Version = "v1"
    });
});

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("ApplicationSettings"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:3001")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddScoped<UnitOfWork>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderService>();

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<TotpService>();
builder.Services.AddScoped<AuthorizeFilter>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddSignalR();

builder.Services.AddSingleton<NotificationSubject>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "EcommerceWebApi v1");
    });
}

app.UseCors();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
