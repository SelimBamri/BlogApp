namespace BlogApp.Models.Dtos
{
    public class AuthResponseDto
    {
        public bool IsAuthenticated { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Token { get; set; }
    }
}
