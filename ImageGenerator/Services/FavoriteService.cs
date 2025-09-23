using ImageGenerator.Interface;
using ImageGenerator.Dtos;
using ImageGenerator.Models;
using ImageGenerator.Database;
using ImageGenerator.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using OpenAI.Images;


namespace ImageGenerator.Services;

public class FavoriteService(IgDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper, ImageGenerationClientFactory clientFactory) : IFavoriteService
{
    private readonly IgDbContext _context = context;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IMapper _mapper = mapper;
    private readonly ImageGenerationClientFactory _clientFactory = clientFactory;

    public async Task AddToFavoritesAsync(Guid imageId)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("User not authenticated.");

        var image = await _context.Images!
            .FirstOrDefaultAsync(img => img.Id == imageId && img.UserId == userId) 
            ?? throw new InvalidOperationException("Image not found or does not belong to the user.");

        if (image.IsFavorite)
            throw new InvalidOperationException("Image is already in favorites.");

        image.IsFavorite = true;
        await _context.SaveChangesAsync();
    }

    public async Task RemoveFromFavoritesAsync(Guid imageId)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("User not authenticated.");

        var image = await _context.Images!
            .FirstOrDefaultAsync(img => img.Id == imageId && img.UserId == userId) 
            ?? throw new InvalidOperationException("Image not found or does not belong to the user.");

        if (!image.IsFavorite)
            throw new InvalidOperationException("Image is not in favorites.");

        image.IsFavorite = false;
        await _context.SaveChangesAsync();
    }

    public async Task<List<ImageDto>> GetFavoriteImagesAsync()
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("User not authenticated.");

        var favoriteImages = await _context.Images!
            .Where(img => img.UserId == userId && img.IsFavorite && !img.IsDeleted)
            .ToListAsync();

        return _mapper.Map<List<ImageDto>>(favoriteImages);
    }
    
    private Guid? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return null;
    }
}
