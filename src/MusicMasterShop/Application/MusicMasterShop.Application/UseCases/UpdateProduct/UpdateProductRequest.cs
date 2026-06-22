using System.Text.Json.Serialization;
using MediatR;
using MusicMasterShop.Application.Abstractions.Request;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Application.UseCases.CreateProduct;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.UseCases.UpdateProduct;

public sealed record UpdateProductRequest(
    string? Nome,
    string? Descricao,
    string? Modelo,
    string? Marca,
    string? SerialNumber,
    int GarantiaEmDias,
    decimal Preco,
    CreateProductDimensaoRequest? Dimensoes,
    TipoCategoria? TipoCategoriaId)
    : BaseRequest, IRequest<BaseResponse<UpdateProductResponse>>
{
    [JsonIgnore]
    public Guid Id { get; init; }

    public override bool IsValid()
    {
        ValidationResult = new UpdateProductValidator().Validate(this);
        return ValidationResult.IsValid;
    }
}
