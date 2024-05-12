using AutoMapper;
using Backend.Api.Models;
using Backend.Persistence.Models;
using Backend.Persistence.Repositories.Interfaces;
using Backend.Services.Interfaces;
using static Backend.Exceptions.CustomExceptions;

namespace Backend.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ITournamentParticipantRepository _tournamentParticipantRepository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public TournamentService(ITournamentRepository tournamentRepository, ITournamentParticipantRepository tournamentParticipantRepository, IMapper mapper, IEmailService emailService)
    {
        _tournamentRepository = tournamentRepository;
        _tournamentParticipantRepository = tournamentParticipantRepository;
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task<IEnumerable<TournamentDTO>> GetAllAsync(int userId)
    {
        var tournaments = await _tournamentRepository.GetAllAsync();
        var tournamentDtos = _mapper.Map<IEnumerable<TournamentDTO>>(tournaments);

        foreach (var item in tournamentDtos)
        {
            item.Status = await _tournamentParticipantRepository.IsUserEnrolledInTournament(userId, item.Id) ? 2 :
                          await _tournamentParticipantRepository.IsUserWaitingForApprovalInTournament(userId, item.Id) ? 1 : 0;
        }

        return tournamentDtos;
    }

    public async Task<TournamentDTO?> GetTournamentByIdAsync(int tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);

        if (tournament == null)
        {
            throw new TournamentNotFoundException();
        }

        return _mapper.Map<Tournament, TournamentDTO>(tournament);

    }

    public async Task<TournamentDTO> AddTournamentAsync(TournamentDTO dto)
    {
        var tournament = _mapper.Map<Tournament>(dto);

        var addedTournament = await _tournamentRepository.AddAsync(tournament);

        return _mapper.Map<TournamentDTO>(addedTournament);
    }

    public async Task UpdateTournamentAsync(TournamentDTO dto)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(dto.Id);

        if (tournament == null)
        {
            throw new TournamentNotFoundException();
        }

        _mapper.Map(dto, tournament);

        await _tournamentRepository.UpdateAsync(tournament);
    }

    public async Task ApproveRequestAsync(int tournamentId, int userId)
    {
        var linkage = (await _tournamentParticipantRepository.GetAllAsync()).Where(l => l.UserId == userId && l.TournamentId == tournamentId).FirstOrDefault();

        if (linkage == null)
        {
            throw new LinkageNotFoundException();
        }

        await _tournamentParticipantRepository.ApproveLinkageAsync(linkage);
    }

    public async Task DenyRequestAsync(int tournamentId, int userId)
    {
        var linkageId = (await _tournamentParticipantRepository.GetAllAsync()).Where(l => l.UserId == userId && l.TournamentId == tournamentId).Select(l => l.Id);

        if (linkageId == null)
        {
            throw new LinkageNotFoundException();
        }

        await _tournamentParticipantRepository.DeleteAsync(linkageId.FirstOrDefault());
    }

    public async Task AddUserToTournamentAsync(int userId, int tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);

        if (tournament == null)
        {
            throw new TournamentNotFoundException();
        }

        bool isUserAlreadyEnrolled = (await _tournamentParticipantRepository.IsUserEnrolledInTournament(userId, tournamentId))
                                    || (await _tournamentParticipantRepository.IsUserWaitingForApprovalInTournament(userId, tournamentId));
        if (isUserAlreadyEnrolled)
        {
            throw new UserAlreadyRegisteredInTournamentException();
        }

        var linkage = new TournamentParticipant
        {
            UserId = userId,
            TournamentId = tournamentId
        };

        await _tournamentParticipantRepository.AddAsync(linkage);     
    }

    public async Task<IEnumerable<TournamentParticipantDTO>> GetPendingTournamentRequestsAsync()
    {
        return _mapper.Map<IEnumerable<TournamentParticipantDTO>>((await _tournamentParticipantRepository.GetAllAsync()).Where(l => l.Approved == false));
    }

    public async Task<IEnumerable<int>> GetUserIdsByTournamentAsync(int tournamentId)
    {
        return (await _tournamentParticipantRepository.GetAllAsync()).Where(l => l.TournamentId == tournamentId && l.Approved).Select(l => l.UserId);
    }

    public async Task DeleteUsersFromTournamentAsync(int? tournamentId = null, int? userId = null)
    {
        var matchIdsToBeDeleted = (await _tournamentParticipantRepository.GetAllAsync()).Where(m => (tournamentId != null && m.TournamentId == tournamentId) || (userId != null && m.UserId == userId)).Select(m => m.Id);

        foreach (var id in matchIdsToBeDeleted)
        {
            await _tournamentParticipantRepository.DeleteAsync(id);
        }
    }

    public async Task DeleteAsync(int tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);

        if (tournament == null)
        {
            throw new TournamentNotFoundException();
        }

        await _tournamentRepository.DeleteAsync(tournamentId);
    }
}
