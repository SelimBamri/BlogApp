using System.Diagnostics.CodeAnalysis;

namespace BlogApp.Models.Dtos
{
    public class UpdateMyProfileRequestDto
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePhoto { get; set; }
    }
}
