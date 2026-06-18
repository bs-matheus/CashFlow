using CashFlow.Application.UseCases.Expenses;
using CashFlow.Communication.Enums;
using CashFlow.Exception.ErrorMessages;
using CommonTestUtilities.Requests;
using Shouldly;

namespace Validators.Test.Expenses;

[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters")]
public class ExpenseValidatorTest
{
    [Fact]
    public void Success()
    {
        //Arrange
        var validator = new ExpenseValidator();
        var request = RequestExpenseJsonBuilder.Build();

        //Act
        var result = validator.Validate(request);

        //Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Error_Title_Empty(string title)
    {
        //Arrange
        var validator = new ExpenseValidator();
        var request = RequestExpenseJsonBuilder.Build();
        request.Title = title;

        //Act
        var result = validator.Validate(request);

        //Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            c => c.ShouldHaveSingleItem(),
            c => c.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.TITLE_REQUIRED)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Error_Amount_Invalid(decimal amount)
    {
        //Arrange
        var validator = new ExpenseValidator();
        var request = RequestExpenseJsonBuilder.Build();
        request.Amount = amount;

        //Act
        var result = validator.Validate(request);

        //Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            c => c.ShouldHaveSingleItem(),
            c => c.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.AMOUNT_MUST_BE_GREATER_THAN_ZERO)));
    }

    [Fact]
    public void Error_Date_Future()
    {
        //Arrange
        var validator = new ExpenseValidator();
        var request = RequestExpenseJsonBuilder.Build();
        request.Date = DateTime.UtcNow.AddDays(1);

        //Act
        var result = validator.Validate(request);

        //Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            c => c.ShouldHaveSingleItem(),
            c => c.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.EXPENSES_CANNOT_BE_FOR_FUTURE)));
    }

    [Fact]
    public void Error_PaymentType_Invalid()
    {
        //Arrange
        var validator = new ExpenseValidator();
        var request = RequestExpenseJsonBuilder.Build();
        request.PaymentType = (PaymentType)700;

        //Act
        var result = validator.Validate(request);

        //Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            c => c.ShouldHaveSingleItem(),
            c => c.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.PAYMENT_TYPE_INVALID)));
    }
}
