using Microsoft.EntityFrameworkCore;
using WEBSockets.Domain.Models;

namespace WEBSockets.EfcDataAccess
{
    public class DatabaseContext : DbContext
    {
        public DbSet<WeatherData> WeatherData { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Notification> Notification { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server=sql.freedb.tech;Port=3306;Database=freedb_weatherstation;Uid=freedb_cristi;Pwd=vrS@Q8C95!QM!eG");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherData>().HasKey(w => w.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<Notification>().HasKey(n => n.Id);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .IsRequired();
        }
    }
}
