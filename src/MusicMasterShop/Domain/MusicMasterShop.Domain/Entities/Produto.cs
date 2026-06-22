using MusicMasterShop.Domain.Entities.Base;
using MusicMasterShop.Domain.ValueObjects;

namespace MusicMasterShop.Domain.Entities
{
    public sealed class Produto : BaseEntity
    {
        protected Produto(Guid id,
            string nome, 
            string descricao, 
            decimal preco)
        {
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
            Id = id;
        }

        public string Nome { get; private set; } = string.Empty;
        public string Descricao { get; private set; } = string.Empty;
        public string NumeroNotaFiscal { get; private set; } = string.Empty;
        public decimal Preco { get; private set; }
        public uint QtdDisponivel { get; private set; }
        public Guid CategoriaId { get; private set; }
        public Categoria Categoria { get; private set; } = null!;

        public static Produto Create(string nome, string descricao, 
            decimal preco, Categoria categoria)
        {
            var produto = new Produto(Guid.NewGuid(), nome, descricao, preco);
            produto.SetCreateDate(DateTime.Now);
            produto.SetNavigationProperties(categoria);
            return produto;
        }

        public void Update(string nome, string descricao, 
            decimal preco, Categoria? categoria)
        {
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
            SetUpdateDate(DateTime.Now);
            SetNavigationProperties(categoria!);
        }     

        public void SetNavigationProperties(Categoria categoria)
        {
            if (categoria != null)
            {
                Categoria = categoria;
                CategoriaId = categoria.Id;
            }
        }
        public void AddNotaFiscal(string numero)
        {
            NumeroNotaFiscal = numero;
        }

        public void AddQtdDisponivel(uint qtd)
        {
            QtdDisponivel = qtd;
        }

        public void RemoveQtdDisponivel(uint quantidade)
        {
            QtdDisponivel -= quantidade;
        }
    }
}
