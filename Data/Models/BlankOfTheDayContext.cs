using Microsoft.EntityFrameworkCore;

namespace Data.Models;

public class BlankOfTheDayContext : DbContext
{
    public DbSet<EmotionState> Emotions { get; set; }
    public string DbPath { get; }

    public BlankOfTheDayContext(string? dbPath = null)
    {
        if (dbPath is not null)
        {
            DbPath = dbPath;
            return;
        }

        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "BlankOfTheDayContext.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}", x => x.MigrationsAssembly("BlankOfTheDayGenerator"));
}
