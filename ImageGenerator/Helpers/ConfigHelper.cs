using ImageGenerator.Database;
using ImageGenerator.Interface;
using ImageGenerator.Services;
using ImageGenerator.Provider;
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

        CreditCostSettings creditSettings = new();
        // 将 "CreditCosts" 整个部分绑定到 'Costs' 字典属性中
        configuration.GetSection("CreditCosts").Bind(creditSettings.Costs);
        
        

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IInvitationService, InvitationService>();
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<IImageStorageService, ImageStorageService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<IGenerateService, GenerateService>();
        services.AddScoped<IPresetService, PresetService>();
        services.AddScoped<IPresetEngagementService, PresetEngagementService>();
        services.AddScoped<IPresetReportService, PresetReportService>();
        services.AddScoped<IRankingService, RankingService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<ICostEstimationService, CostEstimationService>();
        
        // Register Providers as Singleton (stateless, can be reused)
        services.AddSingleton<StubProvider>();
        services.AddSingleton<OpenAIProvider>();
        services.AddSingleton<GeminiProvider>();
        services.AddSingleton<ImageProvider>();

        services.AddSingleton(jwtHelper);

        // 将价目表注册为单例，以便所有服务都能读取
        services.AddSingleton(creditSettings);

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
