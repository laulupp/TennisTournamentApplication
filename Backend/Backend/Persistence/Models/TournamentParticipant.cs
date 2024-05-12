using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Persistence.Models;

[Table("tournament_participants", Schema = "tennis_schema")]
public class TournamentParticipant
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Tournament")]
    public int TournamentId { get; set; }
    public Tournament? Tournament { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User? User { get; set; }
    public bool Approved { get; set; } = false;
}
