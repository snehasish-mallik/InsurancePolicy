using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace InsurancePolicy.Models
{
    public partial class InsuranceCompanyContext : DbContext
    {
        public InsuranceCompanyContext()
        {
        }

        public InsuranceCompanyContext(DbContextOptions<InsuranceCompanyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<Claim> Claims { get; set; } = null!;
        public virtual DbSet<Policy> Policies { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server= Darth-Vader\\SQLEXPRESS;Database=InsuranceCompany;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Password).HasMaxLength(255);
            });

            modelBuilder.Entity<Claim>(entity =>
            {
                entity.ToTable("Claim");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Reason).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Policy)
                    .WithMany(p => p.Claims)
                    .HasForeignKey(d => d.PolicyId)
                    .HasConstraintName("FK__Claim__PolicyId__440B1D61");
            });

            modelBuilder.Entity<Policy>(entity =>
            {
                entity.Property(e => e.PolicyName).HasMaxLength(255);

                entity.Property(e => e.PolicyType).HasMaxLength(255);

                entity.Property(e => e.VehicleRegistrationNum).HasMaxLength(255);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Policies)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Policies__UserId__398D8EEE");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Password).HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
