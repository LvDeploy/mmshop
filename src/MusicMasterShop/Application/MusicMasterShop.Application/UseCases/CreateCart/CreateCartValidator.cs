using FluentValidation;

namespace MusicMasterShop.Application.UseCases.CreateCart;

public sealed class CreateCartValidator : AbstractValidator<CreateCartRequest>
{
    public CreateCartValidator()
    {
        RuleFor(request => request.Quantidade)
            .GreaterThan(0)
            .WithMessage("Quantidade deve ser maior que zero");

        RuleFor(request => request.ProdutoId)
            .NotEmpty()
            .WithMessage("ProdutoId não pode ser vazio");
    }
}
