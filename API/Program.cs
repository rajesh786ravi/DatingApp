using System.Text;
using API.Controllers;
using API.Data;
using API.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Enable Datadog tracing
builder.Services.AddOpenTelemetry(); // optional for metrics

// Use App Insights with Connection String
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = "InstrumentationKey=a9d6e51b-fbbe-4ab0-8873-0388b179ef98;IngestionEndpoint=https://canadacentral-1.in.applicationinsights.azure.com/;LiveEndpoint=https://canadacentral.livediagnostics.monitor.azure.com/;ApplicationId=7f34b597-3fc5-43c7-b205-66dad63cb82f";
});

// ✅ Disable all logging providers (optional for clean console)
builder.Logging.ClearProviders();

// Add config-based logging setup
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

builder.Logging
    .AddConsole()
    .AddApplicationInsights();

// Add logging filter manually if you want to be sure
builder.Logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>(
    "", LogLevel.Information);

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

builder.Services.AddSingleton(serviceProvider =>
{
    var config = builder.Configuration.GetSection("CosmosDb");

    string account = config["Account"] ?? throw new InvalidOperationException("CosmosDb:Account not found");
    string key = config["Key"] ?? throw new InvalidOperationException("CosmosDb:Key not found");
    string database = config["DatabaseName"] ?? throw new InvalidOperationException("CosmosDb:DatabaseName not found");
    string container = config["ContainerName"] ?? throw new InvalidOperationException("CosmosDb:ContainerName not found");

    // ✅ Explicitly use key-based constructor
    CosmosClient client = new CosmosClient(account, key);

    return new CosmosDbService(client, database, container);
});

var app = builder.Build();

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

app.MapGet("/", () => "App is running"); // Root welcome message

app.Run();