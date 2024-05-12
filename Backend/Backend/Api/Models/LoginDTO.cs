using System.ComponentModel.DataAnnotations;

namespace Backend.Api.Models;

public class LoginDTO
{
    [MaxLength(100)]
    [Required]
    public string? Username { get; set; }

    [MaxLength(500)]
    [Required]
    public string? Password { get; set; }
}
