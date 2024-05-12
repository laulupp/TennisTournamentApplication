using Backend.Api.Models;
using Backend.Services.Interfaces;
using System.Text;

namespace Backend.Services;

public class CsvMatchDataFormatter : IMatchDataFormatter
{
    public string Format(IEnumerable<MatchDTO> matches)
    {
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("MatchId,PlayerOne,PlayerTwo,Referee,Tournament,Score");

        foreach (var match in matches)
        {
            csvBuilder.AppendLine($"{match.Id},{match.PlayerOne.FirstName} {match.PlayerOne.LastName},{match.PlayerTwo.FirstName} {match.PlayerTwo.LastName},{match.Referee.FirstName} {match.Referee.LastName},{match.Tournament.Name},{match.Score}");
        }

        return csvBuilder.ToString();
    }
}