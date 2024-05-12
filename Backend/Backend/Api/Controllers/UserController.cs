using Microsoft.AspNetCore.Mvc;
using Backend.Api.Models;
using Backend.Attributes;
using static Backend.Constants.Permissions;
using Backend.Services.Interfaces;

namespace Backend.Api.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMatchService _matchService;
    private readonly ITournamentService _tournamentService;

    public UserController(IUserService userService, IMatchService matchService, ITournamentService tournamentService)
    {
        _userService = userService;
        _matchService = matchService;
        _tournamentService = tournamentService;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateInfo([FromBody] UserDTO dto)
    {
        await _userService.UpdateInfoAsync(dto, Request.Headers[Constants.Headers.Username].ToString(), (bool)HttpContext.Items[Constants.Items.IsAdmin]!);
        return Ok();
    }

    [HttpPut("changepassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
    {
        bool isAdminRequest = (bool)HttpContext.Items[Constants.Items.IsAdmin]!;

        var username = isAdminRequest ? dto.Username : Request.Headers[Constants.Headers.Username].ToString();
        await _userService.ChangePasswordAsync(username, dto.OldPassword, dto.NewPassword!, isAdminRequest);

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] string? usernameToBeDeleted)
    {
        bool isAdminRequest = (bool)HttpContext.Items[Constants.Items.IsAdmin]!;

        var username = isAdminRequest && usernameToBeDeleted != string.Empty ? usernameToBeDeleted : Request.Headers[Constants.Headers.Username].ToString();

        var userId = (await _userService.GetByUsernameAsync(username)).Id!.Value;

        await _matchService.DeleteByUserAsync(userId);
        await _tournamentService.DeleteUsersFromTournamentAsync(userId: userId);
        await _userService.DeleteByUsernameAsync(username);

        return Ok();
    }

    [HttpGet]
    [Permission(Admin)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();

        return Ok(users);
    }        
    
    [HttpGet("personal")]
    [Permission(Admin, Player, Referee)]
    public async Task<IActionResult> GetPersonalInfo()
    {
        var user = await _userService.GetByUsernameAsync(Request.Headers[Constants.Headers.Username].ToString());

        return Ok(user);
    }    
    
    [HttpGet("referees")]
    [Permission(Admin)]
    public async Task<IActionResult> GetAllReferees()
    {
        var referees = await _userService.GetAllRefereesAsync();

        return Ok(referees);
    }

    [HttpGet("players")]
    [Permission(Admin)]
    public async Task<IActionResult> GetAllPlayers()
    {
        var players = await _userService.GetAllPlayersAsync();

        return Ok(players);
    }
}
