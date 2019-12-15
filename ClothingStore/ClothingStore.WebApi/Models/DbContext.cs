using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Models
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options)
        : base(options)
        { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CanceledOrder> CanceledOrders{ get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OrderPayment> OrderPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>().Property(p => p.PiecesAvailable).IsConcurrencyToken();
            //modelBuilder.Entity<Order>().Property(p => p.Total).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<OrderPayment>().Property(p => p.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<OrderItem>().Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Coupon>().Property(p => p.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Coupon>().Property(p => p.CouponIdentity);
            modelBuilder.Entity<User>().Property(p => p.CreatedDate);
            modelBuilder.Entity<Product>().Property(p => p.Updated);
        }
    }
}
