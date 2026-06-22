using MusicMasterShop.Domain.Entities.Base;
using MusicMasterShop.Domain.ValueObjects;

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
        public Dimensao Dimensao { get; private set; } = null!;
        public Guid CategoriaId { get; private set; }
        public Categoria Categoria { get; private set; } = null!;

        public static Produto Create(string nome, string descricao, string modelo, string marca, string serialNumber, int? garantiaEmDias,
            decimal preco, Dimensao dimensao, Categoria categoria)
        {
            var produto = new Produto(Guid.NewGuid(), nome, descricao, modelo, marca, serialNumber, garantiaEmDias ?? 7, preco);
            produto.SetCreateDate(DateTime.Now);
            produto.SetNavigationProperties(dimensao, categoria);
            return produto;
        }

        public void Update(string nome, string descricao, string modelo, string marca, string serialNumber, int? garantiaEmDias,
            decimal preco, Dimensao dimensao, Categoria? categoria)
        {
            Nome = nome;
            Descricao = descricao;
            Modelo = modelo;
            Marca = marca;
            SerialNumber = serialNumber;
            GarantiaEmDias = garantiaEmDias ?? 7;
            Preco = preco;
            SetUpdateDate(DateTime.Now);
            SetNavigationProperties(dimensao, categoria!);
        }     

        public void SetNavigationProperties(Dimensao dimensao, Categoria categoria)
        {
            if (dimensao != null)
                Dimensao = dimensao;
            if (categoria != null)
            {
                Categoria = categoria;
                CategoriaId = categoria.Id;
            }
        }
        
        public void AddQtdDisponivel(uint qtd)
        {
            QtdDisponivel = qtd;
        }
    }
}
