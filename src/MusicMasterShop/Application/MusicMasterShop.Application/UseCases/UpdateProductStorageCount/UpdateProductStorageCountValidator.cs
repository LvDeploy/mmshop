using FluentValidation;

namespace MusicMasterShop.Application.UseCases.UpdateProductStorageCount;

public sealed class UpdateProductStorageCountValidator
    : AbstractValidator<UpdateProductStorageCountRequest>
{
    public UpdateProductStorageCountValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .WithMessage("Id do produto não pode ser vazio");

        RuleFor(request => request.Quantidade)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantidade deve ser maior ou igual a zero");
    }
}
