using Microsoft.EntityFrameworkCore;
using RedZone.Data;

namespace RedZone.Services.Core.Tests.Helpers
{
    public static class DbContextHelper
    {
        public static RedZoneDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<RedZoneDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new RedZoneDbContext(options);
        }
    }
}