using FluentValidation;
using MediatR;
using MusicMasterShop.Application.Abstractions.Request;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Application.UseCases.CreateUser;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.UseCases.CreateProduct
{
    public record CreateProductRequest(
            string Nome,
            string Descricao,
            string Modelo,
            string Marca,
            string SerialNumber,
            int GarantiaEmDias,
            decimal Preco,
            CreateProductDimensaoRequest Dimensoes,
            TipoCategoria? TipoCategoriaId) : BaseRequest, IRequest<BaseResponse<CreateProductResponse>>
    {
        public override bool IsValid()
        {
            ValidationResult = new CreateProductValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
