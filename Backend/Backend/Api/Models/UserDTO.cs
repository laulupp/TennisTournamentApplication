using Backend.Persistence.Models;
using System.ComponentModel.DataAnnotations;

namespace Backend.Api.Models;

public class UserDTO
{
    public int? Id { get; set; }

    public Role? Role { get; set; }

    [Required]
    [MaxLength(100)]
    public string? Username { get; set; }

    [Required]
    [MaxLength(500)]
    public string? FirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string? Email { get; set; }

    [Required]
    [MaxLength(100)]
    [RegularExpression("([0-9]+)")]
    public string? PhoneNumber { get; set; }

    public string? Password { get; set; }
}
