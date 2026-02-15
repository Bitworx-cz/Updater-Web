using Microsoft.EntityFrameworkCore;
using System;
using Updater.ApiService.Database.Models;

namespace Updater.ApiService.Database;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options) { }

    public Context() { }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<UserProject> UserProjects { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Software> Softwares { get; set; }
    public DbSet<Binary> Binarys { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<DeviceActivity> DeviceActivities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // UserProjects composite key
        modelBuilder.Entity<UserProject>()
            .HasKey(up => new { up.UserId, up.ProjectId });

        // UserProjects relationships
        modelBuilder.Entity<UserProject>()
            .HasOne(up => up.User)
            .WithMany(u => u.UserProjects)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserProject>()
            .HasOne(up => up.Project)
            .WithMany(p => p.UserProjects)
            .HasForeignKey(up => up.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Group unique constraint
        modelBuilder.Entity<Group>()
            .HasIndex(g => new { g.ProjectId, g.Name })
            .IsUnique();

        // Group -> TargetSoftware relationship
        modelBuilder.Entity<Group>()
            .HasOne(g => g.TargetSoftware)
            .WithMany(s => s.GroupsUsingThisSoftware)
            .HasForeignKey(g => g.TargetSoftwareId)
            .OnDelete(DeleteBehavior.SetNull);

        // Software -> Group relationship
        modelBuilder.Entity<Software>()
            .HasOne(s => s.Group)
            .WithMany(g => g.Softwares)
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Software -> Uploader relationship
        modelBuilder.Entity<Software>()
            .HasOne(s => s.Uploader)
            .WithMany(u => u.UploadedSoftwares)
            .HasForeignKey(s => s.UploadedBy)
            .OnDelete(DeleteBehavior.SetNull);

        // Binary -> Software 1:1 relationship
        modelBuilder.Entity<Binary>()
            .HasOne(b => b.Software)
            .WithOne(s => s.Binary)
            .HasForeignKey<Binary>(b => b.SoftwareId)
            .OnDelete(DeleteBehavior.Cascade);

        // Device -> Group relationship
        modelBuilder.Entity<Device>()
            .HasOne(d => d.Group)
            .WithMany(g => g.Devices)
            .HasForeignKey(d => d.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        // Device -> CurrentSoftware relationship
        modelBuilder.Entity<Device>()
            .HasOne(d => d.CurrentSoftware)
            .WithMany(s => s.DevicesUsingThisSoftware)
            .HasForeignKey(d => d.CurrentSoftwareId)
            .OnDelete(DeleteBehavior.SetNull);

        // Device -> PendingSoftware relationship
        modelBuilder.Entity<Device>()
            .HasOne(d => d.PendingSoftware)
            .WithMany()
            .HasForeignKey(d => d.PendingSoftwareId)
            .OnDelete(DeleteBehavior.SetNull);

        // Default values for timestamps
        modelBuilder.Entity<Project>()
            .Property(p => p.CreatedAt)
            .HasDefaultValueSql("NOW()");

        modelBuilder.Entity<Project>()
            .Property(p => p.UpdatedAt)
            .HasDefaultValueSql("NOW()");

        modelBuilder.Entity<Group>()
            .Property(g => g.CreatedAt)
            .HasDefaultValueSql("NOW()");

        modelBuilder.Entity<Group>()
            .Property(g => g.UpdatedAt)
            .HasDefaultValueSql("NOW()");

        modelBuilder.Entity<Software>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("NOW()");

        modelBuilder.Entity<Device>()
            .Property(d => d.CreatedAt)
            .HasDefaultValueSql("NOW()");

        modelBuilder.Entity<Device>()
            .Property(d => d.UpdatedAt)
            .HasDefaultValueSql("NOW()");

        modelBuilder.Entity<UserProject>()
            .Property(up => up.CreatedAt)
            .HasDefaultValueSql("NOW()");

        // DeviceActivity -> Device relationship
        modelBuilder.Entity<DeviceActivity>()
            .HasOne(da => da.Device)
            .WithMany()
            .HasForeignKey(da => da.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        // DeviceActivity timestamp default
        modelBuilder.Entity<DeviceActivity>()
            .Property(da => da.Timestamp)
            .HasDefaultValueSql("NOW()");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<Context>()  // Load from User Secrets
                .Build();

            string connectionString = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
