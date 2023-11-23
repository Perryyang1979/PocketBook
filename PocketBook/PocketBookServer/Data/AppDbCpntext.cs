using Microsoft.EntityFrameworkCore;
using PocketBookServer.Models;
using PocketUser = PocketBookServer.Models.PocketUser;

namespace PocketBookServer.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PocketTransaction> PocketTransaction =>Set<PocketTransaction>();
        public DbSet<PocketUser> Users =>Set<PocketUser>();
    }
}
