using ImageGenerator.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ImageGenerator.Services
{
    public class ImageGenerationClientFactory(IServiceProvider serviceProvider)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public IImageGenerationClient GetClient(string clientType)
        {
            return clientType.ToLower() switch
            {
                "openai" => _serviceProvider.GetRequiredService<OpenAIClient>(),
                "gemini" => _serviceProvider.GetRequiredService<GeminiClient>(),
                _ => throw new NotSupportedException($"Client type '{clientType}' is not supported."),
            };
        }
    }
}
