using Microsoft.EntityFrameworkCore;
using PokerPlanning.Application.src.Common.Interfaces.Persistence;
using PokerPlanning.Domain.src.Models.VotingSystemAggregate;

namespace PokerPlanning.Infrastructure.src.Persistence.Repositories;

public class VotingSystemRepository : IVotingSystemRepository
{
    private readonly PokerPlanningDbContext _dbContext;

    public VotingSystemRepository(PokerPlanningDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<VotingSystem>> Get()
    {
        return await _dbContext.VotingSystems
            .Include(vs => vs.Votes)
            .ToListAsync();
    }
}