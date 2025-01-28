using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GroceryStore.Models.DataLayer
{
    [Table("User")]
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
            ShoppingCartItems = new HashSet<ShoppingCartItem>();
        }

        [Key]
        [Column("UserID")]
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        [StringLength(50)]
        public string? Password { get; set; }
        public string? Role { get; set; }
        public bool? IsAdmin { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}
