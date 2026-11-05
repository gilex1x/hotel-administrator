using System.Security.Claims;
using hotel_admin_api.Models;
using hotel_admin_api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SQLite;

var builder = WebApplication.CreateBuilder(args);
var configuredDatabasePath = builder.Configuration["Database:Path"];
if (string.IsNullOrWhiteSpace(configuredDatabasePath))
{
    throw new InvalidOperationException("Database:Path must be configured.");
}

var databasePath = Path.IsPathRooted(configuredDatabasePath)
    ? configuredDatabasePath
    : Path.Combine(builder.Environment.ContentRootPath, configuredDatabasePath);
var databaseDirectory = Path.GetDirectoryName(databasePath);
if (!string.IsNullOrEmpty(databaseDirectory))
{
    Directory.CreateDirectory(databaseDirectory);
}

var adminUsername = builder.Configuration["Authentication:AdminUsername"];
var adminPassword = builder.Configuration["Authentication:AdminPassword"];
if (string.IsNullOrWhiteSpace(adminUsername))
{
    throw new InvalidOperationException("Authentication:AdminUsername must be configured.");
}
if (string.IsNullOrWhiteSpace(adminPassword) || adminPassword.Length < 12)
{
    throw new InvalidOperationException(
        "Authentication:AdminPassword must be configured and contain at least 12 characters.");
}

var signingKey = JwtConfiguration.GetSigningKey(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton(_ => new SQLiteConnection(databasePath));
var frontendOrigin = builder.Configuration["Frontend:Origin"] ?? "http://localhost:3000";
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = true,
            ValidIssuer = JwtConfiguration.Issuer,
            ValidateAudience = true,
            ValidAudience = JwtConfiguration.Audience,
            ValidateLifetime = true,
            RoleClaimType = ClaimTypes.Role
        };
    });
builder.Services.AddAuthorization();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
var database = app.Services.GetRequiredService<SQLiteConnection>();
database.CreateTable<Hotel>();
database.CreateTable<User>();

if (!database.Table<Hotel>().Any())
{
    database.InsertAll(new[]
    {
        new Hotel { Name = "Hotel Sol", Address = "Calle Mayor 1, Madrid", Reviews = new List<int> { 5, 4, 5 } },
        new Hotel { Name = "Hotel Mediterráneo", Address = "Avenida del Mar 12, Barcelona", Reviews = new List<int> { 4, 5, 4 } },
        new Hotel { Name = "Hotel Alameda", Address = "Paseo de la Alameda 8, Valencia", Reviews = new List<int> { 4, 4, 3 } },
        new Hotel { Name = "Hotel Giralda", Address = "Calle Sierpes 20, Sevilla", Reviews = new List<int> { 5, 5, 4 } },
        new Hotel { Name = "Hotel Atlántico", Address = "Rúa Real 15, A Coruña", Reviews = new List<int> { 3, 4, 4 } },
        new Hotel { Name = "Hotel Sierra", Address = "Gran Vía 33, Bilbao", Reviews = new List<int> { 4, 3, 4 } },
        new Hotel { Name = "Hotel Jardines", Address = "Calle de los Jardines 6, Málaga", Reviews = new List<int> { 5, 4, 5 } },
        new Hotel { Name = "Hotel Catedral", Address = "Plaza de la Catedral 2, Santiago de Compostela", Reviews = new List<int> { 4, 5, 5 } },
        new Hotel { Name = "Hotel Naranjos", Address = "Calle Colón 18, Córdoba", Reviews = new List<int> { 3, 4, 3 } },
        new Hotel { Name = "Hotel Castillo", Address = "Paseo del Castillo 10, Toledo", Reviews = new List<int> { 5, 4, 4 } }
    });
}

var seededAdmin = database.Table<User>().FirstOrDefault(user => user.Username == adminUsername);
if (seededAdmin is null)
{
    if (database.Table<User>().Any())
    {
        throw new InvalidOperationException(
            "The configured admin username does not exist in the Users table. Configure an existing user.");
    }

    database.Insert(new User
    {
        Username = adminUsername,
        PasswordHash = PasswordHashing.Hash(adminPassword),
        Role = "Admin"
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
