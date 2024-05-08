using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess;

public class DataModel : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Weather> Weather => Set<Weather>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySQL("server=localhost;database=weatherstation;user=root;password=1234;");
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(e => e.Id);
        modelBuilder.Entity<Weather>().HasKey(e => e.Id);
    }
}