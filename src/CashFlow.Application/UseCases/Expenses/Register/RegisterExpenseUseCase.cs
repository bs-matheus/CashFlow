using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Expenses.Register;

internal class RegisterExpenseUseCase : IRegisterExpenseUseCase
{
    private readonly IExpensesWriteOnlyRepository _repository;
    private readonly IWorkUnit _workUnit;
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;

    public RegisterExpenseUseCase(IExpensesWriteOnlyRepository repository,
                                  IWorkUnit workUnit,
                                  IMapper mapper,
                                  ILoggedUser loggedUser)
    {
        _repository = repository;
        _workUnit = workUnit;
        _mapper = mapper;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseRegisteredExpenseJson> ExecuteAsync(RequestExpenseJson request)
    {
        Validate(request);

        var user = await _loggedUser.GetAsync();

        var entity = _mapper.Map<Expense>(request);
        entity.UserId = user.Id;

        await _repository.AddAsync(entity);

        await _workUnit.CommitAsync();

        return _mapper.Map<ResponseRegisteredExpenseJson>(entity);
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
