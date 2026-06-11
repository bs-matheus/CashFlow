using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Users;

public interface IUsersReadOnlyRepository
{
    Task<bool> ExistActiveUserWithEmailAsync(string email);

    Task<User?> GetUserByEmailAsync(string email);
}
