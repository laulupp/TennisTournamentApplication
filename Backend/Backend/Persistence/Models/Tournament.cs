using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Backend.Persistence.Models;

[Table("tournaments", Schema = "tennis_schema")]
public class Tournament
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string? Name { get; set; }

    [Required]
    public DateTime? StartDate { get; set; }

    [Required]
    public DateTime? EndDate { get; set; }

    public virtual ICollection<Match> Matches { get; set; }
    public virtual ICollection<TournamentParticipant> TournamentParticipants { get; set; }

    public Tournament()
    {
        Matches = new HashSet<Match>();
        TournamentParticipants = new HashSet<TournamentParticipant>();
    }
}
