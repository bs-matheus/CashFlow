using CashFlow.Application.UseCases.Users.Register;
using CashFlow.Exception.ErrorMessages;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Token;
using Shouldly;

namespace UseCases.Test.Users.Register;

public class RegisterUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        //Arrange
        var request = RequestRegisterUserJsonBuilder.Build();
        var useCase = CreateUseCase();

        //Act
        var result = await useCase.ExecuteAsync(request);

        //Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe(request.Name);
        result.Token.ShouldNotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public async Task Error_Name_Empty()
    {
        //Arrange
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;

        var useCase = CreateUseCase();

        //Act
        var act = async () => await useCase.ExecuteAsync(request);

        //Assert
        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrors().ShouldSatisfyAllConditions(
            c => c.ShouldHaveSingleItem(),
            c => c.ShouldContain(ResourceErrorMessages.NAME_CANNOT_BE_EMPTY));
    }
    
    [Fact]
    public async Task Error_Email_Already_Exist()
    {
        //Arrange
        var request = RequestRegisterUserJsonBuilder.Build();
        var useCase = CreateUseCase(request.Email);

        //Act
        var act = async () => await useCase.ExecuteAsync(request);

        //Assert
        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrors().ShouldSatisfyAllConditions(
            c => c.ShouldHaveSingleItem(),
            c => c.ShouldContain(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED));
    }

    private static RegisterUserUseCase CreateUseCase(string? existentEmail = null)
    {
        var mapper = MapperBuilder.Build();
        var workUnit = WorkUnitBuilder.Build();
        var writeOnlyRepository = UsersWriteOnlyRepositoryBuilder.Build();
        var passwordEncrypter = new PasswordEncrypterBuilder().Build();
        var tokenGenerator = JwtTokenGeneratorBuilder.Build();
        var readOnlyRepository = new UsersReadOnlyRepositoryBuilder();

        if (!string.IsNullOrEmpty(existentEmail))
        {
            readOnlyRepository.ExistActiveUserWithEmail(existentEmail);
        }

        return new RegisterUserUseCase(mapper, passwordEncrypter, readOnlyRepository.Build(), writeOnlyRepository, workUnit, tokenGenerator);
    }
}
