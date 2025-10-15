using Microsoft.EntityFrameworkCore;
using ImageGenerator.Models;

namespace ImageGenerator.Database;

/// <summary>
/// The database context for the Image Generator application.
/// </summary>
public class IgDbContext(DbContextOptions<IgDbContext> options) : DbContext(options)
{
    /// <summary>
    /// The Users table.
    /// </summary>
    public DbSet<User>? Users { get; set; }

    /// <summary>
    /// The Invitations table.
    /// </summary>
    public DbSet<Invitation>? Invitations { get; set; }

    /// <summary>
    /// The Conversations table.
    /// </summary>
    public DbSet<Conversation> Conversations { get; set; }

    /// <summary>
    /// The GenerationRecords table.
    /// </summary>
    public DbSet<GenerationRecord> GenerationRecords { get; set; }

    /// <summary>
    /// The Images table.
    /// </summary>
    public DbSet<Image> Images { get; set; }

    /// <summary>
    /// Configures the model for the database context.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        this.AddEntityRelationship(modelBuilder);
        this.SeedData(modelBuilder);
    }
    
}
