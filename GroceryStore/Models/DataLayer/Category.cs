using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GroceryStore.Models.DataLayer
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public int CategoryId { get; set; }
        [StringLength(100)]
        public string? CategoryName { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
