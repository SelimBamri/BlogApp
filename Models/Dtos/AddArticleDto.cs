namespace BlogApp.Models.Dtos
{
    public class AddArticleDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Banner { get; set; }
        public required string Content { get; set; }
    }
}
