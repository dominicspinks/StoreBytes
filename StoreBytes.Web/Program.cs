using StoreBytes.Web.Services;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using StoreBytes.Web.Utilities;
using StoreBytes.Common.Configuration;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddHttpContextAccessor();

// Configure HTTP client
builder.Services.AddHttpClient("StoreBytesAPI", client =>
{
    var apiUrl = builder.Configuration[ConfigurationKeys.WebApp.StoreBytesApiUrl];
    if (string.IsNullOrEmpty(apiUrl))
    {
        throw new Exception("API URL is not configured. Ensure the STOREBYTESAPI_URL environment variable is set.");
    }
    client.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BucketService>();

// Add JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration[ConfigurationKeys.Shared.JwtSecret])),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.HttpContext.Request.Cookies["AuthToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Middleware to enforce login for non-public pages
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    if (!context.User.Identity.IsAuthenticated && !PublicPages.Pages.Contains(path))
    {
        context.Response.Redirect("/Auth/Login");
        return;
    }

    await next();
});

app.MapRazorPages();

app.Run();
