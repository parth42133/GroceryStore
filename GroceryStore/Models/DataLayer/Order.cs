using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GroceryStore.Models.DataLayer
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        [Key]
        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalAmount { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Orders")]
        public virtual User? User { get; set; }
        [InverseProperty("Order")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
