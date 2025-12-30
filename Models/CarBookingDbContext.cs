using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CarBooking.Models;

public partial class CarBookingDbContext : DbContext
{
    public CarBookingDbContext()
    {
    }

    public CarBookingDbContext(DbContextOptions<CarBookingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CarBookingDetail> CarBookingDetails { get; set; }

    public virtual DbSet<CarDetail> CarDetails { get; set; }

    public virtual DbSet<CarImage> CarImages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-G7RVM8H;Database=CarBooking;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CarBookingDetail>(entity =>
        {
            entity.HasKey(e => e.CarBookingId);

            entity.ToTable("CarBookingDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Car).WithMany(p => p.CarBookingDetails)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CarBookingDetail_CarDetail");

            entity.HasOne(d => d.User).WithMany(p => p.CarBookingDetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CarBookingDetail_User");
        });

        modelBuilder.Entity<CarDetail>(entity =>
        {
            entity.HasKey(e => e.CarId);

            entity.ToTable("CarDetail");

            entity.Property(e => e.CarName)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.CarNumber).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InsuranceDate)
                .HasColumnType("datetime")
                .HasColumnName("Insurance Date");
            entity.Property(e => e.PricePerDay).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PucexpireDate)
                .HasColumnType("datetime")
                .HasColumnName("PUCExpireDate");
            entity.Property(e => e.PucimageOrignalFileName).HasColumnName("PUCImageOrignalFileName");
            entity.Property(e => e.PucimagePath).HasColumnName("PUCImagePath");
            entity.Property(e => e.RcimageOrignalFileName).HasColumnName("RCImageOrignalFileName");
            entity.Property(e => e.RcimagePath).HasColumnName("RCImagePath");
            entity.Property(e => e.RegisterDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CarImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__CarImage__7516F70CF21242A6");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Car).WithMany(p => p.CarImages)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CarImages_CarDetail");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.OtpExpireTime).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.PhoneNo).HasMaxLength(50);
            entity.Property(e => e.ResetOtp).HasMaxLength(10);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.TwoFactorSecret).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updatedDate");
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
