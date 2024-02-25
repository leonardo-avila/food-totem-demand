using System.Text.Json.Serialization;
using FoodTotem.Demand.API.Setup;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true); 

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();


// Add services to the container.

builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new DashRouteConvention());
    }).AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Configure Swagger options
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Food Totem Demand API", Version = "v1" });
    var filePath = Path.Combine(AppContext.BaseDirectory, "FoodTotem.Demand.API.xml");
    c.IncludeXmlComments(filePath);
});

// Set DbContexts
builder.Services.AddDatabaseConfiguration(builder.Configuration);

// Dependency Injection
builder.Services.AddDemandServices();
builder.Services.AddCommunicationServices();


var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Type", "application/json");
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    await next.Invoke();
});

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
