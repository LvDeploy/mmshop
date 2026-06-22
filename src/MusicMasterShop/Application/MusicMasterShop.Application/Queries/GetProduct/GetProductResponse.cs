using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.Queries.GetProduct
{
    public record GetProductResponse(
            Guid Id,
            string Nome,
            string Descricao,
            string NotaFiscal,
            decimal Preco,
            uint QtdDisponivel,
            TipoCategoria? Categoria)
    {
    }
}
