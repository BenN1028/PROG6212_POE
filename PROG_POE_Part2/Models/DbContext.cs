using Microsoft.EntityFrameworkCore;

public class UserDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Claim> Claims { get; set; }

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the User entity (primary key already set implicitly)
        modelBuilder.Entity<User>().HasKey(u => u.Id);

        // Configure the Claim entity
        modelBuilder.Entity<Claim>()
            .HasKey(c => c.ClaimId); // Primary Key for Claim (this should stay as the actual primary key)

        modelBuilder.Entity<Claim>()
            .Property(c => c.HoursWorked)
            .IsRequired()
            .HasColumnType("DECIMAL(18,2)");

        modelBuilder.Entity<Claim>()
            .Property(c => c.HourlyRate)
            .IsRequired()
            .HasColumnType("DECIMAL(18,2)");

        modelBuilder.Entity<Claim>()
            .Property(c => c.TotalAmount)
            .HasComputedColumnSql("[HoursWorked] * [HourlyRate]");

        modelBuilder.Entity<Claim>()
            .Property(c => c.DateSubmitted)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Claim>()
            .Property(c => c.Status)
            .HasDefaultValue("Pending")
            .HasMaxLength(20);

        modelBuilder.Entity<Claim>()
            .Property(c => c.RejectionReason)
            .HasMaxLength(500);

        // Ensure LecturerName is treated as a regular column and not as a key
        modelBuilder.Entity<Claim>()
            .Property(c => c.LecturerName)
            .HasMaxLength(100); // You can adjust the length as needed
    }
}