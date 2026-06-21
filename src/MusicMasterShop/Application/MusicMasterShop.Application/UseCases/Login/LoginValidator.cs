using FluentValidation;

namespace MusicMasterShop.Application.UseCases.Login;

public sealed class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Campo e-mail não pode ser vazio")
            .EmailAddress()
            .WithMessage("Campo e-mail é inválido");

        RuleFor(x => x.Senha)
            .NotEmpty()
            .WithMessage("Campo senha não pode ser vazio");
    }
}
