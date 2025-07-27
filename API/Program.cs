using System.Text;
using API.Controllers;
using API.Data;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

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

// JWT Token
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);

var jwtSettings = jwtSection.Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings!.Key!);

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
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true
    };
});


var app = builder.Build();

// ✅ Enable request timing middleware
app.UseMiddleware<RequestTimingMiddleware>();

// ✅ Setup pub-sub (demo/test scenario)
var publisher = app.Services.GetRequiredService<Publisher>();
var subscriber = app.Services.GetRequiredService<Subscriber>();
publisher.OnPublish += subscriber.OnPublish1;
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
