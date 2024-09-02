using System.Diagnostics.CodeAnalysis;

namespace BlogApp.Models.Dtos
{
    public class UpdateMyProfileRequestDto
    {
        public required string Email { get; set; }
        public string? Password { get; set; }
        public required string UserName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public byte[]? ProfilePhoto { get; set; }
        public bool RequireNewToken { get; set; }
    }
}
