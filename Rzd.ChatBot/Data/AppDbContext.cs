using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Options;

namespace Rzd.ChatBot.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<UserForm> Forms { get; set; } = null!;
    private bool _isDev;
    private DbOptions _options;

    public AppDbContext(IOptions<DbOptions> options, IWebHostEnvironment _env)
    {
        _isDev = _env.IsDevelopment();
        _options = options.Value;
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            NpgsqlConnectionStringBuilder builder = new()
            {
                Database = _options.Database,
                Username = _options.User,
                Password = _options.Password,
                Host = _options.Host,
                Port = _options.Port,
                IncludeErrorDetail = _isDev,
            };
            optionsBuilder.UseNpgsql(builder.ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserForm>(entity =>
        {
            entity.HasKey(form => form.Id);
            entity.Ignore(form => form.Modified);
            entity.Property(form => form.Photos)
                .HasColumnType("varchar(256)[]")
                .HasDefaultValue(Array.Empty<string>());

            entity.Property(form => form.CreatedAt)
                .HasDefaultValue(DateTime.UtcNow);
        });
    }
}