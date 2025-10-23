using ImageGenerator.Dtos;
using ImageGenerator.Interface;
using ImageGenerator.Models;

namespace ImageGenerator.Provider
{
    /// <summary>
    /// Stub provider that returns local static images for development and testing.
    /// Registered as Singleton and preloads image list at startup for performance.
    /// </summary>
    public class StubProvider : IImageGenerationClient
    {
        private readonly string _staticImagesRoot;
        private readonly List<string> _availableImages;
        private static readonly Random _random = new();
        private readonly byte[] _placeholderImage;

        public StubProvider(IConfiguration configuration)
        {
            _staticImagesRoot = configuration["StaticImagesRoot"] ?? "images/presets";
            _availableImages = [];
            
            // 在启动时遍历文件夹，预加载图片列表
            LoadAvailableImages();
            
            // 预生成占位图片
            _placeholderImage = GeneratePlaceholderImageBytes();
            
            Console.WriteLine($"[StubProvider] Initialized with {_availableImages.Count} images from {_staticImagesRoot}");
        }

        /// <summary>
        /// 启动时加载所有可用的图片文件
        /// </summary>
        private void LoadAvailableImages()
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), _staticImagesRoot);
                
                if (!Directory.Exists(fullPath))
                {
                    Console.WriteLine($"[StubProvider] Warning: Directory not found: {fullPath}");
                    Directory.CreateDirectory(fullPath);
                    return;
                }

                // 支持多种图片格式
                var supportedExtensions = new[] { ".png", ".jpg", ".jpeg", ".webp" };
                var imageFiles = Directory.GetFiles(fullPath)
                    .Where(f => supportedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    .Select(f => Path.GetFileName(f))
                    .ToList();

                _availableImages.AddRange(imageFiles);

                if (_availableImages.Count == 0)
                {
                    Console.WriteLine($"[StubProvider] Warning: No image files found in {fullPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StubProvider] Error loading images: {ex.Message}");
            }
        }

        public Task<BinaryData> GenerateImageAsync(string prompt, object options)
        {
            // 如果没有可用图片，返回占位图
            if (_availableImages.Count == 0)
            {
                Console.WriteLine($"[StubProvider] No images available, using placeholder for prompt: {prompt}");
                return Task.FromResult(new BinaryData(_placeholderImage));
            }

            // 从预加载的图片列表中随机选择一张
            var randomImage = _availableImages[_random.Next(_availableImages.Count)];
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), _staticImagesRoot, randomImage);

            try
            {
                var imageBytes = File.ReadAllBytes(imagePath);
                return Task.FromResult(new BinaryData(imageBytes));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StubProvider] Error reading image {randomImage}: {ex.Message}, using placeholder");
                return Task.FromResult(new BinaryData(_placeholderImage));
            }
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
        /// 生成占位图片字节数组（1x1 透明 PNG）
        /// </summary>
        private static byte[] GeneratePlaceholderImageBytes()
        {
            // 最小的有效 PNG (1x1 透明像素)
            return
            [
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
            ];
        }
    }
}
