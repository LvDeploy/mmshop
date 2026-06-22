using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.Queries.ListProductsPaged
{
    public record ListProductsPagedResponse(
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
