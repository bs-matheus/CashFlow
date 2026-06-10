using CashFlow.Domain.Repositories.User;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.DataAccess.User;

internal class UserRepository : IUserReadOnlyRepository
{
    private readonly CashFlowDbContext _dbContext;

    public UserRepository(CashFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistActiveUserWithEmailAsync(string email)
    {
        return await _dbContext.Users.AsNoTracking().AnyAsync(user => user.Email.Equals(email));
    }
}
