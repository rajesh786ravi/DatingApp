using API.Controllers;
using API.Data;
using API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders(); // Suppress all framework logs

// Add services to the container.
builder.Services.AddControllers();  //Just adding the controller's

// Add DbContext to the container.
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<Func<int, string>>(provider => num => $"Processed Number: {num}");

builder.Services.AddSingleton<EmailService>();

builder.Services.AddCors();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registering Delegate in DI Container 
builder.Services.AddSingleton<MyDelegateService>();
builder.Services.AddScoped<MatchService>();
builder.Services.AddSingleton<Publisher>();
builder.Services.AddSingleton<Subscriber>();

// Registering google drive service
builder.Services.AddSingleton<GoogleDriveService>();


var app = builder.Build();
app.UseMiddleware<RequestTimingMiddleware>();
var publisher = app.Services.GetRequiredService<Publisher>();
var subscriber = app.Services.GetRequiredService<Subscriber>();
publisher.OnPublish += subscriber.OnPublish1;

// Apply migrations on startup (only for development or testing)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate(); // Apply migrations programmatically
}

// Optional: Simulate event (for demo/testing)
publisher.Publish("Hello from Program.cs!");
app.UseCors(x => x
.AllowAnyHeader()
.AllowAnyMethod()
.WithOrigins("http://localhost:4200", "https://localhost:4200"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // This will make Swagger UI the default page (optional)
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();