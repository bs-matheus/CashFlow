using CashFlow.Communication.Requests;
using CashFlow.Exception.ErrorMessages;
using FluentValidation;

namespace CashFlow.Application.UseCases.User.Register;

public class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
{
    public RegisterUserValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(user => user.Name).NotEmpty().WithMessage(ResourceErrorMessages.NAME_CANNOT_BE_EMPTY);

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage(ResourceErrorMessages.EMAIL_CANNOT_BE_EMPTY)
            .EmailAddress().WithMessage(ResourceErrorMessages.EMAIL_INVALID);

        RuleFor(user => user.Password).SetValidator(new PasswordValidator<RequestRegisterUserJson>());
    }
}
