using MusicMasterShop.Domain.Entities.Base;
using MusicMasterShop.Domain.Enums;
using System.Text.Json.Serialization;

namespace MusicMasterShop.Domain.Entities
{
    public sealed class Categoria : BaseEntity
    {
        protected Categoria(Guid id, string nome, TipoCategoria tipo)
        {
            Nome = nome;
            Id = id;
            Tipo = tipo;
        }
        public string Nome { get; private set; } = string.Empty;
        public TipoCategoria Tipo { get; private set; }
        [JsonIgnore]
        public Produto Produto { get; set; } 

        public static Categoria Create(TipoCategoria tipo, string nome)
        {
            var categoria = new Categoria(Guid.NewGuid(), nome, tipo);
            categoria.SetCreateDate(DateTime.Now);
            return categoria;
        }
    }
}
