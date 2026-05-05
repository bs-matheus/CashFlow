using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Exception.ErrorMessages;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Expenses.DeleteById;

internal class DeleteExpenseUseCase : IDeleteExpenseUseCase
{
    private readonly IExpensesDeleteOnlyRepository _repository;
    private readonly IWorkUnit _workUnit;

    public DeleteExpenseUseCase(IExpensesDeleteOnlyRepository repository,
                                IWorkUnit workUnit)
    {
        _repository = repository;
        _workUnit = workUnit;
    }

    public async Task ExecuteAsync(long id)
    {
        bool successToDelete = await _repository.DeleteByIdAsync(id);

        if (!successToDelete)
            throw new NotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);

        await _workUnit.CommitAsync();
    }
}
