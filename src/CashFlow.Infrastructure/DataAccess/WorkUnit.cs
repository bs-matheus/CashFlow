using CashFlow.Domain.Repositories;

namespace CashFlow.Infrastructure.DataAccess;

internal class WorkUnit : IWorkUnit
{
    private readonly CashFlowDbContext _dbContext;

    public WorkUnit(CashFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CommitAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
