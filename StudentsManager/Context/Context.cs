using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Models;

namespace StudentsManager.Context;

public class Context : IdentityDbContext<ApplicationUser>
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>()
            .HasOne<ApplicationUser>(s => s.ApplicationUser)
            .WithOne()
            .HasForeignKey<Student>(s => s.ApplicationUserId);

        modelBuilder.Entity<Teacher>()
            .HasOne<ApplicationUser>(t => t.ApplicationUser)
            .WithOne()
            .HasForeignKey<Teacher>(t => t.ApplicationUserId);

        modelBuilder.Entity<Address>()
            .HasOne<Teacher>(a => a.Teacher)
            .WithMany()
            .HasForeignKey(a => a.TeacherId)
            .OnDelete(DeleteBehavior.NoAction);
    }


    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Address> Addresses { get; set; }
}