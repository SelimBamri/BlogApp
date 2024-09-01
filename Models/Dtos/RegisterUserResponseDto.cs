namespace BlogApp.Models.Dtos
{
    public class RegisterUserResponseDto
    {
        public bool IsSuccessfulRegistration { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
