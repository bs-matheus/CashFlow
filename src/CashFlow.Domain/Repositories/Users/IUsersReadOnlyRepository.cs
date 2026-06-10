namespace CashFlow.Domain.Repositories.Users;

public interface IUsersReadOnlyRepository
{
    Task<bool> ExistActiveUserWithEmailAsync(string email);
}
