using Microsoft.EntityFrameworkCore;

namespace Db_Setup.EfcDataAccess
{
    public class WeatherContext : DbContext
    {
        public DbSet<WeatherData> WeatherData { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=weatherstation;user=root;password=1234");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherData>().HasKey(w => w.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Id);
        }
    }
}
