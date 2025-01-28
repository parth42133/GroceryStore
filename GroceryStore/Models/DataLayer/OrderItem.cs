using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GroceryStore.Models.DataLayer
{
    public partial class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Price { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("OrderItems")]
        public virtual Order? Order { get; set; }
        [ForeignKey("ProductId")]
        [InverseProperty("OrderItems")]
        public virtual Product? Product { get; set; }
    }
}
