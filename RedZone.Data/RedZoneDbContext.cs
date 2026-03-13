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
    }
}
