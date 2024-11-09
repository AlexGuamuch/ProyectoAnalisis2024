using Microsoft.EntityFrameworkCore;

namespace SistemaSeguridad
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
