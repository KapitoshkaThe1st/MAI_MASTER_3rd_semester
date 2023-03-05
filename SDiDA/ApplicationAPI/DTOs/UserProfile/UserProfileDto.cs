using System.ComponentModel.DataAnnotations;

namespace ApplicationAPI.DTOs
{
    public record UserProfileDto
    {
        public string Id { get; init; }
        public string Username { get; init; }
        public string Status { get; init; }
    }
}
