using CashFlow.Communication.Requests;

namespace CashFlow.Application.UseCases.Expenses.UpdateById;

public interface IUpdateExpenseUseCase
{
    Task ExecuteAsync(long id, RequestExpenseJson request);
}
