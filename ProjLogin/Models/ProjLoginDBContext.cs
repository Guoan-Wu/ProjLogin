using Microsoft.EntityFrameworkCore;

namespace ProjLogin.Models
{
    public class ProjLoginDBContext : DbContext
    {
        public ProjLoginDBContext(DbContextOptions<ProjLoginDBContext> options)
            : base(options)
        {
        }

        public DbSet<ProjLogin.Models.User> Users { get; set; } = default!;
    }
}
