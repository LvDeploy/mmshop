using MusicMasterShop.Domain.Entities.Base;
using MusicMasterShop.Domain.Enums;
using MusicMasterShop.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MusicMasterShop.Domain.Entities
{
    public sealed class Produto : BaseEntity
    {
        protected Produto(Guid id,
            string nome, 
            string descricao, 
            string modelo, 
            string marca, 
            string serialNumber, 
            int garantiaEmDias, 
            decimal preco)
        {
            Nome = nome;
            Descricao = descricao;
            Modelo = modelo;
            Marca = marca;
            SerialNumber = serialNumber;
            GarantiaEmDias = garantiaEmDias;
            Preco = preco;
            Id = id;
        }

        public string Nome { get; private set; } = string.Empty;
        public string Descricao { get; private set; } = string.Empty;
        public string Modelo { get; private set; } = string.Empty;
        public string Marca { get; private set; } = string.Empty;
        public string SerialNumber { get; private set; } = string.Empty;
        public int GarantiaEmDias { get; private set; } 
        public decimal Preco { get; private set; }
        public uint QtdDisponivel { get; private set; }
        [JsonIgnore]
        public int DimensaoId { get; set; }

        [ForeignKey(nameof(DimensaoId))]
        [InverseProperty(nameof(Dimensao.Produto))]
        public Dimensao Dimensao { get; set; }
        [JsonIgnore]
        public Guid CategoriaId { get; set; }

        [ForeignKey(nameof(CategoriaId))]
        [InverseProperty(nameof(Categoria.Produto))]
        public Categoria Categoria { get; set; }

        public static Produto Create(string nome, string descricao, string modelo, string marca, string serialNumber, int? garantiaEmDias,
            decimal preco, Dimensao dimensao, Categoria categoria)
        {
            var produto = new Produto(Guid.NewGuid(), nome, descricao, modelo, marca, serialNumber, garantiaEmDias ?? 7, preco);
            produto.SetCreateDate(DateTime.Now);
            produto.SetNavigationProperties(dimensao, categoria);
            return produto;
        }

        public static Produto Update(Guid id, string nome, string descricao, string modelo, string marca, string serialNumber, int? garantiaEmDias,
            decimal preco, Dimensao dimensao, Categoria categoria)
        {
            var produto = new Produto(id, nome, descricao, modelo, marca, serialNumber, garantiaEmDias ?? 7, preco);
            produto.SetUpdateDate(DateTime.Now);
            produto.SetNavigationProperties(dimensao, categoria);
            return produto;
        }     

        public void SetNavigationProperties(Dimensao dimensao, Categoria categoria)
        {
            if (dimensao != null)
                Dimensao = dimensao;
            if (categoria != null)
                Categoria = categoria;
        }
        
        public void AddQtdDisponivel(uint qtd)
        {
            QtdDisponivel += qtd;
        }

        public void RemoveQtdDisponivel(uint qtd)
        {
            QtdDisponivel -= qtd;
        }
    }
}
