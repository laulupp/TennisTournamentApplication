using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Api.Models;

public class TournamentParticipantDTO
{
    public int Id { get; set; }

    public int TournamentId { get; set; }
    public TournamentDTO? Tournament { get; set; }
    public int UserId { get; set; }
    public UserDTO? User { get; set; }
    public bool Approved { get; set; } = false;
}
