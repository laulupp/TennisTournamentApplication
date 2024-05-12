using Backend.Api.Models;
using Backend.Attributes;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static Backend.Constants.Permissions;

namespace Backend.Api.Controllers;

[ApiController]
[Route("tournament")]
public class TournamentController : ControllerBase
{
    private readonly ITournamentService _tournamentService;
    private readonly IMatchService _matchService;
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;

    public TournamentController(ITournamentService tournamentService, IMatchService matchService, IUserService userService, IEmailService emailService)
    {
        _tournamentService = tournamentService;
        _matchService = matchService;
        _userService = userService;
        _emailService = emailService;
    }

    [HttpGet]
    [Permission(Admin, Player)]
    public async Task<IActionResult> GetAllTournaments()
    {
        var username = Request.Headers[Constants.Headers.Username].ToString()!;

        var userId = (await _userService.GetByUsernameAsync(username)).Id;

        var tournaments = await _tournamentService.GetAllAsync(userId!.Value);

        return Ok(tournaments);
    }

    [HttpGet("{tournamentId}/players")]
    [Permission(Admin)]
    public async Task<IActionResult> GetPlayersInTournament([FromRoute] int tournamentId)
    {
        var users = new List<UserDTO>();
        var userIds = await _tournamentService.GetUserIdsByTournamentAsync(tournamentId);

        foreach (var userId in userIds)
        {
            users.Add(await _userService.GetByIdAsync(userId));
        }

        return Ok(users);
    }

    [HttpGet("pendingrequests")]
    [Permission(Admin)]
    public async Task<IActionResult> GetPendingRequests()
    {
        var requests = await _tournamentService.GetPendingTournamentRequestsAsync();

        foreach (var request in requests)
        {
            request.User = await _userService.GetByIdAsync(request.UserId);
            request.Tournament = await _tournamentService.GetTournamentByIdAsync(request.TournamentId);
        }

        return Ok(requests);
    }

    [HttpPost]
    [Permission(Admin)]
    public async Task<IActionResult> AddTournament(TournamentDTO dto)
    {
        return Ok(await _tournamentService.AddTournamentAsync(dto));
    }

    [HttpPut]
    [Permission(Admin)]
    public async Task<IActionResult> UpdateTournament(TournamentDTO dto)
    {
        await _tournamentService.UpdateTournamentAsync(dto);
        return Ok();
    }

    [HttpPost("{tournamentId}/enroll")]
    [Permission(Player)]
    public async Task<IActionResult> EnrollInTournament([FromRoute] int tournamentId)
    {
        var username = (string)Request.Headers[Constants.Headers.Username]!;

        var user = await _userService.GetByUsernameAsync(username);

        await _tournamentService.AddUserToTournamentAsync(user.Id!.Value, tournamentId);

        return Ok();
    }

    [HttpDelete("{tournamentId}")]
    [Permission(Admin)]
    public async Task<IActionResult> DeleteTournament([FromRoute] int tournamentId)
    {
        await _matchService.DeleteByTournamentAsync(tournamentId);
        await _tournamentService.DeleteUsersFromTournamentAsync(tournamentId: tournamentId);
        await _tournamentService.DeleteAsync(tournamentId);

        return Ok();
    }

    [HttpPost("approve")]
    [Permission(Admin)]
    public async Task<IActionResult> ApproveRequest([FromQuery] int tournamentId, [FromQuery] int userId)
    {
        await _tournamentService.ApproveRequestAsync(tournamentId, userId);

        var user = await _userService.GetByIdAsync(userId);
        var tournament = await _tournamentService.GetTournamentByIdAsync(tournamentId);

        if (user == null || tournament == null)
        {
            return BadRequest();
        }

        _emailService.SendMail(
            user.Email!,
            Constants.EmailMessages.ApprovalSubject,
            Constants.EmailMessages.ApprovalMessage(
                 user.FirstName!,
                 user.LastName!,
                 tournament.Name!
            )
        );

        return Ok();
    }
    
    [HttpPost("deny")]
    [Permission(Admin)]
    public async Task<IActionResult> DenyRequest([FromQuery] int tournamentId, [FromQuery] int userId)
    {
        await _tournamentService.DenyRequestAsync(tournamentId, userId);

        var user = await _userService.GetByIdAsync(userId);
        var tournament = await _tournamentService.GetTournamentByIdAsync(tournamentId);

        if (user == null || tournament == null)
        {
            return BadRequest();
        }

        _emailService.SendMail(
            user.Email!,
            Constants.EmailMessages.RejectionSubject,
            Constants.EmailMessages.RejectionMessage(
                 user.FirstName!,
                 user.LastName!,
                 tournament.Name!
            )
        );

        return Ok();
    }
}
