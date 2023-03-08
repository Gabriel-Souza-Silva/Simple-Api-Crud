using Microsoft.EntityFrameworkCore;
using SimpleCrud.Models;

namespace SimpleCrud.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
    }
}
