using Backend.Persistence.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Api.Models;

public class MatchDTO
{
    public int Id { get; set; }

    [Required]
    public DateTime? Date { get; set; }

    public string Score { get; set; } = string.Empty;

    [Required]
    public int? TournamentId { get; set; }
    public TournamentDTO? Tournament { get; set; }


    [Required]
    public int? PlayerOneId { get; set; }
    public UserDTO? PlayerOne { get; set; }

    [Required]
    public int? PlayerTwoId { get; set; }
    public UserDTO? PlayerTwo { get; set; }

    [Required]
    public int? RefereeId { get; set; }
    public UserDTO? Referee { get; set; }
}
