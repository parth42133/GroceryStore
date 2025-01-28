using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GroceryStore.Models.DataLayer
{
    public partial class Product
    {
        public Product()
        {
            OrderItems = new HashSet<OrderItem>();
            ShoppingCartItems = new HashSet<ShoppingCartItem>();
        }

        [Key]
        public int ProductId { get; set; }
        [StringLength(100)]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public int? CategoryId { get; set; }
        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("Products")]
        public virtual Category? Category { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}
