using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses;

public interface IExpensesReadOnlyRepository
{
    Task<List<Expense>> GetAllAsync(User loggedUser);

    Task<Expense?> GetByIdAsync(User loggedUser, long id);

    Task<List<Expense>> FilterByMonthAsync(User loggedUser, DateOnly date);
}
