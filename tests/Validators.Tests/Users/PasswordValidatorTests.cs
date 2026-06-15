using CashFlow.Application.UseCases.Users;
using CashFlow.Communication.Requests;
using FluentValidation;
using Shouldly;

namespace Validators.Tests.Users;

public class PasswordValidatorTests
{
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    [InlineData("a")]
    [InlineData("aa")]
    [InlineData("aaa")]
    [InlineData("aaaa")]
    [InlineData("aaaaa")]
    [InlineData("aaaaaa")]
    [InlineData("aaaaaaa")]
    [InlineData("aaaaaaaa")]
    [InlineData("AAAAAAAA")]
    [InlineData("Aaaaaaaa")]
    [InlineData("Aaaaaaa1")]
    public void Error_Password_Invalid(string password)
    {
        //Arrange
        var validator = new PasswordValidator<RequestRegisterUserJson>();
        var context = new ValidationContext<RequestRegisterUserJson>(new RequestRegisterUserJson());

        //Act
        var result = validator.IsValid(context, password);

        //Assert
        result.ShouldBeFalse();
    }
}
