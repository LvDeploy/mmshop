using MusicMasterShop.Domain.Enums;
using MusicMasterShop.Domain.ValueObjects;

namespace MusicMasterShop.Application.Queries.GetProduct
{
    public record GetProductResponse(
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
