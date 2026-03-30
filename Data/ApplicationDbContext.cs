using Microsoft.EntityFrameworkCore;
using ItemProcessingSystem.Models;

namespace ItemProcessingSystem.Data
{
    /// <summary>
    /// Application database context - the main bridge between EF Core and SQL Server.
    /// Registers all entity models and configures relationships.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        // Constructor receives options (like connection string) from dependency injection
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==========================================
        // DbSet properties - each maps to a table
        // ==========================================

        /// <summary>
        /// Users table - stores login credentials
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Items table - stores items with self-referencing hierarchy
        /// </summary>
        public DbSet<Item> Items { get; set; }

        /// <summary>
        /// Configure entity relationships and constraints using Fluent API.
        /// This method runs when the database model is being created.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- User configuration ----
            // Ensure email addresses are unique across all users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // ---- Item self-referencing relationship ----
            // Configure the parent-child relationship:
            // One parent item can have many child items.
            // Deleting a parent will restrict (not cascade) to prevent accidental data loss.
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Parent)                  // Each item has one optional parent
                .WithMany(i => i.ChildItems)            // A parent can have many children
                .HasForeignKey(i => i.ParentId)         // ParentId is the foreign key
                .OnDelete(DeleteBehavior.Restrict);     // Prevent cascade delete

            // Set decimal precision for Weight column (18 digits total, 2 decimal places)
            modelBuilder.Entity<Item>()
                .Property(i => i.Weight)
                .HasColumnType("decimal(18,2)");
        }
    }
}
