using ImageGenerator.Enums;

namespace ImageGenerator.Dtos;

public class ConversationDto: ActionBaseDto
{
    public string Title { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
    public List<GenerationRecordDto> GenerationRecords { get; set; } = [];
}

public class GenerationRecordDto: ActionBaseDto
{
    public GenerationType GenerationType { get; set; }
    public string Prompt { get; set; } = string.Empty;
    public string GenerationParams { get; set; } = string.Empty;
    public GenerationStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<ImageDto> InputImages { get; set; } = [];
    public ImageDto? OutputImage { get; set; }
}

public class ImageDto: ActionBaseDto
{
    public string ImagePath { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
    public long Size { get; set; }
}

public class UploadImageDto
{
    public IFormFile File { get; set; } = default!;
}
