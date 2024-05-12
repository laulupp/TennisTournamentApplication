using System.ComponentModel.DataAnnotations;

namespace Backend.Api.Models;

public class RegisterDTO
{
    [MaxLength(100)]
    [Required]
    public string? Username { get; set; }

    [MaxLength(500)]
    [Required]
    public string? Password { get; set; }

    [MaxLength(100)]
    [Required]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    [Required]
    public string? LastName { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    [Required]
    public string? Email { get; set; }

    [MaxLength(100)]
    [RegularExpression("([0-9]+)")]
    public string? PhoneNumber { get; set; }

    public bool IsReferee { get; set; } = false;
}
