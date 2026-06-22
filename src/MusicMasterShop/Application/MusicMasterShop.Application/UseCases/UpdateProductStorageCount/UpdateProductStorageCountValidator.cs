using FluentValidation;

namespace MusicMasterShop.Application.UseCases.UpdateProductStorageCount;

public sealed class UpdateProductStorageCountValidator
    : AbstractValidator<UpdateProductStorageCountRequest>
{
    public UpdateProductStorageCountValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(x => $"Campo {x.Id} não pode ser vazio");

        RuleFor(x => x.NumeroNotaFiscal)
            .NotEmpty()
            .WithMessage(x => $"Campo {x.NumeroNotaFiscal} não pode ser vazio");

        RuleFor(x => x.Quantidade)
            .GreaterThanOrEqualTo(0)
            .WithMessage(x => $"Campo {x.Quantidade} deve ser maior ou igual a zero");
    }
}
