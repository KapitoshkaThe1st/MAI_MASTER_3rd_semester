using System.ComponentModel.DataAnnotations;

namespace ApplicationAPI.DTOs
{
    public class UserAccountRegistrationDto
    {
        [Required(ErrorMessage = "!!! Login is a required field.")]
        [MaxLength(50, ErrorMessage = "Maximum length for the Username is 30 characters.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Password is a required field.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Username is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Username is 30 characters.")]
        public string Username { get; init; }
        public string Status { get; init; } = "";
    }
}
