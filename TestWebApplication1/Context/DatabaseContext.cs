using Microsoft.EntityFrameworkCore;
using TestWebApplication1.Models;

namespace TestWebApplication1.Context
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> UsersContext { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer("Data Source=.;Initial Catalog=TestWebApplicationDb;Integrated Security=True");

    }
}