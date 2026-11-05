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
    throw new InvalidOperationException("Authentication:AdminPassword must be configured and contain at least 12 characters.");
}

var signingKey = JwtConfiguration.GetSigningKey(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton(_ => new SQLiteConnection(databasePath));
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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
