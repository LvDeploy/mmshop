using MusicMasterShop.Domain.Enums;
using MusicMasterShop.Domain.ValueObjects;

namespace MusicMasterShop.Application.Queries.GetProduct
{
    public record GetProductResponse(string Nome,
            string Descricao,
            string Modelo,
            string Marca,
            string SerialNumber,
            int GarantiaEmDias,
            decimal Preco,
            TipoCategoria? Categoria,
            Dimensao Dimensao)
    {
    }
}
