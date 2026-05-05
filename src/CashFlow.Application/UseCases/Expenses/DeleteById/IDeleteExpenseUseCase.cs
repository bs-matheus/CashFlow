namespace CashFlow.Application.UseCases.Expenses.DeleteById;

public interface IDeleteExpenseUseCase
{
    Task ExecuteAsync(long id);
}
