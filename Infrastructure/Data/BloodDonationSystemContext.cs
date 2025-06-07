using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public class BloodDonationSystemContext : DbContext
    {
        public BloodDonationSystemContext(DbContextOptions<BloodDonationSystemContext> options)
            : base(options)
        {
        }
        public DbSet<BloodType> BloodTypes { get; set; }
        public DbSet<BloodComponent> BloodComponents { get; set; }
        public DbSet<BloodTypeCompatibility> BloodTypeCompatibilities { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<DonationHistory> DonationHistories { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<BloodProcedure> BloodProcedures { get; set; }
        public DbSet<HealthProcedure> HealthProcedures { get; set; }
        public DbSet<CollectionProcedure> CollectionProcedures { get; set; }
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

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.DonationHistory)
                .WithOne(d => d.Registration)
                .HasForeignKey<DonationHistory>(d => d.RegistrationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DonationHistory>()
                .HasOne(dh => dh.Inventory)
                .WithOne(i => i.DonationHistory)
                .HasForeignKey<Inventory>(i => i.DonationHistoryId)
                .OnDelete(DeleteBehavior.Restrict);


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

            modelBuilder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Registrations)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.BloodProcedures)
                .WithOne(u => u.Performer)
                .HasForeignKey(u => u.PerformedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CollectionProcedures)
                .WithOne(u => u.PerformedByUser)
                .HasForeignKey(u => u.PerformedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.HealthProcedures)
                .WithOne(u => u.Performer)
                .HasForeignKey(u => u.PerformedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.EventsCreated)
                .WithOne(u => u.Creator)
                .HasForeignKey(u => u.CreateBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
