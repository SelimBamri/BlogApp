namespace BlogApp.Models.Dtos
{
    public class UpdateMyPasswordDto
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
