using Backend.Persistence.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Api.Models;

public class UpdateMatchDTO
{
    public int Id { get; set; }

    [Required]
    public DateTime? Date { get; set; }

    public string Score { get; set; } = string.Empty;

    [Required]
    public int? RefereeId { get; set; }
    public UserDTO? Referee { get; set; }
}
