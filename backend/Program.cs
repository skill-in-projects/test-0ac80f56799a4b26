var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Database connection string from Railway environment variable
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
}

// Configure URL for Railway deployment
var port = Environment.GetEnvironmentVariable("PORT");
var url = string.IsNullOrEmpty(port) ? "http://0.0.0.0:8080" : $"http://0.0.0.0:{port}";
builder.WebHost.UseUrls(url);

var app = builder.Build();

// Enable Swagger in all environments (including production)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Add a simple root route to verify the service is running
app.MapGet("/", () => new { 
    message = "Backend API is running", 
    status = "ok",
    swagger = "/swagger",
    api = "/api/test"
});

app.Run();
