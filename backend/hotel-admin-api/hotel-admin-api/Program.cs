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

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton(_ => new SQLiteConnection(databasePath));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
app.Services.GetRequiredService<SQLiteConnection>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
