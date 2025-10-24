using System.Security.Cryptography;
using System.Text;
using ImageGenerator.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageGenerator.Database;

/// <summary>
/// A helper class for configuring the database model.
/// </summary>
public static class ModelConfigurationHelper
{
    /// <summary>
    /// Configures the entity relationships for the database model.
    /// </summary>
    /// <param name="_">The database context.</param>
    /// <param name="modelBuilder">The model builder.</param>
    public static void AddEntityRelationship(this IgDbContext _, ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invitation>()
            .HasQueryFilter(e => !e.IsDeleted)
            .HasIndex(i => i.Code)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasQueryFilter(e => !e.IsDeleted)
            .HasMany<Invitation>()
            .WithOne(i => i.Issuer)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.InvitedBy)
            .WithMany()
            .HasForeignKey(u => u.InvitedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Conversation>()
            .HasQueryFilter(e => !e.IsDeleted)
            .HasMany(c => c.GenerationRecords)
            .WithOne(gr => gr.Conversation)
            .HasForeignKey(gr => gr.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GenerationRecord>()
            .HasQueryFilter(e => !e.IsDeleted)
            .HasMany(gr => gr.InputImages)
            .WithMany();

        modelBuilder.Entity<GenerationRecord>()
            .HasOne(gr => gr.OutputImages)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Image>()
            .HasQueryFilter(e => !e.IsDeleted);

        modelBuilder.Entity<Transaction>()
            .HasQueryFilter(e => !e.IsDeleted)
            .HasOne(t => t.Creator)
            .WithMany()
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Preset>()
            .HasQueryFilter(e => !e.IsDeleted) 
            .HasMany(p => p.GenerationRecords) // 一个 Preset 有多条 GenerationRecords
            .WithOne(gr => gr.Preset) // 每条 GenerationRecord 对应一个 Preset
            .HasForeignKey(gr => gr.PresetId) 
            .OnDelete(DeleteBehavior.SetNull); // 如果删除了一个 Preset，历史记录(Record)不应被删除，只是把 PresetId 设为 null

        modelBuilder.Entity<User>()
            .HasMany<Preset>() // 一个 User 可以创建多个 Presets
            .WithOne(p => p.CreatedByUser) // 每个 Preset 对应一个 User
            .HasForeignKey(p => p.CreatedByUserId) // 外键是 CreatedByUserId
            .OnDelete(DeleteBehavior.Restrict); // 不允许删除 User，如果 TA 还有 Presets
    }

    /// <summary>
    /// Seeds the database with initial data.
    /// </summary>
    /// <param name="_">The database context.</param>
    /// <param name="modelBuilder">The model builder.</param>
    public static void SeedData(this IgDbContext _, ModelBuilder modelBuilder)
    {
        var adminUserId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var salt = "admin-salt-123";

        // Use the same encryption method as in AuthenticationService
        var passwordBytes = Encoding.UTF8.GetBytes("admin123" + salt);
        var passwordEncryption = Convert.ToHexString(SHA256.HashData(passwordBytes));

        // Create admin user - use static datetime value
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = adminUserId,
            Username = "admin",
            Password = passwordEncryption,
            Salt = salt,
            CreatedAt = seedDate,
            IsDeleted = false,
            Credits = 100,
            LastCreditClaimedAt = seedDate
        });

