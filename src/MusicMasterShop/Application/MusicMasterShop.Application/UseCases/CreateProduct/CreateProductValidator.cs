using FluentValidation;

namespace MusicMasterShop.Application.UseCases.CreateProduct
{
    public sealed class CreateProductValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage(x => $"Campo {nameof(x.Nome)} não pode ser vazio");

            RuleFor(x => x.Descricao)
                .NotEmpty()
                .WithMessage(x => $"Campo {nameof(x.Descricao)} não pode ser vazio");

            RuleFor(x => x.TipoCategoriaId)
                .NotEmpty()
                .WithMessage(x => $"Campo {nameof(x.TipoCategoriaId)} não pode ser vazio");

            RuleFor(x => x.Preco)
                .GreaterThan(0)
                .WithMessage(x => $"Campo {nameof(x.Preco)} deve ser maior que 0.0");
        }
    }
}
