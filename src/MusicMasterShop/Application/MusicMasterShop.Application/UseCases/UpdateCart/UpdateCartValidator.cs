using FluentValidation;

namespace MusicMasterShop.Application.UseCases.UpdateCart;

public sealed class UpdateCartValidator : AbstractValidator<UpdateCartRequest>
{
    public UpdateCartValidator()
    {
        RuleFor(request => request.CarrinhoId)
            .NotEmpty()
            .WithMessage("CarrinhoId não pode ser vazio");

        RuleFor(request => request.Produtos)
            .NotNull()
            .NotEmpty()
            .WithMessage("Produtos não pode ser vazio");

        RuleForEach(request => request.Produtos)
            .ChildRules(item =>
            {
                item.RuleFor(value => value.ProdutoId)
                    .NotEmpty()
                    .WithMessage("ProdutoId não pode ser vazio");

                item.RuleFor(value => value.Quantidade)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Quantidade deve ser maior ou igual a zero");
            });
    }
}
