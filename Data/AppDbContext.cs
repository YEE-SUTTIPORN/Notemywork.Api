using Microsoft.EntityFrameworkCore;
using Notemywork.Api.Entities;

namespace Notemywork.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
