using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services;
using CashFlow.Exception.ErrorMessages;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Expenses.DeleteById;

internal class DeleteExpenseUseCase : IDeleteExpenseUseCase
{
    private readonly IExpensesDeleteOnlyRepository _deleteOnlyRepository;
    private readonly IExpensesReadOnlyRepository _readOnlyRepository;
    private readonly IWorkUnit _workUnit;
    private readonly ILoggedUser _loggedUser;

    public DeleteExpenseUseCase(IExpensesDeleteOnlyRepository deleteOnlyRepository,
                                IExpensesReadOnlyRepository readOnlyRepository,
                                IWorkUnit workUnit,
                                ILoggedUser loggedUser)
    {
        _deleteOnlyRepository = deleteOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _workUnit = workUnit;
        _loggedUser = loggedUser;
    }

    public async Task ExecuteAsync(long id)
    {
        var user = await _loggedUser.GetAsync();
        var expense = await _readOnlyRepository.GetByIdAsync(user, id);

        if (expense is null)
            throw new NotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);

        await _deleteOnlyRepository.DeleteByIdAsync(id);

        await _workUnit.CommitAsync();
    }
}
