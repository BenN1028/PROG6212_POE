using Microsoft.EntityFrameworkCore;

public class UserDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // If needed, define additional configurations like primary keys or table names
        modelBuilder.Entity<User>().HasKey(u => u.Id); // Explicitly set the primary key if necessary
    }
}
