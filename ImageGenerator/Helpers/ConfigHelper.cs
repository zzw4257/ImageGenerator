using ImageGenerator.Database;
using ImageGenerator.Interface;
using ImageGenerator.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace ImageGenerator.Helpers;

/// <summary>
/// A helper class for configuring services.
/// </summary>
public static class ConfigHelper
{
    /// <summary>
    /// Registers all the necessary services for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration manager.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection RegisterServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        JwtConfig jwtConfig = new();
        configuration.Bind("JwtConfig", jwtConfig);
        JwtHelper jwtHelper = new()
        {
            JwtConfig = jwtConfig
        };

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IInvitationService, InvitationService>();
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<IImageStorageService, ImageStorageService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        
        services.AddScoped<OpenAIClient>();
        services.AddScoped<GeminiClient>();
        services.AddScoped<ImageGenerationClientFactory>();

        services.AddSingleton(jwtHelper);

        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddAutoMapper(typeof(Program));
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddDbContext<IgDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("Source"))
        );
        services.AddEndpointsApiExplorer();
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidIssuer = jwtHelper.JwtConfig.Issuer,
                ValidAudience = jwtHelper.JwtConfig.Audience,
                IssuerSigningKey = jwtHelper.JwtConfig.SymmetricSecurityKey,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = true
            };
        });

        return services;
    }
}
