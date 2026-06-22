using FluentValidation;

namespace MusicMasterShop.Application.UseCases.CreateOrder;

public sealed class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(request => request.CarrinhoId)
            .NotEmpty()
            .WithMessage("CarrinhoId não pode ser vazio");

        RuleFor(request => request.DocumentoCliente)
            .NotEmpty()
            .WithMessage("DocumentoCliente não pode ser vazio");
    }
}
