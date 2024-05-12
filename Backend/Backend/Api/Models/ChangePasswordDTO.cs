using System.ComponentModel.DataAnnotations;

namespace Backend.Api.Models;

public class ChangePasswordDTO
{
    public string Username { get; set; } = string.Empty;
    public string OldPassword { get; set; } = string.Empty;

    [MaxLength(500)]
    [Required]
    public string? NewPassword { get; set; }
}
