using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Domain.Models;
using StudentsManager.Models;

namespace StudentsManager.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().Property(m => m.ClassGroupId).IsRequired(false);
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>()
            .HasOne<ApplicationUser>(s => s.ApplicationUser)
            .WithOne()
            .HasForeignKey<Student>(s => s.ApplicationUserId);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.ClassGroup)
            .WithMany(cg => cg.Students)
            .HasForeignKey(s => s.ClassGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Teacher>()
            .HasOne<ApplicationUser>(t => t.ApplicationUser)
            .WithOne()
            .HasForeignKey<Teacher>(t => t.ApplicationUserId);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<ClassGroup>()
            .HasMany(c => c.Students)
            .WithOne(s => s.ClassGroup)
            .HasForeignKey(s => s.ClassGroupId);

        modelBuilder.Entity<LessonPlan>()
            .HasOne(lp => lp.ClassGroup)
            .WithMany(cg => cg.LessonPlans)
            .HasForeignKey(lp => lp.ClassGroupId);

        modelBuilder.Entity<Teacher>()
            .HasMany(t => t.ClassGroups)
            .WithOne(cg => cg.Teacher)
            .HasForeignKey(cg => cg.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ClassGroup> ClassGroups { get; set; }
    public DbSet<LessonPlan> LessonPlans { get; set; }
}