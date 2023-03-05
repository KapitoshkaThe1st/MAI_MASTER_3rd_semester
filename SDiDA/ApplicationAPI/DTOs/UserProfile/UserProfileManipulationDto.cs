using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApplicationAPI.DTOs
{
    public record UserProfileManipulationDto
    {
        [Required(ErrorMessage = "Username is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Username is 30 characters.")]
        public string Username { get; init; }
        public string Status { get; init; }
    }
}
