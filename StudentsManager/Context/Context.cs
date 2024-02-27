using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Models;

namespace StudentsManager.Context;

public class Context : IdentityDbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>()
            .HasKey(p => p.StudentId);

        modelBuilder.Entity<Teacher>()
            .HasKey(v => v.TeacherId);

        modelBuilder.Entity<Teacher>()
            .HasMany(v => v.Students)
            .WithMany(p => p.Teachers);

        modelBuilder.Entity<Student>()
            .HasOne(v => v.Address)
            .WithOne(p => p.Student)
            .HasForeignKey<Address>(v => v.AddressId);

        modelBuilder.Entity<Teacher>()
            .HasOne(v => v.Address)
            .WithOne(p => p.Teacher)
            .HasForeignKey<Address>(v => v.AddressId);
        ;

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
}