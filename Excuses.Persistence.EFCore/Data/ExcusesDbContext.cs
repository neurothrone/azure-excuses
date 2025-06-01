using Excuses.Persistence.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Excuses.Persistence.EFCore.Data;

public class ExcusesDbContext(DbContextOptions<ExcusesDbContext> options) : DbContext(options)
{
    public DbSet<Excuse> Excuses => Set<Excuse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Excuse>().HasData(
            new Excuse { Id = 1, Text = "My computer exploded", Category = "work" },
            new Excuse { Id = 2, Text = "My cat hid my car keys", Category = "pets" },
            new Excuse { Id = 3, Text = "Gravity stopped working for me temporarily", Category = "general" }
        );
    }
}
