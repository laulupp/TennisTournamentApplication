using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Persistence.Models;

[Table("matches", Schema = "tennis_schema")]
public class Match
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [MaxLength(50)]
    public string Score { get; set; } = string.Empty;

    [ForeignKey("Tournament")]
    public int TournamentId { get; set; }
    public virtual Tournament? Tournament { get; set; }

    [ForeignKey("PlayerOne")]
    public int PlayerOneId { get; set; }
    public virtual User? PlayerOne { get; set; }

    [ForeignKey("PlayerTwo")]
    public int PlayerTwoId { get; set; }
    public virtual User? PlayerTwo { get; set; }

    [ForeignKey("Referee")]
    public int RefereeId { get; set; }
    public virtual User? Referee { get; set; }
}
