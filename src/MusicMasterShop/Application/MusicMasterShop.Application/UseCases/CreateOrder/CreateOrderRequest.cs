using MediatR;
using MusicMasterShop.Application.Abstractions.Request;
using MusicMasterShop.Application.Abstractions.Response;

namespace MusicMasterShop.Application.UseCases.CreateOrder;

public sealed record CreateOrderRequest(Guid CarrinhoId, string DocumentoCliente)
    : BaseRequest, IRequest<BaseResponse<CreateOrderResponse>>
{
    public override bool IsValid()
    {
        ValidationResult = new CreateOrderValidator().Validate(this);
        return ValidationResult.IsValid;
    }
}
