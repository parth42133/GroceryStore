using System.ComponentModel.DataAnnotations;

namespace GroceryStore.Models
{
    public class RegisterRequestModel
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}$", ErrorMessage = "Please enter a valid E-mail address")]
        public string? Email { get; set; }
        [Required]
        [StringLength(50)]
        public string? Password { get; set; }
    }
}
