namespace ImageGenerator.Dtos;

public class PresetDto: ActionBaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CoverUrl { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public int PriceCredits { get; set; } = 0;
    public List<string> Tags { get; set; } = [];
    public Guid CreatedByUserId { get; set; }
    public UserDto CreatedByUser { get; set; } = null!;
    public int LikeCount { get; set; } = 0;
    public int FavoriteCount { get; set; } = 0;
}

public class PresetDetailedDto: PresetDto
{
    public string Provider { get; set; } = "Stub";
    public string DefaultParams { get; set; } = "{}";
    public ICollection<PresetLikeDto> PresetLikes { get; set; } = [];
    public ICollection<PresetFavoriteDto> PresetFavorites { get; set; } = [];
}

public class PresetLikeDto: ActionBaseDto
{
    public Guid UserId { get; set; }
    public UserDto? User { get; set; }
    public Guid PresetId { get; set; }
}

public class PresetFavoriteDto: ActionBaseDto
{
    public Guid UserId { get; set; }
    public UserDto? User { get; set; }
    public Guid PresetId { get; set; }
}
