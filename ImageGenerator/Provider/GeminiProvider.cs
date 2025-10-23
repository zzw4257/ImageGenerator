using ImageGenerator.Dtos;
using ImageGenerator.Interface;
using ImageGenerator.Models;
using System.Text;
using System.Text.Json;

namespace ImageGenerator.Provider
{
    public class GeminiProvider(HttpClient httpClient, IConfiguration configuration) : IImageGenerationClient
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _apiKey = configuration["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini API key not configured");
        private readonly string _endPoint = configuration["Gemini:EndPoint"] ?? "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-preview-0514:generateContent";
        public object ConvertOptions(GenerateImageDto generateDto)
        {
            // Gemini API might not have the same options as DALL-E.
            // This can be a simple object or a dictionary.
            return new { };
        }

        public int GetCreditCost(GenerateImageDto generateDto)
        {
            // Gemini 默认费用：TextToImage=1, ImageToImage=2
            return generateDto.GenerationType switch
            {
                Enums.GenerationType.ImageToImage => 2,
                _ => 1
            };
        }

        public async Task<BinaryData> GenerateImageAsync(string prompt, object options)
        {
            var payload = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = $"generate an image of {prompt}" } } }
                },
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_endPoint}/models/gemini-2.5-flash-image-preview:generateContent")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("x-goog-api-key", _apiKey);

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Gemini API returned {(int)response.StatusCode}: {responseString}");
            }

            return GetImageData(responseString);
        }

        public async Task<BinaryData> GenerateImageFromImageAsync(string prompt, Image[] images, object options)
        {
            if (images == null || images.Length == 0)
                throw new ArgumentException("未找到任何可用的输入图片文件。");

            var parts = new List<object>
            {
                new { text = prompt }
            };

            foreach (var image in images)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), image.ImagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (!File.Exists(imagePath))
                {
                    Console.WriteLine($"图片文件不存在: {imagePath}");
                    continue;
                }

                var imageBytes = await File.ReadAllBytesAsync(imagePath);
                var imageBase64 = Convert.ToBase64String(imageBytes);
                var mimeType = GetMimeType(imagePath);

                parts.Add(new
                {
                    inline_data = new
                    {
                        mime_type = mimeType,
                        data = imageBase64
                    }
                });
            }

            if (parts.Count <= 1) // Only contains the prompt
            {
                throw new FileNotFoundException("所有提供的图片文件都无法访问。");
            }

            var payload = new
            {
                contents = new[]
                {
                    new { parts }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_endPoint}/models/gemini-2.5-flash-image-preview:generateContent")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("x-goog-api-key", _apiKey);

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Gemini API returned {(int)response.StatusCode}: {responseString}");
            }

            return GetImageData(responseString);
        }

        private static string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".heic" => "image/heic",
                ".heif" => "image/heif",
                _ => "application/octet-stream",
            };
        }

        private static BinaryData GetImageData(string responseString)
        {
            using var doc = JsonDocument.Parse(responseString);
            var candidates = doc.RootElement.GetProperty("candidates");
            var parts = candidates[0].GetProperty("content").GetProperty("parts");

            string? b64 = null;
            foreach (var part in parts.EnumerateArray())
            {
                if (part.TryGetProperty("inlineData", out var inlineData))
                {
                    if (inlineData.TryGetProperty("data", out var data))
                    {
                        b64 = data.GetString();
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(b64))
            {
                throw new InvalidOperationException("API returned empty image data.");
            }

            return new BinaryData(Convert.FromBase64String(b64));
        }
    }
}
