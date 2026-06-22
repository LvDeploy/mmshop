using MediatR;
using MusicMasterShop.Application.Abstractions.Request;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.UseCases.CreateProduct
{
    public record CreateProductRequest(
            string Nome,
            string Descricao,
            decimal Preco,
            TipoCategoria? TipoCategoriaId) : BaseRequest, IRequest<BaseResponse<CreateProductResponse>>
    {
        public override bool IsValid()
        {
            ValidationResult = new CreateProductValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
