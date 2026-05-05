using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Exception.ErrorMessages;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Expenses.UpdateById;

internal class UpdateExpenseUseCase : IUpdateExpenseUseCase
{
    private readonly IExpensesUpdateOnlyRepository _repository;
    private readonly IWorkUnit _workUnit;
    private readonly IMapper _mapper;

    public UpdateExpenseUseCase(IExpensesUpdateOnlyRepository repository,
                                IWorkUnit workUnit,
                                IMapper mapper)
    {
        _repository = repository;
        _workUnit = workUnit;
        _mapper = mapper;
    }

    public async Task ExecuteAsync(long id, RequestExpenseJson request)
    {
        Validate(request);

        var expense = await _repository.GetByIdAsync(id);

        if (expense is null)
            throw new NotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);

        _mapper.Map(request, expense);

        _repository.Update(expense);

        await _workUnit.CommitAsync();
    }

    private void Validate(RequestExpenseJson request)
    {
        var validator = new ExpenseValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
