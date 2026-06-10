using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.Users;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.DataAccess.Repositories;

internal class UsersRepository : IUsersReadOnlyRepository, IUsersWriteOnlyRepository
{
    private readonly CashFlowDbContext _dbContext;

    public UsersRepository(CashFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region ReadOnly
    public async Task<bool> ExistActiveUserWithEmailAsync(string email)
    {
        return await _dbContext.Users.AsNoTracking().AnyAsync(user => user.Email.Equals(email));
    }
    #endregion

    #region WriteOnly
    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }
    #endregion
}
