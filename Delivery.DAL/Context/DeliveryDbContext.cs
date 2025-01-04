using Delivery.DAL.Models;
using Delivery.Models;
using Microsoft.EntityFrameworkCore;

namespace Delivery.DAL.Context
{
    public class DeliveryDbContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Courier> Couriers { get; set; }
        public DbSet<CourierStatus> CourierStatuses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<CancelReason> CancelReasons { get; set; }

        public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasOne(o => o.FromAddress).WithMany(a => a.FromOrders)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Order>().HasOne(o => o.TargetAddress).WithMany(a => a.TargetOrders)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Client>().Property(s => s.Id).HasDefaultValueSql("newid()");
            modelBuilder.Entity<Order>().Property(s => s.Id).HasDefaultValueSql("newid()");
            modelBuilder.Entity<Address>().Property(s => s.Id).HasDefaultValueSql("newid()");
            modelBuilder.Entity<Courier>().Property(s => s.Id).HasDefaultValueSql("newid()");
            modelBuilder.Entity<CourierStatus>().Property(s => s.Id).HasDefaultValueSql("newid()");
            modelBuilder.Entity<OrderStatus>().Property(s => s.Id).HasDefaultValueSql("newid()");
            modelBuilder.Entity<OrderLine>().Property(s => s.Id).HasDefaultValueSql("newid()");
            modelBuilder.Entity<OrderLine>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Courier>().HasQueryFilter(x => !x.IsDeleted);
			modelBuilder.Entity<Order>().HasQueryFilter(x => !x.IsDeleted);
			modelBuilder.Entity<Client>().HasQueryFilter(x => !x.IsDeleted);
			modelBuilder.Entity<Address>().HasQueryFilter(x => !x.IsDeleted);
			modelBuilder.Entity<Order>()
                .HasOne(e => e.CancelReason)
                .WithOne(e => e.Order)
                .HasForeignKey<CancelReason>(e => e.OrderId)
                .IsRequired(false);
            modelBuilder.Entity<CancelReason>().Property(s => s.Id).HasDefaultValueSql("newid()");
        }
    }
}
