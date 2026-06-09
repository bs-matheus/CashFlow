using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.User.Register;

public interface IRegisterUserUseCase
{
    Task<ResponseRegisteredUserJson> ExecuteAsync(RequestRegisterUserJson request);
}
