using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RedZone.Data
{
    public class RedZoneDbContext(DbContextOptions<RedZoneDbContext> options) : IdentityDbContext(options)
    {
    }
}
