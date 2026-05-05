using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses;

public interface IExpensesReadOnlyRepository
{
    Task<List<Expense>> GetAllAsync();

    Task<Expense?> GetByIdAsync(long id);

    Task<List<Expense>> FilterByMonthAsync(DateOnly date);
}
