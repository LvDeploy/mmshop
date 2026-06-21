using System.Text.Json.Serialization;
using MediatR;
using MusicMasterShop.Application.Abstractions.Request;
using MusicMasterShop.Application.Abstractions.Response;

namespace MusicMasterShop.Application.UseCases.UpdateProductStorageCount;

public sealed record UpdateProductStorageCountRequest(int Quantidade)
    : BaseRequest, IRequest<BaseResponse<UpdateProductStorageCountResponse>>
{
    [JsonIgnore]
    public Guid Id { get; init; }

    public override bool IsValid()
    {
        ValidationResult = new UpdateProductStorageCountValidator().Validate(this);
        return ValidationResult.IsValid;
    }
}
