using Introduction.Models;
using Microsoft.EntityFrameworkCore;

namespace Introduction.Service
{
    public class PustakaDbContext : DbContext
    {
        public PustakaDbContext(DbContextOptions<PustakaDbContext> options) : base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
    }
}
