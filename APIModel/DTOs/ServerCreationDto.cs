using System.ComponentModel.DataAnnotations;

namespace APIModel.DTOs
{
    public class ServerCreationDto
    {
        [Required]
        public string? NewName { get; set; }
    }
}
