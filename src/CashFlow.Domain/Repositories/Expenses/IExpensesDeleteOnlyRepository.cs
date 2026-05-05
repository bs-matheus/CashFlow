namespace CashFlow.Domain.Repositories.Expenses;

public interface IExpensesDeleteOnlyRepository
{
    /// <summary>
    /// Deletes an expense by searching for the specified ID
    /// </summary>
    /// <returns><c>true</c> if the deletion was successful, otherwise <c>false</c></returns>
    Task<bool> DeleteByIdAsync(long id);
}
