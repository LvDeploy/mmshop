using Microsoft.EntityFrameworkCore;
using MusicMasterShop.Domain.Entities;
using System.Text.Json.Serialization;

namespace MusicMasterShop.Domain.ValueObjects
{
    [Keyless]
    public sealed class Dimensao
    {
        public Dimensao(decimal altura, decimal largura, decimal comprimento, decimal pesoKg)
        {
            Altura = altura;
            Largura = largura;
            Comprimento = comprimento;
            PesoKg = pesoKg;
        }

        public decimal Altura { get; private set; }
        public decimal Largura { get; private set; }
        public decimal Comprimento { get; private set; }
        public decimal PesoKg { get; private set; }
        [JsonIgnore]
        public Produto Produto { get; set; }
    }
}
