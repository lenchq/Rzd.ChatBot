using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Options;

namespace Rzd.ChatBot.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<UserForm> Forms { get; set; } = null!;
    public DbSet<UserLike> Likes { get; set; } = null!;
    
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
        #region configure DB connection
        
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
        #endregion

        if (_isDev)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
        // optionsBuilder.UseTriggers(configure =>
        //     configure.AddTrigger<>())
        
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
                .HasDefaultValueSql("NOW()");

            // entity.Property(form => form.Disabled)
            //     .HasDefaultValue(true);
        });

        modelBuilder.Entity<UserLike>(entity =>
        {
            entity.HasKey(like => like.Id);
            entity.Property(like => like.Id)
                .ValueGeneratedOnAdd();

        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is UserForm && x.State is EntityState.Added or EntityState.Modified);

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow; // current datetime

            // if (entity.State == EntityState.Added)
            // {
            //     ((UserForm)entity.Entity).CreatedAt = now;
            // }
            ((UserForm)entity.Entity).UpdatedAt = now;
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    public void DetachForm(long formId)
    {
        var entity = this.ChangeTracker
            .Entries()
            .Single(x => (x.Entity as UserForm)?.Id == formId);
        entity.State = EntityState.Detached;
    }
}