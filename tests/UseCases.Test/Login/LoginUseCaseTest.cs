using CashFlow.Application.UseCases.Login;
using CashFlow.Domain.Entities;
using CashFlow.Exception.ErrorMessages;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Token;
using Shouldly;

namespace UseCases.Test.Login;

public class LoginUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        //Arrange
        var user = UserBuilder.Build();
        
        var request = RequestLoginJsonBuilder.Build();
        request.Email = user.Email;

        var useCase = CreateUseCase(user, request.Password);

        //Act
        var result = await useCase.ExecuteAsync(request);

        //Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe(user.Name);
        result.Token.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Error_User_Not_Found()
    {
        //Arrange
        var user = UserBuilder.Build();
        var request = RequestLoginJsonBuilder.Build();
        var useCase = CreateUseCase(user, request.Password);

        //Act
        var act = async () => await useCase.ExecuteAsync(request);

        //Assert
        var exception = await act.ShouldThrowAsync<InvalidLoginException>();
        exception.GetErrors().ShouldSatisfyAllConditions(
            c => c.ShouldHaveSingleItem(),
            c => c.ShouldContain(ResourceErrorMessages.EMAIL_OR_PASSWORD_INVALID));
    }

    [Fact]
    public async Task Error_Password_Does_Not_Match()
    {
        //Arrange
        var user = UserBuilder.Build();

        var request = RequestLoginJsonBuilder.Build();
        request.Email = user.Email;

        var useCase = CreateUseCase(user);

        //Act
        var act = async () => await useCase.ExecuteAsync(request);

        //Assert
        var exception = await act.ShouldThrowAsync<InvalidLoginException>();
        exception.GetErrors().ShouldSatisfyAllConditions(
            c => c.ShouldHaveSingleItem(),
            c => c.ShouldContain(ResourceErrorMessages.EMAIL_OR_PASSWORD_INVALID));
    }

    private static LoginUseCase CreateUseCase(User user, string? password = null)
    {
        var tokenGenerator = JwtTokenGeneratorBuilder.Build();
        var passwordEncrypter = new PasswordEncrypterBuilder().Verify(password).Build();
        var readOnlyRepository = new UsersReadOnlyRepositoryBuilder().GetUserByEmail(user).Build();

        return new LoginUseCase(readOnlyRepository, passwordEncrypter, tokenGenerator);
    }
}
