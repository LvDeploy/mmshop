using FluentValidation;

namespace MusicMasterShop.Application.UseCases.CreateProduct
{
    public sealed class CreateProductDimensaoValidator : AbstractValidator<CreateProductDimensaoRequest>
    {
        public CreateProductDimensaoValidator()
        {
            RuleFor(x => x.PesoKg)
                .GreaterThan(0)
                .WithMessage(x => $"Campo {nameof(x.PesoKg)} deve ser maior que 0 Kg");

            RuleFor(x => x.AlturaCm)
                .GreaterThan(0)
                .WithMessage(x => $"Campo {nameof(x.AlturaCm)} deve ser maior que 0 cm");

            RuleFor(x => x.LarguraCm)
                .GreaterThan(0)
                .WithMessage(x => $"Campo {nameof(x.LarguraCm)} deve ser maior que 0 cm");

            RuleFor(x => x.ComprimentoCm)
                .GreaterThan(0)
                .WithMessage(x => $"Campo {nameof(x.ComprimentoCm)} deve ser maior que 0 cm");
        }
    }
}
