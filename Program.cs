using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RealState.DbContexts;
using RealState.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;


var builder = WebApplication.CreateBuilder(args);

// Database Connection
var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));
var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'GlobalDbContext' not found.");
using (var connection = new MySqlConnection(ConnectionString))
{
    try
    {
        connection.Open();
        Console.WriteLine("Connection successful.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Connection failed: {ex.Message}");
    }
}
builder.Services.AddDbContextPool<GlobalDbContext>(options =>
    options.UseMySql(ConnectionString, serverVersion)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());

// Identity
builder.Services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<GlobalDbContext>()
        .AddDefaultTokenProviders();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


// Add services to the container.
builder.Services.AddControllers();

// Add configuration middleware
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
}
);
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// update database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<GlobalDbContext>();
        dbContext.Database.Migrate(); // Apply pending migrations
        Console.WriteLine($"Migration Success GlobalDbContext");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
    }
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
