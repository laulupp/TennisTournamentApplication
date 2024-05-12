using Backend.Api.Models;
using Backend.Attributes;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using static Backend.Exceptions.CustomExceptions;
using static Backend.Constants.Permissions;
using System.Text;
using Backend.Services.Interfaces;

namespace Backend.Api.Controllers;

[ApiController]
[Route("match")]
public class MatchController : ControllerBase
{
    private readonly ITournamentService _tournamentService;
    private readonly IMatchService _matchService;
    private readonly IUserService _userService;

    public MatchController(ITournamentService tournamentService, IMatchService matchService, IUserService userService)
    {
        _tournamentService = tournamentService;
        _matchService = matchService;
        _userService = userService;
    }

    [HttpGet]
    [Permission(Player, Referee)]
    public async Task<IActionResult> GetUserMatches([FromQuery] DateTime? after, [FromQuery] string? playerOne, [FromQuery] string? playerTwo, [FromQuery] bool? isScored)
    {
        var username = (string)HttpContext.Request.Headers[Constants.Headers.Username]!;
        var isRefereeRequest = (bool)HttpContext.Items[Constants.Items.IsReferee]!;

        var user = await _userService.GetByUsernameAsync(username);
        if (user == null)
        {
            throw new UserNotFoundException(isRefereeRequest ? "Referee" : "Player");
        }

        var matches = isRefereeRequest ? await _matchService.GetByRefereeIdAsync(user.Id!.Value) : await _matchService.GetByPlayerIdAsync(user.Id!.Value);

        await EnrichWithUsersAndTournament(matches);

        var filteredMatches = matches.AsQueryable();
        filteredMatches = after == null ? filteredMatches : filteredMatches.Where(f => f.Date >= after);
        filteredMatches = playerOne == null ? filteredMatches : filteredMatches.Where(f => f.PlayerOne!.FirstName!.ToLower().StartsWith(playerOne.ToLower())
                                                                                        || f.PlayerOne!.LastName!.ToLower().StartsWith(playerOne.ToLower()));
        filteredMatches = playerTwo == null ? filteredMatches : filteredMatches.Where(f => f.PlayerTwo!.FirstName!.ToLower().StartsWith(playerTwo.ToLower())
                                                                                        || f.PlayerTwo!.LastName!.ToLower().StartsWith(playerTwo.ToLower()));
        filteredMatches = isScored == null ? filteredMatches : filteredMatches.Where(f => f.Score != string.Empty);

        return Ok(filteredMatches.ToList());
    }    
    
    [HttpGet("all")]
    [Permission(Admin, Player)]
    public async Task<IActionResult> GetAllMatches([FromQuery] int? playerId, [FromQuery] int? refereeId, [FromQuery] int? tournamentId)
    {
        var matches = await _matchService.GetAllAsync(playerId, refereeId, tournamentId);

        await EnrichWithUsersAndTournament(matches);

        return Ok(matches);
    }

    [HttpGet("download")]
    [Permission(Admin)]
    public async Task<IActionResult> DownloadMatches([FromQuery] int? playerId, [FromQuery] int? refereeId, [FromQuery] int? tournamentId, [FromQuery] bool downloadAsCsv = false)
    {
        var matches = await _matchService.GetAllAsync(playerId, refereeId, tournamentId);

        await EnrichWithUsersAndTournament(matches);

        IMatchDataFormatter formatter = downloadAsCsv ? new CsvMatchDataFormatter() : new TxtMatchDataFormatter();

        var fileContent = formatter.Format(matches);
        var contentType = downloadAsCsv ? "text/csv" : "text/plain";
        var fileName = downloadAsCsv ? "matches.csv" : "matches.txt";

        var byteArray = Encoding.UTF8.GetBytes(fileContent);
        var stream = new MemoryStream(byteArray);

        return File(stream, contentType, fileName);
    }

    [HttpPost]
    [Permission(Admin)]
    public async Task<IActionResult> AddMatchToTournament([FromBody] MatchDTO matchDTO)
    {
        var tournament = await _tournamentService.GetTournamentByIdAsync(matchDTO.TournamentId!.Value);

        if (tournament == null)
        {
            throw new TournamentNotFoundException();
        }

        await CheckPlayersAndReferee(matchDTO);

        return Ok(await _matchService.AddMatchAsync(matchDTO));
    }

    [HttpPut]
    [Permission(Admin)]
    public async Task<IActionResult> UpdateMatch([FromBody] UpdateMatchDTO matchDTO)
    {
        await _matchService.UpdateMatchAsync(matchDTO);

        return Ok();
    }

    [HttpPut("{matchId}/score")]
    [Permission(Referee)]
    public async Task<IActionResult> UpdateScore([FromRoute] int matchId, [FromBody] string score)
    {

        await _matchService.UpdateScoreAsync(matchId, score);

        return Ok();
    }    
    
    [HttpDelete("{matchId}")]
    [Permission(Admin)]
    public async Task<IActionResult> DeleteMatch([FromRoute] int matchId)
    {

        await _matchService.DeleteByIdAsync(matchId);

        return Ok();
    }

    #region HELPER
    private async Task CheckPlayersAndReferee(MatchDTO matchDTO)
    {
        if(matchDTO.PlayerOneId!.Value == matchDTO.PlayerTwoId!.Value)
        {
            throw new SamePlayersExceptions();
        }

        if ((await _userService.GetByIdAsync(matchDTO.PlayerOneId!.Value)) == null)
        {
            throw new UserNotFoundException("Player One");
        }        
        
        if ((await _userService.GetByIdAsync(matchDTO.PlayerOneId!.Value)) == null)
        {
            throw new UserNotFoundException("Player Two");
        }        
        
        if ((await _userService.GetByIdAsync(matchDTO.RefereeId!.Value)) == null)
        {
            throw new UserNotFoundException("Referee");
        }
    }

    private async Task EnrichWithUsersAndTournament(IEnumerable<MatchDTO> matchDTO)
    {
        foreach (var match in matchDTO)
        {
            match.Referee = await _userService.GetByIdAsync(match.RefereeId!.Value);
            match.PlayerOne = await _userService.GetByIdAsync(match.PlayerOneId!.Value);
            match.PlayerTwo = await _userService.GetByIdAsync(match.PlayerTwoId!.Value);
            match.Tournament = await _tournamentService.GetTournamentByIdAsync(match.TournamentId!.Value);
        }
    }
    #endregion
}
