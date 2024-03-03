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
        modelBuilder.Entity<Student>().Property(m => m.ClassGroupId).IsRequired(false);
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>()
            .HasOne<ApplicationUser>(s => s.ApplicationUser)
            .WithOne()
            .HasForeignKey<Student>(s => s.ApplicationUserId);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.ClassGroup)
            .WithMany(cg => cg.Students)
            .HasForeignKey(s => s.ClassGroupId);

        modelBuilder.Entity<Teacher>()
            .HasOne<ApplicationUser>(t => t.ApplicationUser)
            .WithOne()
            .HasForeignKey<Teacher>(t => t.ApplicationUserId);

        modelBuilder.Entity<Message>()
            .HasOne<ApplicationUser>(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne<ApplicationUser>(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ClassGroup>()
            .HasMany(c => c.Students)
            .WithOne(s => s.ClassGroup)
            .HasForeignKey(s => s.ClassGroupId);

        modelBuilder.Entity<LessonPlan>()
            .HasOne(lp => lp.Teacher)
            .WithMany(t => t.LessonPlans)
            .HasForeignKey(lp => lp.TeacherId);

        modelBuilder.Entity<LessonPlan>()
            .HasOne(lp => lp.ClassGroup)
            .WithMany(cg => cg.LessonPlans)
            .HasForeignKey(lp => lp.ClassGroupId);
    }


    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ClassGroup> ClassGroups { get; set; }
    public DbSet<LessonPlan> LessonPlans { get; set; }
}