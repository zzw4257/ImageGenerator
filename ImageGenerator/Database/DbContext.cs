using Microsoft.EntityFrameworkCore;
using ImageGenerator.Models;

namespace ImageGenerator.Database;

public class IgDbContext(DbContextOptions<IgDbContext> options) : DbContext(options)
{
    public DbSet<User>? Users { get; set; }
    public DbSet<Invitation>? Invitations { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<GenerationRecord> GenerationRecords { get; set; }
    public DbSet<Image> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        this.AddEntityRelationship(modelBuilder);
        this.SeedData(modelBuilder);
    }
    
}
