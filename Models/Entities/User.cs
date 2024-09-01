using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace BlogApp.Models.Entities
{
    public class User : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required DateTime DateOfBirth { get; set; }
        [AllowNull]
        public byte[]? ProfilePhoto { get; set; }
    }
}
