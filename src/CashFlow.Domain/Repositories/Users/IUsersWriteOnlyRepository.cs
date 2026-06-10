using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Users;

public interface IUsersWriteOnlyRepository
{
    Task AddAsync(User user);
}
