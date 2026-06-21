using FluentValidation;

namespace MusicMasterShop.Application.UseCases.CreateUser
{
    public sealed class CreateUserValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(x => $"Campo {nameof(x.Email)} não pode ser vazio")
                .MaximumLength(100)
                .WithMessage(x => $"Campo {nameof(x.Email)} deve ter no máximo 100 caracteres")
                .EmailAddress()
                .WithMessage(x => $"Campo {nameof(x.Email)} é inválido");

            RuleFor(x => x.Senha)
                .NotEmpty()
                .WithMessage(x => $"Campo {nameof(x.Senha)} não pode ser vazio")
                .MinimumLength(6)
                .WithMessage(x => $"Campo {nameof(x.Senha)} deve ter no mínimo 6 caracteres");

            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage(x => $"Campo {nameof(x.Nome)} não pode ser vazio");

            RuleFor(x => x.TipoUsuario)
                .NotNull()
                .WithMessage(x => $"Campo {nameof(x.TipoUsuario)} não pode ser vazio");
        }
    }
}
