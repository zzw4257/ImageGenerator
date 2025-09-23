using ImageGenerator.Database;
using ImageGenerator.Interface;
using ImageGenerator.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace ImageGenerator.Helpers;

public static class ConfigHelper
{
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
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        
        services.AddScoped<OpenAIClient>();
        services.AddScoped<GeminiClient>();
        services.AddScoped<ImageGenerationClientFactory>();

        services.AddSingleton(jwtHelper);

        services.AddHttpContextAccessor(); // 注册 IHttpContextAccessor
        services.AddHttpClient(); // 注册 HttpClient
        services.AddAutoMapper(typeof(Program)); // 注册 AutoMapper
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
