using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GroceryStore.Models.DataLayer
{
    public partial class GroceryContext : DbContext
    {
        public GroceryContext()
        {
        }

        public GroceryContext(DbContextOptions<GroceryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=GroceryContext");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Orders__UserId__440B1D61");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__OrderItem__Order__3D5E1FD2");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__OrderItem__Produ__3E52440B");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__Products__Catego__38996AB5");
            });

            modelBuilder.Entity<ShoppingCartItem>(entity =>
            {
                entity.HasKey(e => e.CartItemId)
                    .HasName("PK__Shopping__488B0B0A23D8BD86");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ShoppingCartItems)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ShoppingC__Produ__412EB0B6");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ShoppingCartItems)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__ShoppingC__UserI__44FF419A");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
