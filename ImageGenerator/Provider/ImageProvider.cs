using ImageGenerator.Interface;

namespace ImageGenerator.Provider
{
    public class ImageProvider(IServiceProvider serviceProvider)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public IImageGenerationClient GetClient(string clientType)
        {
            return clientType.ToLower() switch
            {
                "stub" => _serviceProvider.GetRequiredService<StubProvider>(),
                "openai" => _serviceProvider.GetRequiredService<OpenAIProvider>(),
                "gemini" => _serviceProvider.GetRequiredService<GeminiProvider>(),
                _ => throw new NotSupportedException($"Client type '{clientType}' is not supported."),
            };
        }
    }
}
