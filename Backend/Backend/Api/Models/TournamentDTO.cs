using Backend.Attributes;
using Backend.Persistence.Models;
using System.ComponentModel.DataAnnotations;

namespace Backend.Api.Models;

public class TournamentDTO
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    [DateGreaterThan("EndDate", ErrorMessage = "Start Date must be earlier than End Date.")]
    public DateTime? StartDate { get; set; }

    [Required]
    public DateTime? EndDate { get; set; }

    public int Status { get; set; } = 0; //0 = not enrolled, 1 = pending, 2 = enrolled
}
