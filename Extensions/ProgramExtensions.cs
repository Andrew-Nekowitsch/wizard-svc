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
        builder.Services.AddScoped<IPlayerCommands, PlayerCommands>();
        builder.Services.AddScoped<IPlayerQueries, PlayerQueries>();
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

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        var secret = builder.Configuration["JWT_SECRET"];
        if (jwtSettings == null || string.IsNullOrEmpty(secret))
            throw new InvalidOperationException("JWT settings or secret is not configured.");
        jwtSettings.Secret = secret;

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
                policy => policy.WithOrigins("http://localhost:5173")
                                .AllowAnyHeader()
                                .AllowAnyMethod());
        });
        return builder;
    }
}