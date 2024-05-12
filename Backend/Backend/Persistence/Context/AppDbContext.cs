using Backend.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Persistence.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("users", "tennis_schema")
            .HasIndex(u => u.Username)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique(); ;

        modelBuilder.Entity<Tournament>().ToTable("tournaments", "tennis_schema");

        modelBuilder.Entity<TournamentParticipant>().ToTable("tournament_participants", "tennis_schema");
        modelBuilder.Entity<TournamentParticipant>()
            .HasOne(tp => tp.User)
            .WithMany(u => u.TournamentParticipants)
            .HasForeignKey(tp => tp.UserId);

        modelBuilder.Entity<TournamentParticipant>()
            .HasOne(tp => tp.Tournament)
            .WithMany(t => t.TournamentParticipants)
            .HasForeignKey(tp => tp.TournamentId);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<TournamentParticipant> TournamentParticipants { get; set; }
}
