using System.ComponentModel.DataAnnotations;

namespace GroceryStore.Models
{
    public class UserRequestModel
    {
        public int? UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        [StringLength(50)]
        public string? Password { get; set; }
        public string? Role { get; set; }
        public bool? IsAdmin { get; set; }
    }
}
