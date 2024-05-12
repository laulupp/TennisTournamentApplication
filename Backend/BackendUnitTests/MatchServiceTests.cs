using AutoMapper;
using Backend.Api.Models;
using Backend.Persistence.Models;
using Backend.Persistence.Repositories.Interfaces;
using Backend.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MMatch = Backend.Persistence.Models.Match;
namespace BackendUnitTests;

public class MatchServiceTests
{
    private readonly Mock<IMatchRepository> _matchRepositoryMock = new Mock<IMatchRepository>();
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
    private MatchService _service;

    public MatchServiceTests()
    {
        _service = new MatchService(_matchRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetByPlayerIdAsyncShouldReturnMatches()
    {
        // Arrange
        var date = DateTime.Now;
        var matches = new List<MMatch>
        {
            new MMatch { PlayerOneId = 1, PlayerTwoId = 2 , RefereeId = 4, Date = date, Id = 500, TournamentId = 3},
            new MMatch { PlayerOneId = 1, PlayerTwoId = 3 , RefereeId = 4, Date = date, Id = 501, TournamentId = 4}
        };
        var matchDTOs = new List<MatchDTO>
        {
            new MatchDTO { PlayerOneId = 1, PlayerTwoId = 2 , RefereeId = 4, Date = date, Id = 500, TournamentId = 3 },
            new MatchDTO { PlayerOneId = 1, PlayerTwoId = 3 , RefereeId = 4, Date = date, Id = 501, TournamentId = 4 }
        };

        _matchRepositoryMock.Setup(repo => repo.GetByPlayerIdAsync(1)).ReturnsAsync(matches);
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<MatchDTO>>(matches)).Returns(matchDTOs);

        // Act
        var result = await _service.GetByPlayerIdAsync(1);

        // Assert
        Assert.Equal(matchDTOs, result);
    }

    [Fact]
    public async Task AddMatchAsyncShouldAddMatch()
    {
        // Arrange
        var date = DateTime.Now;
        var matchDTO = new MatchDTO { PlayerOneId = 1, PlayerTwoId = 2, RefereeId = 4, Date = date, Id = 500, TournamentId = 3 };
        var match = new MMatch { PlayerOneId = 1, PlayerTwoId = 2, RefereeId = 4, Date = date, Id = 500, TournamentId = 3 };

        _mapperMock.Setup(mapper => mapper.Map<MMatch>(matchDTO)).Returns(match);
        _mapperMock.Setup(mapper => mapper.Map<MatchDTO>(match)).Returns(matchDTO);
        _matchRepositoryMock.Setup(repo => repo.AddAsync(match)).ReturnsAsync(match);

        // Act
        var result = await _service.AddMatchAsync(matchDTO);

        // Assert
        Assert.Equal(matchDTO, result);
    }

    [Fact]
    public async Task DeleteByIdAsyncShouldInvokeDeleteOnRepository()
    {
        // Arrange
        int matchId = 1;

        _matchRepositoryMock.Setup(repo => repo.DeleteAsync(matchId)).Returns(Task.CompletedTask).Verifiable();

        // Act
        await _service.DeleteByIdAsync(matchId);

        // Assert
        _matchRepositoryMock.Verify(repo => repo.DeleteAsync(matchId), Times.Once);
    }

}
