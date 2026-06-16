using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.Users;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Login;

public class LoginUseCase : ILoginUseCase
{
    private readonly IUsersReadOnlyRepository _repository;
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IAccessTokenGenerator _tokenGenerator;

    public LoginUseCase(IUsersReadOnlyRepository repository,
                        IPasswordEncrypter passwordEncrypter,
                        IAccessTokenGenerator tokenGenerator)
    {
        _repository = repository;
        _passwordEncrypter = passwordEncrypter;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<ResponseRegisteredUserJson> ExecuteAsync(RequestLoginJson request)
    {
        var user = await _repository.GetUserByEmailAsync(request.Email);
        if (user is null)
        {
            throw new InvalidLoginException();
        }

        bool passwordMatch = _passwordEncrypter.Verify(request.Password, user.Password);
        if (!passwordMatch)
        {
            throw new InvalidLoginException();
        }

        return new ResponseRegisteredUserJson
        {
            Name = user.Name,
            Token = _tokenGenerator.Generate(user)
        };
    }
}
