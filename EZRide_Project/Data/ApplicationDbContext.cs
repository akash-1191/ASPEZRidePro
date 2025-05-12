using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace EZRide_Project.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly DbContextOptions<ApplicationDbContext> options;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            this.options = options;
        }

        // DbSet properties for all entities
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<OwnerPayment> OwnerPayments { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<SecurityDeposit> SecurityDeposits { get; set; }
        public DbSet<CustomerDocument> CustomerDocuments { get; set; }
        public DbSet<PricingRule> PricingRules { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<FuelLog> FuelLogs { get; set; }
        public DbSet<DamageReport> DamageReports { get; set; }
        public DbSet<VehicleImage> VehicleImages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Role>().HasData(
                      new Role { RoleId = 1, RoleName = Role.Rolename.Admin, Description = "System Administrator" },
                      new Role { RoleId = 2, RoleName = Role.Rolename.OwnerVehicle, Description = "Normal User" },
                      new Role { RoleId = 3, RoleName = Role.Rolename.Customer, Description = "Vehicle Owner" }
                      );
    

            modelBuilder.Entity<User>()
                .HasIndex(un => un.Email)
                .IsUnique();

            // Configure Role -> User (One-to-Many)
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User -> Vehicle (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Vehicles)
                .WithOne(v => v.User)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Configure User -> Booking (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Change to Restrict

            // Configure User -> OwnerPayment (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.OwnerPayments)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure User -> Conversation (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Conversations)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

          

            // Configure User -> CustomerDocument (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.CustomerDocuments)
                .WithOne(d => d.User)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User -> Feedback (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Feedbacks)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User -> Contact (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Contacts)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Vehicle -> Booking (One-to-Many)
            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.Bookings)
                .WithOne(b => b.Vehicle)
                .HasForeignKey(b => b.VehicleId)
                .OnDelete(DeleteBehavior.NoAction); // Change to Restrict

            // Configure Vehicle -> PricingRule (One-to-One)
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.PricingRule)
                .WithOne(p => p.Vehicle)
                .HasForeignKey<PricingRule>(p => p.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Vehicle -> OwnerPayment (One-to-Many)
            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.OwnerPayments)
                .WithOne(o => o.Vehicle)
                .HasForeignKey(o => o.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Vehicle -> VehicleImages (One-to-Many)
            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.VehicleImages)
                .WithOne(vi => vi.Vehicle)
                .HasForeignKey(vi => vi.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Booking -> Payment (One-to-One)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Payment)
                .WithOne(p => p.Booking)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Booking -> SecurityDeposit (One-to-One)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.SecurityDeposit)
                .WithOne(s => s.Booking)
                .HasForeignKey<SecurityDeposit>(s => s.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Booking -> FuelLog (One-to-One)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.FuelLog)
                .WithOne(f => f.Booking)
                .HasForeignKey<FuelLog>(f => f.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Booking -> DamageReport (One-to-One)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.DamageReport)
                .WithOne(d => d.Booking)
                .HasForeignKey<DamageReport>(d => d.BookingId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}