using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DatabaseFristPostgres.GeneratedCode;

public partial class ScaffoldedDbContext : DbContext
{
    public ScaffoldedDbContext()
    {
    }

    public ScaffoldedDbContext(DbContextOptions<ScaffoldedDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Todo> Todos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=todo_db;Username=postgres;Password=your_password");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Todo>(entity =>
        {
            entity.HasIndex(e => e.CategoryId, "IX_Todos_CategoryId");

            entity.HasIndex(e => e.Completed, "IX_Todos_Completed");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CategoryId).HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'::uuid");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Text).HasMaxLength(200);

            entity.HasOne(d => d.Category).WithMany(p => p.Todos)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
