using ImageGenerator.Enums;

namespace ImageGenerator.Models;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User: ModelBase
{
    /// <summary>
    /// The user's username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The user's hashed password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The salt used for password hashing.
    /// </summary>
    public string? Salt { get; set; }

    /// <summary>
    /// The user's role in the system.
    /// </summary>
    public UserRole Role { get; set; } = UserRole.User;

    /// <summary>
    /// The invitation that was used to register this user.
    /// </summary>
    public Invitation? InvitedBy { get; set; }

    /// <summary>
    /// The ID of the invitation that was used to register this user.
    /// </summary>
    public Guid? InvitedById { get; set; }

    /// <summary>
    /// The number of credits the user has for image generation.
    /// </summary>
    public decimal Credits { get; set; } = 0;

    /// <summary>
    /// The timestamp when the user last claimed their credits.
    /// </summary>
    public DateTime? LastCreditClaimedAt { get; set; }

    /// <summary>
    /// 该用户创建的所有预制菜。
    /// </summary>
    public ICollection<Preset> PresetsCreated { get; set; } = [];

    /// <summary>
    /// 该用户的"点赞"记录。
    /// </summary>
    public ICollection<PresetLike> PresetLikes { get; set; } = [];

    /// <summary>
    /// 该用户的"收藏"记录。
    /// </summary>
    public ICollection<PresetFavorite> PresetFavorites { get; set; } = [];

    /// <summary>
    /// 该用户提交的所有"举报"记录。
    /// </summary>
    public ICollection<PresetReport> PresetReportsMade { get; set; } = [];

    /// <summary>
    /// 该用户的所有对话。
    /// </summary>
    public ICollection<Conversation> Conversations { get; set; } = [];
}
