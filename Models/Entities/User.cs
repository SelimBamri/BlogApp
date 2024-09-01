using Microsoft.AspNetCore.Identity;

namespace BlogApp.Models.Entities
{
    public class User : IdentityUser<string>
    {
        public required string FirstName { get; set; }
        public required string lastName { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public required byte[] profilePhoto { get; set; }
    }
}
