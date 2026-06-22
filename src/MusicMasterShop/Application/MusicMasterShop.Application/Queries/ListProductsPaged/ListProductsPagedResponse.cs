using MusicMasterShop.Domain.Enums;
using MusicMasterShop.Domain.ValueObjects;

namespace MusicMasterShop.Application.Queries.ListProductsPaged
{
    public record ListProductsPagedResponse(
            Guid Id,
            string Nome,
            string Descricao,
            string Modelo,
            string Marca,
            string SerialNumber,
            int GarantiaEmDias,
            decimal Preco,
            uint QtdDisponivel,
            TipoCategoria? Categoria,
            Dimensao Dimensao)
    {
    }
}
