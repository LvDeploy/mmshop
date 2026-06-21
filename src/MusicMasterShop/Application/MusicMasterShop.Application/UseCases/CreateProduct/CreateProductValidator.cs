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

            RuleFor(x => x.Categoria)
                .NotEmpty()
                .WithMessage(x => $"Campo {nameof(x.Categoria)} não pode ser vazio");

            RuleFor(x => x.Modelo)
                .NotEmpty()
                .WithMessage(x => $"Campo {nameof(x.Modelo)} não pode ser vazio");
           
            RuleFor(x => x.Dimensoes)
                .NotNull().WithMessage(x => $"Campo {nameof(x.Dimensoes)} não pode ser vazio")
                .SetValidator(new CreateProductDimensaoValidator());

            RuleFor(x => x.Preco)
                .GreaterThan(0)
                .WithMessage(x => $"Campo {nameof(x.Preco)} deve ser maior que 0.0");

            RuleFor(x => x.GarantiaEmDias)
                .GreaterThan(0)
                .WithMessage(x => $"Campo {nameof(x.GarantiaEmDias)} deve ser maior que 0 dias");

            RuleFor(x => x.SerialNumber)
               .NotEmpty()
               .WithMessage(x => $"Campo {nameof(x.SerialNumber)} não pode ser vazio");
        }
    }
}
