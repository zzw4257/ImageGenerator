using System.Security.Claims;
using ImageGenerator.Database;
using ImageGenerator.Dtos;
using ImageGenerator.Enums;
using ImageGenerator.Interface;
using ImageGenerator.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using SixLabors.ImageSharp;
using SixLaborsImage = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.Processing;
using ImageGenerator.Helpers;

namespace ImageGenerator.Services;

public class ImageStorageService(IgDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper, IConfiguration configuration) : IImageStorageService
{
    private readonly IgDbContext _context = context;
    private readonly IHttpContextAccessor _http = httpContextAccessor;
    private readonly IMapper _mapper = mapper;
    private readonly IConfiguration _configuration = configuration;

    public async Task<ImageDto> UploadAsync(UploadImageDto uploadDto)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未认证");
        var file = uploadDto.File;
        if (file == null || file.Length == 0)
            throw new ArgumentException("未选择文件");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        // 先保存到内存用于读取尺寸
        using var mem = new MemoryStream();
        await file.CopyToAsync(mem);
        mem.Position = 0;

        int maxWidth = _configuration.GetValue<int?>("Upload:MaxWidth") ?? 4096;
        int maxHeight = _configuration.GetValue<int?>("Upload:MaxHeight") ?? 4096;
        int jpegQuality = _configuration.GetValue<int?>("Upload:JpegQuality") ?? 85;

        SixLaborsImage processed = await SixLaborsImage.LoadAsync(mem);

        // 缩放（保持比例）
        if (processed.Width > maxWidth || processed.Height > maxHeight)
        {
            var ratioW = (double)maxWidth / processed.Width;
            var ratioH = (double)maxHeight / processed.Height;
            var ratio = Math.Min(ratioW, ratioH);
            var newW = (int)Math.Round(processed.Width * ratio);
            var newH = (int)Math.Round(processed.Height * ratio);
            processed.Mutate(x => x.Resize(newW, newH));
        }

        var currentDir = Directory.GetCurrentDirectory();
        var saveDirectory = Path.Combine(currentDir, "images", "uploads");
        Directory.CreateDirectory(saveDirectory);

        var fileName = $"{Guid.NewGuid()}.jpg"; 
        var filePath = Path.Combine(saveDirectory, fileName);

        // 重新编码为 JPEG
        mem.Position = 0;
        long fileSize = 0;
        await using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
            {
                Quality = jpegQuality
            };
            await processed.SaveAsJpegAsync(fs, encoder);
            fileSize = fs.Length;
        }
        processed.Dispose();

        var image = new Models.Image
        {
            IsFavorite = false,
            ImagePath = Path.Combine("images", "uploads", fileName).Replace("\\", "/"),
            UserId = userId,
            Type = ImageType.Uploaded,
            Size = fileSize
        };

        _context.Images.Add(image);
        await _context.SaveChangesAsync();
        return _mapper.Map<ImageDto>(image);
    }

    public async Task<PagedList<Models.Image, ImageDto>> ListUserImagesAsync(PaginationBaseDto param)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("未认证");
        var images = _context.Images
            .Where(i => i.UserId == userId && i.Type == ImageType.Uploaded && !i.IsDeleted)
            .OrderByDescending(i => i.CreatedAt);
        return await PagedList<Models.Image, ImageDto>.CreateAsync(images.AsQueryable(), param, _mapper);
    }

    private Guid? GetCurrentUserId()
    {
        var val = _http.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        return Guid.TryParse(val, out var id) ? id : null;
    }
}
