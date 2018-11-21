using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using souvenirs.Models;

namespace souvenirs.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Souvenir>().ToTable("Souvenir").HasMany(s=>s.OrderItems).WithOne(o=>o.Souvenir).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Category>().ToTable("Category");
            builder.Entity<Supplier>().ToTable("Supplier");
            builder.Entity<CartItem>().ToTable("CartItem");
            builder.Entity<Order>().ToTable("Order");
            // builder.Entity<OrderItem>().ToTable("OrderItem");
            builder.Entity<OrderItem>().HasOne(
                 p => p.Order).WithMany(o => o.OrderItems).OnDelete(DeleteBehavior.Cascade);

            /*builder.Entity<OrderItem>().HasKey(
                c => new { c.OrderID, c.SouvenirID });*/

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            
        }

        public DbSet<Souvenir> Souvenirs { get; set; }
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<souvenirs.Models.ShoppingCart> ShoppingCart { get; set; }
           
        

    }
}
