using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StoreBytes.DataAccess.Data;
using StoreBytes.DataAccess.Databases;
using StoreBytes.Service.Files;
using System.Text;
using DotNetEnv;
using StoreBytes.API.Utilities;
using StoreBytes.Common.Configuration;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

var defaultEnvironmentVariables = new Dictionary<string, string>
{
    { ConfigurationKeys.Shared.HashSecret, "this_is_the_hash_secret" },
    { ConfigurationKeys.Shared.JwtSecret, "this_is_the_jwt_secret" }
};

var requiredEnvironmentVariables = new List<string>
{
    ConfigurationKeys.Api.DatabaseUrl
};

foreach (var variable in requiredEnvironmentVariables)
{
    var value = Environment.GetEnvironmentVariable(variable);
    if (string.IsNullOrEmpty(value))
    {
        Console.WriteLine($"ERROR: Environment variable '{variable}' is not set.");
        return; // Exit the application
    }
}

builder.Configuration
    .AddInMemoryCollection(defaultEnvironmentVariables)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Logging.AddConsole();
builder.Logging.AddDebug();


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<IPGSqlDataAccess, PGSqlDataAccess>();
builder.Services.AddSingleton<IDatabaseData, PGSqlData>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton(new FileStorageService(builder.Configuration[ConfigurationKeys.Api.FilesBasePath]));
builder.Services.AddScoped<JwtHelper>();


// Configure JWT authentication
var jwtKey = builder.Configuration[ConfigurationKeys.Shared.JwtSecret];
var key = Encoding.ASCII.GetBytes(jwtKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
