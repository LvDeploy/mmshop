using FluentValidation;

namespace MusicMasterShop.Application.UseCases.CreateUser
{
    public sealed class CreateUserValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Campo e-mail não pode ser vazio")
                .MaximumLength(100)
                .WithMessage("Campo e-mail deve ter no máximo 100 caracteres")
                .EmailAddress()
                .WithMessage("Campo e-mail é inválido");

            RuleFor(x => x.Senha)
                .NotEmpty()
                .WithMessage("Campo senha não pode ser vazio")
                .MinimumLength(6)
                .WithMessage("Campo senha deve ter no mínimo 6 caracteres");

            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("Campo nome não pode ser vazio");

            RuleFor(x => x.TipoUsuario)
                .NotNull()
                .WithMessage("Campo tipoUsuario não pode ser vazio");
        }
    }
}
