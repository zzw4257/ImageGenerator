using ImageGenerator.Dtos;
using ImageGenerator.Interface;
using ImageGenerator.Models;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Images;
using System;
using System.ClientModel;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImageGenerator.Services
{
    public class OpenAIClient : IImageGenerationClient
    {
        private readonly ImageClient _client;
        private readonly string _apiKey;
        private readonly string _endPoint;
        private readonly HttpClient _httpClient;

        public OpenAIClient(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API Key 未配置");
            _endPoint = configuration["OpenAI:EndPoint"] ?? "https://api.openai.com/v1";

            var model = "gpt-image-1";
            var endPoint = new Uri(_endPoint);

            _client = new ImageClient(model, new ApiKeyCredential(_apiKey), new OpenAIClientOptions
            {
                Endpoint = endPoint
            });
        }

        public async Task<BinaryData> GenerateImageAsync(string prompt, object options)
        {
            try
            {
                GeneratedImage image = await _client.GenerateImageAsync(prompt, (ImageGenerationOptions)options);
                BinaryData bytes = image.ImageBytes;
                return bytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API 调用失败: {ex.Message}");
                throw;
            }
        }

        public async Task<BinaryData> GenerateImageFromImageAsync(string prompt, Image[] images, object options)
        {
            try
            {

                if (images.Length == 0)
                    throw new ArgumentException("未找到任何可用的输入图片文件。");

                using var multipart = new MultipartFormDataContent();
                foreach (var image in images)
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), image.ImagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (!File.Exists(imagePath))
                    {
                        Console.WriteLine($"图片文件不存在: {imagePath}");
                        continue;
                    }
                    var stream = File.OpenRead(imagePath);
                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                    multipart.Add(streamContent, "image[]", Path.GetFileName(image.ImagePath));
                }

                multipart.Add(new StringContent(prompt ?? string.Empty), "prompt");
                multipart.Add(new StringContent("1"), "n");

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_endPoint}/images/edits");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
                request.Content = multipart;

                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Image edit API returned {(int)response.StatusCode}: {responseString}");

                using var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;

                if (!root.TryGetProperty("data", out var data) || data.GetArrayLength() == 0)
                    throw new InvalidOperationException("API 返回的数据格式不正确。");

                var b64 = data[0].GetProperty("b64_json").GetString() ?? throw new InvalidOperationException("返回的图像数据为空。");
                var binary = new BinaryData(Convert.FromBase64String(b64));
                return binary;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API 调用失败: {ex.Message}");
                throw;
            }
        }

        public object ConvertOptions(GenerateImageDto generateDto)
        {
            return new ImageGenerationOptions
            {
                Quality = generateDto.Quality?.ToLower() switch
                {
                    "hd" => GeneratedImageQuality.High,
                    "standard" => GeneratedImageQuality.Standard,
                    _ => GeneratedImageQuality.Standard
                },
                Size = GeneratedImageSize.W1024xH1024,
                Style = generateDto.Style?.ToLower() switch
                {
                    "natural" => GeneratedImageStyle.Natural,
                    "vivid" => GeneratedImageStyle.Vivid,
                    _ => GeneratedImageStyle.Vivid
                }
            };
        }
    }
}
