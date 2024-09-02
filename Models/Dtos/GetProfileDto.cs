using System.Diagnostics.CodeAnalysis;

namespace BlogApp.Models.Dtos
{
    public class GetProfileDto
    {
        public required string UserName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public byte[]? ProfilePhoto { get; set; }
    }
}
