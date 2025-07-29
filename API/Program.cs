using System.Text;
using API.Controllers;
using API.Data;
using API.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using Datadog.Trace;


var builder = WebApplication.CreateBuilder(args);

// Enable Datadog tracing
builder.Services.AddOpenTelemetry(); // optional for metrics

// ✅ Disable all logging providers (optional for clean console)
builder.Logging.ClearProviders();

// ✅ Register services
builder.Services.AddControllers(); // Enables [ApiController] based routing

// ✅ Add DbContext using SQLite connection string
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// ✅ Dependency Injection (custom services & delegates)
builder.Services.AddSingleton<Func<int, string>>(provider => num => $"Processed Number: {num}");
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<MyDelegateService>();
builder.Services.AddScoped<MatchService>();
builder.Services.AddSingleton<Publisher>();
builder.Services.AddSingleton<Subscriber>();
builder.Services.AddSingleton<AzureQueueDemo.Services.AzureQueueService>();
builder.Services.AddSingleton<GoogleDriveService>();
builder.Services.AddSingleton<JwtTokenService>();


// ✅ CORS
builder.Services.AddCors();

// ✅ Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Key Vault
var keyVaultUrl = "https://ourkeyvaultname.vault.azure.net/";
builder.Configuration.AddAzureKeyVault(
    new Uri(keyVaultUrl),
    new DefaultAzureCredential());

var jwtKey = builder.Configuration["JWT-Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT key is missing from configuration or Key Vault.");
}
// JWT Token
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(options =>
{
    builder.Configuration.GetSection("Jwt").Bind(options);
    options.Key = jwtKey;
});

var jwtSettings = jwtSection.Get<JwtSettings>();
// var key = Encoding.ASCII.GetBytes(jwtSettings!.Key!);
var key = Encoding.ASCII.GetBytes(jwtKey!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings?.Audience,
        ValidateLifetime = true
    };
});


var app = builder.Build();

// app.UseExceptionHandler(errorApp =>
// {
//     errorApp.Run(async context =>
//     {
//         var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

//         var path = context.Request.Path;
//         var method = context.Request.Method;

//         logger.LogError("🚨 Global exception handler triggered for {Method} {Path}", method, path);
//         Console.WriteLine($"[GlobalExceptionHandler] Path: {path}, Method: {method}");

//         context.Response.StatusCode = 500;
//         context.Response.ContentType = "application/json";

//         var feature = context.Features.Get<IExceptionHandlerFeature>();
//         if (feature != null)
//         {
//             var exception = feature.Error;
//             logger.LogError(exception, "Unhandled exception occurred");

//             var response = new
//             {
//                 error = true,
//                 message = app.Environment.IsDevelopment()
//                     ? exception.Message
//                     : "Internal server error"
//             };

//             await context.Response.WriteAsJsonAsync(response);
//         }
//     });
// });

// ✅ Enable request timing middleware
app.UseMiddleware<RequestTimingMiddleware>();

// ✅ Setup pub-sub (demo/test scenario)
var publisher = app.Services.GetRequiredService<Publisher>();
var subscriber = app.Services.GetRequiredService<Subscriber>();
publisher.OnPublish += subscriber.Subscriber1;
publisher.OnPublish += subscriber.Subscriber2;
publisher.Publish("Hello from Program.cs!");

// ✅ Apply pending EF migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate(); // Applies any new migrations automatically
}

// ✅ CORS configuration (Angular on localhost)
app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("http://localhost:4200", "https://localhost:4200"));

// ✅ HTTPS redirection (optional for production)
app.UseHttpsRedirection();

// JWT Token
app.UseAuthentication(); // This MUST come before UseAuthorization

// ✅ Authorization middleware (required if using [Authorize])
app.UseAuthorization();

// ✅ Enable Swagger (only in development mode)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Swagger UI at root: https://localhost:5001/
    });
}

// ✅ Map API endpoints and default route
app.MapControllers(); // Maps [ApiController] routes like /api/payment

// Create manual trace scope
// app.Use(async (context, next) =>
// {
//     using var scope = Tracer.Instance.StartActive("web.request");
//     scope.Span.ResourceName = $"{context.Request.Method} {context.Request.Path}";
//     await next.Invoke();
// });

app.MapGet("/", () => "App is running"); // Root welcome message

app.Run();
