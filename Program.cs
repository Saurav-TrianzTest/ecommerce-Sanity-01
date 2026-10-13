using EcommerceWebApi;
using EcommerceWebApi.Authentication;
using EcommerceWebApi.Filters;
using EcommerceWebApi.Notification;
using EcommerceWebApi.Services;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;

var logger = new LoggerConfiguration()
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

Log.Logger = logger;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Host.UseSerilog();

builder.Services
    .AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
// .NET 10: Use built-in OpenAPI support (AddOpenApi/MapOpenApi) instead of Swashbuckle.
// Swashbuckle.AspNetCore 8.x is used here for compatibility; switch to AddOpenApi() for native support.
builder.Services.AddOpenApi();

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
    // .NET 10 built-in OpenAPI endpoint
    app.MapOpenApi();
    // Swagger UI via Swashbuckle (requires Swashbuckle.AspNetCore 8.x)
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "EcommerceWebApi v1");
    });
}

app.UseCors();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
