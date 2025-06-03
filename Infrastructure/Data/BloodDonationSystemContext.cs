using System;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public partial class BloodDonationSystemContext : DbContext
{
    public BloodDonationSystemContext()
    {
    }

    public BloodDonationSystemContext(DbContextOptions<BloodDonationSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BloodComponent> BloodComponents { get; set; }

    public virtual DbSet<BloodType> BloodTypes { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<DonationHistory> DonationHistories { get; set; }

    public virtual DbSet<DonationProcessStep> DonationProcessSteps { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventBloodType> EventBloodTypes { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=(local);Database= BloodDonationSystem;UID=sa;PWD=12345;TrustServerCertificate=True");

    private string? GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DBDefault"];
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Blog__3214EC07ED70BE43");

            entity.ToTable("Blog");

            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.LastUpdate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Author).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Blog__AuthorId__59FA5E80");
        });

        modelBuilder.Entity<BloodComponent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BloodCom__3214EC07F6B8765D");

            entity.ToTable("BloodComponent");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Type)
                .HasMaxLength(1)
                .IsUnicode(false);
        });

        modelBuilder.Entity<BloodType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BloodTyp__3214EC07A7D94D0D");

            entity.ToTable("BloodType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasMany(d => d.DonorTypes).WithMany(p => p.RecipientTypes)
                .UsingEntity<Dictionary<string, object>>(
                    "BloodTypeCompatibility",
                    r => r.HasOne<BloodType>().WithMany()
                        .HasForeignKey("DonorTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BloodType__Donor__04E4BC85"),
                    l => l.HasOne<BloodType>().WithMany()
                        .HasForeignKey("RecipientTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BloodType__Recip__05D8E0BE"),
                    j =>
                    {
                        j.HasKey("DonorTypeId", "RecipientTypeId").HasName("PK__BloodTyp__46F3645F81D32F32");
                        j.ToTable("BloodTypeCompatibility");
                    });

            entity.HasMany(d => d.RecipientTypes).WithMany(p => p.DonorTypes)
                .UsingEntity<Dictionary<string, object>>(
                    "BloodTypeCompatibility",
                    r => r.HasOne<BloodType>().WithMany()
                        .HasForeignKey("RecipientTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BloodType__Recip__05D8E0BE"),
                    l => l.HasOne<BloodType>().WithMany()
                        .HasForeignKey("DonorTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BloodType__Donor__04E4BC85"),
                    j =>
                    {
                        j.HasKey("DonorTypeId", "RecipientTypeId").HasName("PK__BloodTyp__46F3645F81D32F32");
                        j.ToTable("BloodTypeCompatibility");
                    });
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comment__3214EC07440588F4");

            entity.ToTable("Comment");

            entity.HasOne(d => d.Blog).WithMany(p => p.Comments)
                .HasForeignKey(d => d.BlogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__BlogId__5DCAEF64");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__UserId__5CD6CB2B");
        });

        modelBuilder.Entity<DonationHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Donation__3214EC07B869F1AC");

            entity.ToTable("DonationHistory");

            entity.HasIndex(e => e.RegistrationId, "UQ__Donation__6EF5881195A04153").IsUnique();

            entity.Property(e => e.CreateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Registration).WithOne(p => p.DonationHistory)
                .HasForeignKey<DonationHistory>(d => d.RegistrationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationH__Regis__4D94879B");
        });

        modelBuilder.Entity<DonationProcessStep>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Donation__3214EC07FC7398DE");

            entity.ToTable("DonationProcessStep");

            entity.Property(e => e.PerformedAt).HasColumnType("datetime");
            entity.Property(e => e.StepName).HasMaxLength(50);

            entity.HasOne(d => d.DonationHistory).WithMany(p => p.DonationProcessSteps)
                .HasForeignKey(d => d.DonationHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationP__Donat__5629CD9C");

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.DonationProcessSteps)
                .HasForeignKey(d => d.PerformedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationP__Perfo__571DF1D5");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Event__3214EC0796EBFD3A");

            entity.ToTable("Event");

            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Events)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Event__CreateBy__4222D4EF");
        });

        modelBuilder.Entity<EventBloodType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EventBlo__3214EC07F4FACCC4");

            entity.ToTable("EventBloodType");

            entity.HasOne(d => d.BloodType).WithMany(p => p.EventBloodTypes)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventBloo__Blood__44FF419A");

            entity.HasOne(d => d.Event).WithMany(p => p.EventBloodTypes)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventBloo__Event__45F365D3");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Inventor__3214EC07769C8F13");

            entity.ToTable("Inventory");

            entity.HasIndex(e => e.DonationHistoryId, "UQ__Inventor__A1E5FD52F3644961").IsUnique();

            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.ExpiredDate).HasColumnType("datetime");

            entity.HasOne(d => d.BloodComponent).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.BloodComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Blood__52593CB8");

            entity.HasOne(d => d.BloodType).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Blood__5165187F");

            entity.HasOne(d => d.DonationHistory).WithOne(p => p.Inventory)
                .HasForeignKey<Inventory>(d => d.DonationHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Donat__534D60F1");
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Registra__3214EC0746698560");

            entity.ToTable("Registration");

            entity.Property(e => e.CreateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Event).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Registrat__Event__49C3F6B7");

            entity.HasOne(d => d.User).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Registrat__UserI__48CFD27E");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC0702CBD23D");

            entity.ToTable("User");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(1);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.HashPass).IsUnicode(false);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Role).HasMaxLength(50);

            entity.HasOne(d => d.BloodType).WithMany(p => p.Users)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__BloodTypeI__3F466844");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
