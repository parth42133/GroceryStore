using GroceryStore.Models.DataLayer;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GroceryStore.Models
{
    public class ProductRequestModel
    {
        public int? ProductId { get; set; }
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Price { get; set; }
        //public int? Stock { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        public IFormFile? fileData { get; set; }

        public string? ProductURL { get; set; }

        public Category? Category { get; set; }
    }
}
