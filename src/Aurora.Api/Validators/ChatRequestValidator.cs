using Aurora.Contracts.Chat;
using FluentValidation;

namespace Aurora.Api.Validators;

public class ChatRequestValidator : AbstractValidator<ChatRequest>
{
    public ChatRequestValidator()
    {
        RuleFor(x => x.Message).NotEmpty().WithMessage("Message is required.");
    }
}
