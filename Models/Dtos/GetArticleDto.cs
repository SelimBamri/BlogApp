namespace BlogApp.Models.Dtos
{
    public class GetArticleDto
    {
        public required int Id { get; set; }
        public required DateTime Created { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Banner { get; set; }
        public required string Content { get; set; }
    }
}
