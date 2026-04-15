using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RedZone.Data.Models.Entities;

namespace RedZone.Data
{
    public class RedZoneDbContext : IdentityDbContext
    {
        public RedZoneDbContext(DbContextOptions<RedZoneDbContext> options)
            : base(options)
        {
        }

        public DbSet<Competition> Competitions { get; set; } = null!;
        public DbSet<Match> Matches { get; set; } = null!;
        public DbSet<Prediction> Predictions { get; set; } = null!;
        public DbSet<MatchResult> MatchResults { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<MatchResult>()
                .HasOne(mr => mr.Match)
                .WithOne(m => m.Result)
                .HasForeignKey<MatchResult>(mr => mr.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.Match)
                .WithMany(m => m.Comments)
                .HasForeignKey(c => c.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Prediction>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}