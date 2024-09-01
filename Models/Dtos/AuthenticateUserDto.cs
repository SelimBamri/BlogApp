using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models.Dtos
{
    public class AuthenticateUserDto
    {
        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
    }
}
