using System.ComponentModel.DataAnnotations;

namespace ApplicationAPI.DTOs
{
    public record CommentManipulationDto
    {
        [Required(ErrorMessage = "PostId is a required field.")]
        public string PostId { get; init; }

        [Required(ErrorMessage = "AuthorId is a required field.")]
        public string AuthorId { get; init; }

        [Required(ErrorMessage = "Text is a required field.")]
        [MaxLength(1024, ErrorMessage = "Maximum length for the Username is 30 characters.")]
        public string Text { get; set; }
    }
}
