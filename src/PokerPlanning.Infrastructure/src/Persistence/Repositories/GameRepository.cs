using Microsoft.EntityFrameworkCore;
using PokerPlanning.Application.src.Common.DTO.GameFeature;
using PokerPlanning.Application.src.Common.Interfaces.Persistence;
using PokerPlanning.Domain.src.Models.GameAggregate;
using PokerPlanning.Domain.src.Models.GameAggregate.Entities;

namespace PokerPlanning.Infrastructure.src.Persistence.Repositories;

public class GameRepository : IGameRepository
{
    private readonly PokerPlanningDbContext _dbContext;

    public GameRepository(PokerPlanningDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateParticipant(Participant participant, CancellationToken cancellationToken)
    {
        await _dbContext.Participants.AddAsync(participant, cancellationToken);
    }

    public async Task Create(Game game, CancellationToken cancellationToken)
    {
        await _dbContext.Games.AddAsync(game, cancellationToken);
    }

    public async Task<Game?> Get(Guid gameId, CancellationToken cancellationToken)
    {
        return await _dbContext.Games
            .Include(g => g.VotingSystem)
            .ThenInclude(vs => vs.Votes)
            .Include(g => g.Participants)
            .Include(g => g.Tickets)
            .SingleAsync(g => g.Id == gameId, cancellationToken);
    }

    public async Task<Participant?> GetParticipant(Guid gameId, Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Participants
            .Include(p => p.Vote)
            .Include(p => p.Game)
            .SingleAsync(p => p.GameId == gameId && p.UserId == userId, cancellationToken);
    }
}
