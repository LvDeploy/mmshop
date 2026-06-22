using System.Text.Json.Serialization;
using MediatR;
using MusicMasterShop.Application.Abstractions.Request;
using MusicMasterShop.Application.Abstractions.Response;

namespace MusicMasterShop.Application.UseCases.UpdateCart;

public sealed record UpdateCartRequest(ICollection<UpdateCartItemRequest> Produtos)
    : BaseRequest, IRequest<BaseResponse<UpdateCartResponse>>
{
    [JsonIgnore]
    public Guid CarrinhoId { get; init; }

    public override bool IsValid()
    {
        ValidationResult = new UpdateCartValidator().Validate(this);
        return ValidationResult.IsValid;
    }
}
