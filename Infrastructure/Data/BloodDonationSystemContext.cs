using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public class BloodDonationSystemContext : DbContext
    {
        //public BloodDonationSystemContext(DbContextOptions<BloodDonationSystemContext> options)
        //    : base(options)
        //{

        //}

        public DbSet<BloodType> BloodTypes { get; set; }
        public DbSet<BloodCompatibility> BloodCompatibilities { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<BloodRegistration> BloodRegistrations { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<HealthProcedure> HealthProcedures { get; set; }
        public DbSet<BloodProcedure> BloodProcedures { get; set; }
        public DbSet<BloodInventory> BloodInventories { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                //Note: Remove this to migrationdotnet
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../BloodDonationSystem/"))
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection")!;

            optionsBuilder
                .UseSqlServer(connectionString)
                .LogTo(Console.WriteLine, LogLevel.Information);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BloodCompatibility>()
                .HasKey(bc => new { bc.Id, bc.BloodTypeId, bc.DonorTypeId, bc.RecipientTypeId });

            modelBuilder.Entity<User>()
                .Property(u => u.Latitude)
                .HasColumnType("decimal(9,6)");

            modelBuilder.Entity<User>()
                .Property(u => u.Longitude)
                .HasColumnType("decimal(9,6)");

            modelBuilder.Entity<Facility>()
                .Property(f => f.Latitude)
                .HasColumnType("decimal(9,6)");

            modelBuilder.Entity<Facility>()
                .Property(f => f.Longitude)
                .HasColumnType("decimal(9,6)");

            modelBuilder.Entity<BloodRegistration>()
                .HasOne(br => br.Volunteer)
                .WithOne(v => v.BloodRegistration)
                .HasForeignKey<BloodRegistration>(br => br.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BloodRegistration>()
                .HasOne(br => br.HealthProcedure)
                .WithOne(hp => hp.BloodRegistration)
                .HasForeignKey<BloodRegistration>(br => br.HealthId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BloodRegistration>()
                .HasOne(br => br.BloodProcedure)
                .WithOne(bp => bp.BloodRegistration)
                .HasForeignKey<BloodRegistration>(br => br.BloodProcedureId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BloodRegistration>()
                .HasOne(br => br.BloodInventory)
                .WithOne(bi => bi.BloodRegistration)
                .HasForeignKey<BloodInventory>(bi => bi.RegistrationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure BloodType relationships
            modelBuilder.Entity<BloodType>()
                .HasMany(b => b.Donors)
                .WithOne(b => b.DonorType)
                .HasForeignKey(b => b.DonorTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BloodType>()
                .HasMany(b => b.Recipients)
                .WithOne(b => b.RecipientType)
                .HasForeignKey(b => b.RecipientTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure User relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.BloodType)
                .WithMany(bt => bt.Users)
                .HasForeignKey(u => u.BloodTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UpdatedByUser)
                .WithMany()
                .HasForeignKey(u => u.UpdateBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Event relationships
            modelBuilder.Entity<Event>()
                .HasOne(e => e.BloodType)
                .WithMany(bt => bt.Events)
                .HasForeignKey(e => e.BloodTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Creator)
                .WithMany(u => u.CreatedEvents)
                .HasForeignKey(e => e.CreateBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Updater)
                .WithMany(u => u.UpdatedEvents)
                .HasForeignKey(e => e.UpdateBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure BloodProcedure relationships
            modelBuilder.Entity<BloodProcedure>()
                .HasOne(bp => bp.BloodType)
                .WithMany(bt => bt.BloodProcedures)
                .HasForeignKey(bp => bp.BloodTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BloodProcedure>()
                .HasOne(bp => bp.PerformedByUser)
                .WithMany(u => u.BloodProcedures)
                .HasForeignKey(bp => bp.PerformedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure BloodInventory relationships
            modelBuilder.Entity<BloodInventory>()
                .HasOne(bi => bi.BloodType)
                .WithMany(bt => bt.BloodInventories)
                .HasForeignKey(bi => bi.BloodTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BloodInventory>()
                .HasOne(bi => bi.RemovedByUser)
                .WithMany(u => u.BloodInventories)
                .HasForeignKey(bi => bi.RemoveBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure BloodRegistration relationships
            modelBuilder.Entity<BloodRegistration>()
                .HasOne(br => br.Staff)
                .WithMany(u => u.StaffRegistrations)
                .HasForeignKey(br => br.StaffId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BloodRegistration>()
                .HasOne(br => br.Member)
                .WithMany(u => u.MemberRegistrations)
                .HasForeignKey(br => br.MemberId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Comment relationships
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Staff)
                .WithMany(u => u.StaffComments)
                .HasForeignKey(c => c.StaffId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Member)
                .WithMany(u => u.MemberComments)
                .HasForeignKey(c => c.MemberId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure HealthProcedure relationships
            modelBuilder.Entity<HealthProcedure>()
                .HasOne(hp => hp.PerformedByUser)
                .WithMany(u => u.HealthProcedures)
                .HasForeignKey(hp => hp.PerformedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Blog relationships
            modelBuilder.Entity<Blog>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Blogs)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Volunteer relationships
            modelBuilder.Entity<Volunteer>()
                .HasOne(v => v.Member)
                .WithMany(u => u.Volunteers)
                .HasForeignKey(v => v.MemberId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
