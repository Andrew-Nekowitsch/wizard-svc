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


        var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string not found.");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
        return builder;
    }

    public static WebApplicationBuilder AddWizardIdlerAuth(this WebApplicationBuilder builder)
    {

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret))
            throw new InvalidOperationException("JWT settings or secret is not configured.");

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