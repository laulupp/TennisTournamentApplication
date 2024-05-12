using AutoMapper;
using Backend.Persistence.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Backend.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Api.Models;
using Backend.Persistence.Models;

namespace BackendUnitTests;

public class TournamentServiceTests
{
    private readonly Mock<ITournamentRepository> _tournamentRepositoryMock = new Mock<ITournamentRepository>();
    private readonly Mock<ITournamentParticipantRepository> _tournamentParticipantRepositoryMock = new Mock<ITournamentParticipantRepository>();
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
    private readonly Mock<IEmailService> _emailServiceMock = new Mock<IEmailService>();
    private TournamentService _service;

    public TournamentServiceTests()
    {
        _service = new TournamentService(_tournamentRepositoryMock.Object, _tournamentParticipantRepositoryMock.Object, _mapperMock.Object, _emailServiceMock.Object);
    }

    [Fact]
    public async Task GetAllAsyncWhenCalledReturnsTournamentsWithStatus()
    {
        // Arrange
        var tournaments = new List<Tournament>
    {
        new Tournament { Id = 1, Name = "Tournament One" },
        new Tournament { Id = 2, Name = "Tournament Two" }
    };
        var tournamentDtos = new List<TournamentDTO>
    {
        new TournamentDTO { Id = 1, Name = "Tournament One", Status = 0 },
        new TournamentDTO { Id = 2, Name = "Tournament Two", Status = 0 }
    };
        int userId = 1;

        _tournamentRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tournaments);
        _mapperMock.Setup(m => m.Map<IEnumerable<TournamentDTO>>(tournaments)).Returns(tournamentDtos);

        _tournamentParticipantRepositoryMock.Setup(repo => repo.IsUserEnrolledInTournament(userId, 1)).ReturnsAsync(true);
        _tournamentParticipantRepositoryMock.Setup(repo => repo.IsUserWaitingForApprovalInTournament(userId, 1)).ReturnsAsync(false);
        _tournamentParticipantRepositoryMock.Setup(repo => repo.IsUserEnrolledInTournament(userId, 2)).ReturnsAsync(false);
        _tournamentParticipantRepositoryMock.Setup(repo => repo.IsUserWaitingForApprovalInTournament(userId, 2)).ReturnsAsync(true);

        // Act
        var result = await _service.GetAllAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(2, result.First().Status);
        Assert.Equal(1, result.Last().Status);
    }

    [Fact]
    public async Task AddTournamentAsyncGivenTournamentDtoShouldAddTournament()
    {
        // Arrange
        var tournamentDto = new TournamentDTO { Name = "New Tournament" };
        var tournament = new Tournament { Name = "New Tournament" };

        _mapperMock.Setup(m => m.Map<Tournament>(tournamentDto)).Returns(tournament);
        _tournamentRepositoryMock.Setup(repo => repo.AddAsync(tournament)).ReturnsAsync(tournament);
        _mapperMock.Setup(m => m.Map<TournamentDTO>(tournament)).Returns(tournamentDto);

        // Act
        var result = await _service.AddTournamentAsync(tournamentDto);

        // Assert
        Assert.Equal(tournamentDto, result);
    }

    [Fact]
    public async Task DeleteAsyncValidTournamentIdShouldDeleteTournament()
    {
        // Arrange
        int tournamentId = 1;
        var tournament = new Tournament { Id = tournamentId, Name = "Tournament" };

        _tournamentRepositoryMock.Setup(repo => repo.GetByIdAsync(tournamentId)).ReturnsAsync(tournament);
        _tournamentRepositoryMock.Setup(repo => repo.DeleteAsync(tournamentId)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(tournamentId);

        // Assert
        _tournamentRepositoryMock.Verify(repo => repo.DeleteAsync(tournamentId), Times.Once());
    }

}
