using ImageGenerator.Dtos;
using ImageGenerator.Interface;
using ImageGenerator.Models;

namespace ImageGenerator.Provider
{
    /// <summary>
    /// Stub provider that returns local static images for development and testing.
    /// </summary>
    public class StubProvider : IImageGenerationClient
    {
        private readonly IConfiguration _configuration;
        private readonly string _staticImagesRoot;
        private static readonly Random _random = new();

        // 预置的静态图片库
        private static readonly string[] SampleImages = 
        {
            "sample-1.png",
            "sample-2.png",
            "sample-3.png",
            "sample-4.png",
            "sample-5.png",
            "sample-6.png"
        };

        public StubProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _staticImagesRoot = _configuration["StaticImagesRoot"] ?? "images/presets";
        }

        public Task<BinaryData> GenerateImageAsync(string prompt, object options)
        {
            // 从预置图片中随机选择一张
            var randomImage = SampleImages[_random.Next(SampleImages.Length)];
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), _staticImagesRoot, randomImage);

            if (!File.Exists(imagePath))
            {
                // 如果文件不存在，创建一个简单的占位图片
                return Task.FromResult(CreatePlaceholderImage(prompt));
            }

            var imageBytes = File.ReadAllBytes(imagePath);
            return Task.FromResult(new BinaryData(imageBytes));
        }

        public Task<BinaryData> GenerateImageFromImageAsync(string prompt, Image[] images, object options)
        {
            // 对于 ImageToImage，也返回静态图片（开发态）
            return GenerateImageAsync(prompt, options);
        }

        public object ConvertOptions(GenerateImageDto generateDto)
        {
            // Stub provider 不需要特殊选项
            return new { };
        }

        public int GetCreditCost(GenerateImageDto generateDto)
        {
            // Stub provider 免费使用
            return 0;
        }

        /// <summary>
        /// 创建一个简单的占位图片（1x1 透明 PNG）
        /// </summary>
        private static BinaryData CreatePlaceholderImage(string prompt)
        {
            // 最小的有效 PNG (1x1 透明像素)
            byte[] pngBytes = 
            {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG signature
                0x00, 0x00, 0x00, 0x0D, // IHDR chunk length
                0x49, 0x48, 0x44, 0x52, // IHDR chunk type
                0x00, 0x00, 0x00, 0x01, // Width: 1
                0x00, 0x00, 0x00, 0x01, // Height: 1
                0x08, 0x06, 0x00, 0x00, 0x00, // Bit depth, color type, etc.
                0x1F, 0x15, 0xC4, 0x89, // CRC
                0x00, 0x00, 0x00, 0x0A, // IDAT chunk length
                0x49, 0x44, 0x41, 0x54, // IDAT chunk type
                0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00, 0x05, 0x00, 0x01, // Compressed data
                0x0D, 0x0A, 0x2D, 0xB4, // CRC
                0x00, 0x00, 0x00, 0x00, // IEND chunk length
                0x49, 0x45, 0x4E, 0x44, // IEND chunk type
                0xAE, 0x42, 0x60, 0x82  // CRC
            };

            Console.WriteLine($"Warning: Stub image not found, using placeholder for prompt: {prompt}");
            return new BinaryData(pngBytes);
        }
    }
}
