using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Users;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Exception.ErrorMessages;
using CashFlow.Exception.ExceptionsBase;
using FluentValidation.Results;

namespace CashFlow.Application.UseCases.Users.Register;

public class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IMapper _mapper;
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IUsersReadOnlyRepository _readOnlyRepository;
    private readonly IUsersWriteOnlyRepository _writeOnlyRepository;
    private readonly IWorkUnit _workUnit;
    private readonly IAccessTokenGenerator _tokenGenerator;

    public RegisterUserUseCase(IMapper mapper,
                               IPasswordEncrypter passwordEncrypter,
                               IUsersReadOnlyRepository readOnlyRepository,
                               IUsersWriteOnlyRepository writeOnlyRepository,
                               IWorkUnit workUnit,
                               IAccessTokenGenerator tokenGenerator)
    {
        _mapper = mapper;
        _passwordEncrypter = passwordEncrypter;
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _workUnit = workUnit;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<ResponseRegisteredUserJson> ExecuteAsync(RequestRegisterUserJson request)
    {
        await ValidateAsync(request);

        var user = _mapper.Map<User>(request);
        user.Password = _passwordEncrypter.Encrypt(request.Password);
        user.UserIdentifier = Guid.NewGuid();

        await _writeOnlyRepository.AddAsync(user);

        await _workUnit.CommitAsync();

        return new ResponseRegisteredUserJson
        {
            Name = user.Name,
            Token = _tokenGenerator.Generate(user)
        };
    }

    private async Task ValidateAsync(RequestRegisterUserJson request)
    {
        var validator = new RegisterUserValidator();
        var result = validator.Validate(request);

        bool emailExist = await _readOnlyRepository.ExistActiveUserWithEmailAsync(request.Email);
        if (emailExist)
        {
            result.Errors.Add(new ValidationFailure(nameof(request.Email), ResourceErrorMessages.EMAIL_ALREADY_REGISTERED));
        }

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
