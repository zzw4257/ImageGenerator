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
    }
}