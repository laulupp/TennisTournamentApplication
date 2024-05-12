using Backend.Api.Models;
using Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendUnitTests;

public class MatchDataFormatterTests
{
    [Fact]
    public void FormatGivenMatchesShouldReturnCorrectCsvFormat()
    {
        // Arrange
        var matches = new List<MatchDTO>
        {
            new MatchDTO
            {
                Id = 1,
                PlayerOne = new UserDTO { FirstName = "John", LastName = "Doe" },
                PlayerTwo = new UserDTO { FirstName = "Jane", LastName = "Smith" },
                Referee = new UserDTO { FirstName = "Ref", LastName = "Eree" },
                Tournament = new TournamentDTO { Name = "Open" },
                Score = "6-3, 6-4"
            }
        };
        var formatter = new CsvMatchDataFormatter();

        // Expected string
        string expected = "MatchId,PlayerOne,PlayerTwo,Referee,Tournament,Score\r\n1,John Doe,Jane Smith,Ref Eree,Open,6-3, 6-4\r\n";

        // Act
        var result = formatter.Format(matches);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FormatGivenMatches_ShouldReturnCorrectTxtFormat()
    {
        // Arrange
        var matches = new List<MatchDTO>
        {
            new MatchDTO
            {
                Id = 1,
                PlayerOne = new UserDTO { FirstName = "John", LastName = "Doe" },
                PlayerTwo = new UserDTO { FirstName = "Jane", LastName = "Smith" },
                Referee = new UserDTO { FirstName = "Ref", LastName = "Eree" },
                Tournament = new TournamentDTO { Name = "Open" },
                Score = "6-3, 6-4"
            }
        };
        var formatter = new TxtMatchDataFormatter();

        // Expected string
        string expected = "Matches Information\r\nMatch ID: 1, Player One: John Doe, Player Two: Jane Smith, Referee: Ref Eree, Tournament: Open, Score: 6-3, 6-4\r\n";

        // Act
        var result = formatter.Format(matches);

        // Assert
        Assert.Equal(expected, result);
    }
}
