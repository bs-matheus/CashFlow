using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Services;

public interface ILoggedUser
{
    Task<User?> GetAsync();
}