        var testerUserId = Guid.Parse("10000000-0000-0000-0000-000000000002"); 
        var testerSalt = "tester-salt-456";
        var testerPasswordBytes = Encoding.UTF8.GetBytes("tester123" + testerSalt);
        var testerPasswordEncryption = Convert.ToHexString(SHA256.HashData(testerPasswordBytes));

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = testerUserId,
            Username = "tester",
            Password = testerPasswordEncryption,
            Salt = testerSalt,
            CreatedAt = seedDate,
            IsDeleted = false,
            Credits = 100, // 初始点数
            LastCreditClaimedAt = seedDate
        });

        // Create initial invitation codes
        modelBuilder.Entity<Invitation>().HasData(
            new Invitation
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                Code = "WELCOME2024ABCDE",
                IssuerId = adminUserId,
                CreatedAt = seedDate,
                RemainingUses = 10,
                IsDeleted = false
            },
            new Invitation
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                Code = "INVITE2024FGHIJK",
                IssuerId = adminUserId,
                CreatedAt = seedDate,
                RemainingUses = 5,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Conversation>().HasData(
            new Conversation
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                UserId = adminUserId,
                CreatedAt = seedDate,
                UpdatedAt = seedDate,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Preset>().HasData(
            new Preset
            {
                Id = Guid.Parse("40000000-0000-0000-0000-00000000000A"),
                Name = "产品商业摄影 (Qwen)",
                Description = "适合电商/广告用途的专业产品照片，强调光线布置、角度与核心细节。",
                CoverUrl = "/images/presets/product-shot.png",
                Prompt = "A high-resolution, studio-lit product photograph of a [product description:matte black wireless earbud case] on a [background surface/description:brushed aluminum surface with soft vignette]. The lighting is a [lighting setup:three-point softbox] to [lighting purpose:emphasize subtle curves]. The camera angle is a [angle type:slight low angle] to showcase [specific feature:charging indicator + hinge]. Ultra-realistic, with sharp focus on [key detail:texture + logo etching]. [Aspect ratio:1:1].",
                Provider = "Qwen",
                PriceCredits = 2, 
                DefaultParams = "{\"style\": \"photorealistic\", \"width\": 1024, \"height\": 1024, \"aspectRatio\": \"1:1\"}", 
                Tags = new List<string> { "product", "studio", "qwen" },
                CreatedByUserId = adminUserId,
                CreatedAt = seedDate, 
                IsDeleted = false
            },
            new Preset
            {
                Id = Guid.Parse("40000000-0000-0000-0000-00000000000B"),
                Name = "文字图形标识 (Flux)",
                Description = "用于生成包含特定文字的图形 / 标识，明确字体感受、风格与配色。",
                CoverUrl = "/images/presets/text-graphic.png", 
                Prompt = "Create a [image type:logo badge] for [brand/concept:Arctic Labs] with the text \"[text to render:POLAR AI]\" in a [font style:geometric sans-serif]. The design should be [style description:minimal, futuristic] with a [color scheme:icy blue + white gradient].",
                Provider = "Flux",
                PriceCredits = 1, 
                DefaultParams = "{\"style\": \"graphic\", \"width\": 768, \"height\": 768, \"aspectRatio\": \"1:1\"}", 
                Tags = new List<string> { "text", "logo", "graphic", "flux" },
                CreatedByUserId = adminUserId,
                CreatedAt = seedDate,
                IsDeleted = false
            },
            new Preset
            {
                Id = Guid.Parse("40000000-0000-0000-0000-00000000000C"),
                Name = "风格化贴纸 (Stub)",
                Description = "用于创建带有指定风格的贴纸 / 图标素材，强调线条、配色与透明背景。",
                CoverUrl = "/images/presets/sticker.png",
                Prompt = "A [style:kawaii chibi] sticker of a [subject:cat astronaut], featuring [key characteristics:round helmet, floating fish] and a [color palette:pastel neon mix]. The design should have [line style:clean bold outline] and [shading style:soft cell shading]. The background must be transparent.",
                Provider = "Stub",
                PriceCredits = 0, 
                DefaultParams = "{\"style\": \"sticker\", \"width\": 512, \"height\": 512, \"aspectRatio\": \"1:1\"}", 
                Tags = new List<string> { "sticker", "chibi", "icon", "stub" },
                CreatedByUserId = adminUserId,
                CreatedAt = seedDate,
                IsDeleted = false
            },
            new Preset
            {
                Id = Guid.Parse("40000000-0000-0000-0000-00000000000D"),
                Name = "逼真摄影场景 (Qwen)",
                Description = "对于逼真的图片，请使用摄影术语。提及拍摄角度、镜头类型、光线和细节，引导模型生成逼真的效果。",
                CoverUrl = "/images/presets/photorealistic.png",
                Prompt = "A photorealistic [shot type:close-up] of [subject:a mystical fox], [action or expression:looking into the distance], set in [environment:ancient forest]. The scene is illuminated by [lighting description:soft golden hour rim light], creating a [mood:serene] atmosphere. Captured with a [camera/lens details:Canon EOS R5 + 85mm f1.2], emphasizing [key textures and details:detailed fur, shimmering particles]. The image should be in a [aspect ratio:16:9] format.",
                Provider = "Qwen",
                PriceCredits = 2,
                DefaultParams = "{\"style\": \"photorealistic\", \"width\": 1024, \"height\": 576, \"aspectRatio\": \"16:9\"}",
                Tags = new List<string> { "photo", "realistic", "camera", "qwen" },
                CreatedByUserId = adminUserId,
                CreatedAt = seedDate,
                IsDeleted = false
            },
            new Preset
            {
                Id = Guid.Parse("40000000-0000-0000-0000-00000000000E"),
                Name = "极简负空间 (Flux)",
                Description = "生成带大量留白与单主体的极简风图像，适合做背景或叠加文案。",
                CoverUrl = "/images/presets/minimal.png",
                Prompt = "A minimalist composition featuring a single [subject:solitary bonsai] positioned in the [position in frame:lower right] of the frame. The background is a vast, empty [color:off-white] canvas, creating significant negative space. Soft, subtle lighting. [Aspect ratio:3:2].",
                Provider = "Flux",
                PriceCredits = 1,
                DefaultParams = "{\"style\": \"minimalist\", \"width\": 768, \"height\": 512, \"aspectRatio\": \"3:2\"}", 
                Tags = new List<string> { "minimalist", "art", "negative space", "flux" },
                CreatedByUserId = adminUserId,
                CreatedAt = seedDate,
                IsDeleted = false
            },
            new Preset
            {
                Id = Guid.Parse("40000000-0000-0000-0000-00000000000F"),
                Name = "漫画单格 (Stub)",
                Description = "生成漫画风单格场景，分离前景角色动作与背景设定，可含对白框。",
                CoverUrl = "/images/presets/comic.png",
                Prompt = "A single comic book panel in a [art style:neo-noir ink wash] style. In the foreground, [character description and action:detective leaning over a glowing map]. In the background, [setting details:rain streaked window + neon signs]. The panel has a [dialogue/caption box:caption] with the text \"[Text:We were already too late]\". The lighting creates a [mood:brooding] mood. [Aspect ratio:9:16].",
                Provider = "Stub",
                PriceCredits = 0,
                DefaultParams = "{\"style\": \"comic\", \"width\": 512, \"height\": 910, \"aspectRatio\": \"9:16\"}", 
                Tags = new List<string> { "comic", "noir", "storyboard", "stub" },
                CreatedByUserId = adminUserId,
                CreatedAt = seedDate,
                IsDeleted = false
            }
        );
    }
}