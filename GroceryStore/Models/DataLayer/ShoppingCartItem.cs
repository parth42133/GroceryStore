using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GroceryStore.Models.DataLayer
{
    public partial class ShoppingCartItem
    {
        [Key]
        public int CartItemId { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("ShoppingCartItems")]
        public virtual Product? Product { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("ShoppingCartItems")]
        public virtual User? User { get; set; }
    }
}
