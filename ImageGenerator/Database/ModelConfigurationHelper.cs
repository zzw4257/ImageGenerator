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
    }

    /// <summary>
    /// Seeds the database with initial data.
    /// </summary>
    /// <param name="_">The database context.</param>
    /// <param name="modelBuilder">The model builder.</param>
    public static void SeedData(this IgDbContext _, ModelBuilder modelBuilder)
    {
        var adminUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
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

        // Create initial invitation codes
        modelBuilder.Entity<Invitation>().HasData(
            new Invitation
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Code = "WELCOME2024ABCDE",
                IssuerId = adminUserId,
                CreatedAt = seedDate,
                RemainingUses = 10,
                IsDeleted = false
            },
            new Invitation
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
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
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                UserId = adminUserId,
                CreatedAt = seedDate,
                UpdatedAt = seedDate,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Preset>().HasData(
            new Preset
            {
                Id = Guid.Parse("00000000-0000-0000-0000-00000000000A"),
                Name = "产品商业摄影 (Qwen)",
                CoverUrl = "/images/presets/product-shot.png",
                Prompt = "A high-resolution, studio-lit product photograph of a [product description] on a [background surface]. The lighting is a [lighting setup] to emphasize subtle curves. Ultra-realistic, 4k.",
                Provider = "Qwen",
                PriceCredits = 2, // 规格 E 节
                DefaultParams = "{\"style\": \"cinematic\", \"width\": 1024, \"height\": 1024}", // JSON 字符串
                Tags = new List<string> { "product", "cinematic", "qwen" },
                CreatedAt = seedDate, // 重用已定义的 seedDate
                IsDeleted = false
            },
            new Preset
            {
                Id = Guid.Parse("00000000-0000-0000-0000-00000000000B"),
                Name = "霓虹风格 (Flux)",
                CoverUrl = "/images/presets/neon-shoe.png",
                Prompt = "a neon-lit product photograph of a sneaker on glossy floor, cinematic lighting, high contrast, 4k, [subject]",
                Provider = "Flux",
                PriceCredits = 1, // 规格 E 节
                DefaultParams = "{\"aspectRatio\": \"3:2\", \"width\": 768, \"height\": 512}", // JSON 字符串
                Tags = new List<string> { "product", "neon", "flux" },
                CreatedAt = seedDate,
                IsDeleted = false
            },
            new Preset
            {
                Id = Guid.Parse("00000000-0000-0000-0000-00000000000C"),
                Name = "风格化贴纸 (Stub)",
                CoverUrl = "/images/presets/sticker.png",
                Prompt = "A kawaii chibi sticker of a [subject], clean bold outline, soft cell shading, transparent background.",
                Provider = "Stub",
                PriceCredits = 0, // 规格 E 节
                DefaultParams = "{\"style\": \"sticker\", \"width\": 512, \"height\": 512}", // JSON 字符串
                Tags = new List<string> { "sticker", "chibi", "stub" },
                CreatedAt = seedDate,
                IsDeleted = false
            },
            new Preset
            {
                Id = Guid.Parse("00000000-0000-0000-0000-00000000000D"),
                Name = "逼真摄影 (Qwen)",
                CoverUrl = "/images/presets/photorealistic.png",
                Prompt = "A photorealistic close-up of [subject], set in [environment]. The scene is illuminated by [lighting description], creating a serene atmosphere. Captured with a Canon EOS R5.",
                Provider = "Qwen",
                PriceCredits = 2,
                DefaultParams = "{\"aspectRatio\": \"16:9\", \"width\": 1024, \"height\": 576}",
                Tags = new List<string> { "photo", "realistic", "qwen" },
                CreatedAt = seedDate,
                IsDeleted = false
            },
            new Preset
            {
                Id = Guid.Parse("00000000-0000-0000-0000-00000000000E"),
                Name = "极简负空间 (Flux)",
                CoverUrl = "/images/presets/minimal.png",
                Prompt = "A minimalist composition featuring a single [subject] positioned in the lower right. The background is a vast, empty off-white canvas, creating significant negative space.",
                Provider = "Flux",
                PriceCredits = 1,
                DefaultParams = "{\"aspectRatio\": \"3:2\", \"width\": 768, \"height\": 512}",
                Tags = new List<string> { "minimalist", "art", "flux" },
                CreatedAt = seedDate,
                IsDeleted = false
            },
            new Preset
            {
                Id = Guid.Parse("00000000-0000-0000-0000-00000000000F"),
                Name = "漫画单格 (Stub)",
                CoverUrl = "/images/presets/comic.png",
                Prompt = "A single comic book panel in a neo-noir ink wash style. In the foreground, [character description]. In the background, [setting details].",
                Provider = "Stub",
                PriceCredits = 0,
                DefaultParams = "{\"style\": \"comic\", \"width\": 512, \"height\": 768}",
                Tags = new List<string> { "comic", "noir", "stub" },
                CreatedAt = seedDate,
                IsDeleted = false
            }
        );
    }
}