using AutoMapper;
using Backend.Api.Models;
using Backend.Persistence.Models;
using Backend.Persistence.Repositories.Interfaces;
using Backend.Services.Interfaces;
using static Backend.Exceptions.CustomExceptions;

namespace Backend.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMapper _mapper;

    public MatchService(IMatchRepository matchRepository, IMapper mapper)
    {
        _matchRepository = matchRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MatchDTO>> GetByPlayerIdAsync(int playerId)
    {
        return _mapper.Map<IEnumerable<MatchDTO>>(await _matchRepository.GetByPlayerIdAsync(playerId));
    }

    public async Task<IEnumerable<MatchDTO>> GetByRefereeIdAsync(int refereeId)
    {
        return _mapper.Map<IEnumerable<MatchDTO>>(await _matchRepository.GetByRefereeIdAsync(refereeId));
    }
    
    public async Task<IEnumerable<MatchDTO>> GetAllAsync(int? playerId, int? refereeId, int? tournamentId)
    {
        var matches = await _matchRepository.GetAllAsync();

        matches = matches.Where(t => playerId == null || t.PlayerOneId == playerId || t.PlayerTwoId == playerId);
        matches = matches.Where(t => refereeId == null || t.RefereeId == refereeId);
        matches = matches.Where(t => tournamentId == null || t.TournamentId == tournamentId);

        return _mapper.Map<IEnumerable<MatchDTO>>(matches);
    }

    public async Task<MatchDTO> AddMatchAsync(MatchDTO m)
    {
        var match = _mapper.Map<Match>(m);

        return _mapper.Map<MatchDTO>(await _matchRepository.AddAsync(match));
    }

    public async Task UpdateMatchAsync(UpdateMatchDTO m)
    {
        var match = await _matchRepository.GetByIdAsync(m.Id);

        if (match == null)
        {
            throw new MatchNotFoundException();
        }

        _mapper.Map(m, match);

        await _matchRepository.UpdateAsync(match);
    }

    public async Task UpdateScoreAsync(int matchId, string score)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);

        if (match == null)
        {
            throw new MatchNotFoundException();
        }

        match.Score = score;

        await _matchRepository.UpdateAsync(match);
    }

    public async Task DeleteByIdAsync(int matchId)
    {
         await _matchRepository.DeleteAsync(matchId);
    }

    public async Task DeleteByTournamentAsync(int tournamentId)
    {
        var matchIdsToBeDeleted = (await _matchRepository.GetAllAsync()).Where(m => m.TournamentId == tournamentId).Select(m => m.Id);

        foreach (var id in matchIdsToBeDeleted)
        {
            await _matchRepository.DeleteAsync(id);
        }
    }

    public async Task DeleteByUserAsync(int userId)
    {
        var matchIdsToBeDeleted = (await _matchRepository.GetAllAsync()).Where(m => m.PlayerOneId == userId || m.PlayerTwoId == userId || m.RefereeId == userId).Select(m => m.Id);

        foreach (var id in matchIdsToBeDeleted)
        {
            await _matchRepository.DeleteAsync(id);
        }
    }
}
