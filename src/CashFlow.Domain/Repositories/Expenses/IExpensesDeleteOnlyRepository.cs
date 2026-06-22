namespace CashFlow.Domain.Repositories.Expenses;

public interface IExpensesDeleteOnlyRepository
{
    /// <summary>
    /// Deletes an expense by searching for the specified ID
    /// </summary>
    Task DeleteByIdAsync(long id);
}
