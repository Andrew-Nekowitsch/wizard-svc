using System.Text;
using Data;
using Data.Commands;
using Data.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;
using Services;

namespace Extensions;

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddWizardIdlerServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        return builder;
    }

    public static WebApplicationBuilder AddWizardIdlerData(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAccountQueries, AccountQueries>();
        builder.Services.AddScoped<IAccountCommands, AccountCommands>();
        builder.Services.AddScoped<IAuthQueries, AuthQueries>();
        builder.Services.AddScoped<IAuthCommands, AuthCommands>();

        var connectionString = string.Empty;
        var dbUser = builder.Configuration["DB_USER"];
        var dbPassword = builder.Configuration["DB_PASSWORD"];
        var dbServer = builder.Configuration["DB_SERVER"];

        if (!string.IsNullOrEmpty(dbUser) && !string.IsNullOrEmpty(dbPassword) && !string.IsNullOrEmpty(dbServer))
        {
            connectionString = $"Server={dbServer};Database=wizards;User Id={dbUser};Password={dbPassword};TrustServerCertificate=True;Connection Timeout=30;";
        }
        else
        {
            throw new InvalidOperationException("Database connection string parameters are not configured properly. Please set DB_USER, DB_PASSWORD, and DB_SERVER in your environment variables or configuration.");
        }

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
        return builder;
    }

    public static WebApplicationBuilder AddWizardIdlerAuth(this WebApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        var secret = builder.Configuration["JWT_SECRET"];
        if (jwtSettings == null || string.IsNullOrEmpty(secret))
            throw new InvalidOperationException("JWT settings or secret is not configured.");
        jwtSettings.Secret = secret;

        builder.Services.Configure<JwtSettings>(options =>
        {
            options.Issuer = jwtSettings.Issuer;
            options.Audience = jwtSettings.Audience;
            options.Secret = jwtSettings.Secret;
            options.AccessTokenExpirationMinutes = jwtSettings.AccessTokenExpirationMinutes;
            options.RefreshTokenExpirationDays = jwtSettings.RefreshTokenExpirationDays;
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy => policy.WithOrigins([builder.Configuration["UI_URL"] ?? "http://localhost:5174", "http://192.168.1.3:5174/"])
                                .AllowAnyHeader()
                                .AllowAnyMethod());
        });
        return builder;
    }
}