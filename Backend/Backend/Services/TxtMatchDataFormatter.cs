using Backend.Api.Models;
using Backend.Services.Interfaces;
using System.Text;

namespace Backend.Services;

public class TxtMatchDataFormatter : IMatchDataFormatter
{
    public string Format(IEnumerable<MatchDTO> matches)
    {
        var txtBuilder = new StringBuilder();
        txtBuilder.AppendLine("Matches Information");

        foreach (var match in matches)
        {
            txtBuilder.AppendLine($"Match ID: {match.Id}, Player One: {match.PlayerOne.FirstName} {match.PlayerOne.LastName}, Player Two: {match.PlayerTwo.FirstName} {match.PlayerTwo.LastName}, Referee: {match.Referee.FirstName} {match.Referee.LastName}, Tournament: {match.Tournament.Name}, Score: {match.Score}");
        }

        return txtBuilder.ToString();
    }
}