using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses;

public interface IExpensesUpdateOnlyRepository
{
    Task<Expense?> GetByIdAsync(User loggedUser, long id);

    void Update(Expense expense);
}
