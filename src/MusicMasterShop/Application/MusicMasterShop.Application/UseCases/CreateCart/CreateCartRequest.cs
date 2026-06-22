using MediatR;
using MusicMasterShop.Application.Abstractions.Request;
using MusicMasterShop.Application.Abstractions.Response;

namespace MusicMasterShop.Application.UseCases.CreateCart;

public sealed record CreateCartRequest(int Quantidade, Guid ProdutoId)
    : BaseRequest, IRequest<BaseResponse<CreateCartResponse>>
{
    public override bool IsValid()
    {
        ValidationResult = new CreateCartValidator().Validate(this);
        return ValidationResult.IsValid;
    }
}
