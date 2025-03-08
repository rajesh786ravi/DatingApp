using API.Controllers;
using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext to the container.
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Registering the delegate
builder.Services.AddSingleton<Func<int, string>>(provider => num => $"Processed Number: {num}");

// Register the service that depends on the delegate
builder.Services.AddSingleton<MyDelegateService>();

builder.Services.AddSingleton<EmailService>();

builder.Services.AddCors();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Registering Delegate in DI Container 
builder.Services.AddSingleton<Func<int, string>>(x => num => $"Number: {num}");
builder.Services.AddSingleton<MyDelegateService>();

var app = builder.Build();

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
