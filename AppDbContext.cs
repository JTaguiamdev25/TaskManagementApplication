using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApplication.Entities;

namespace TaskManagementApplication
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<TaskItem> Tasks { get; set; } = null!;
        public DbSet<SubTask> SubTasks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.DateOfBirth).IsRequired();
                entity.Property(u => u.SelectedPersonality).IsRequired().HasMaxLength(50);
                entity.Property(u => u.CreatedAt).IsRequired();

                entity.HasIndex(u => u.Email).IsUnique();
            });

            // TaskItem configuration
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Description).HasMaxLength(1000);
                entity.Property(t => t.DueDate).IsRequired();
                entity.Property(t => t.Priority).IsRequired();
                entity.Property(t => t.CreatedAt).IsRequired();

                entity.HasOne(t => t.User)
                      .WithMany(u => u.Tasks)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // SubTask configuration
            modelBuilder.Entity<SubTask>(entity =>
            {
                entity.HasKey(st => st.Id);
                entity.Property(st => st.Title).IsRequired().HasMaxLength(200);
                entity.Property(st => st.IsCompleted).IsRequired();

                entity.HasOne(st => st.TaskItem)
                      .WithMany(t => t.SubTasks)
                      .HasForeignKey(st => st.TaskItemId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
