using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models.Dtos
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage ="First name is required")]
        public required string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public required string LastName { get; set; }
        [Required(ErrorMessage = "Date of birth is required")]
        public required DateTime DateOfBirth { get; set; }
        public string? ProfilePhoto { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public required string UserName { get; set; }

    }
}
