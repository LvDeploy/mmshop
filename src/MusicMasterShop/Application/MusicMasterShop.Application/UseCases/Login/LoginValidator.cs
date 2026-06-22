using FluentValidation;

namespace MusicMasterShop.Application.UseCases.Login;

public sealed class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(x => $"Campo {nameof(x.Email)} não pode ser vazio")
            .EmailAddress()
            .WithMessage(x => $"Campo {nameof(x.Email)} é inválido");

        RuleFor(x => x.Senha)
            .NotEmpty()
            .WithMessage(x => $"Campo {nameof(x.Senha)} não pode ser vazio");
    }
}
