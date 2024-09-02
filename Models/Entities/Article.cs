using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models.Entities
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Description { get; set; }
        [Required]
        public required byte[] Banner { get; set; }
        [Required]
        public required string Content { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public required DateTime Created { get; set; }

        public string? AuthorFk { get; set; }
        [ForeignKey("AuthorFk")]
        public required virtual User Author { get; set; }
    }
}
