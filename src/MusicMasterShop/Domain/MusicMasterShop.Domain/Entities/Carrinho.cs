using MusicMasterShop.Domain.Entities.Base;

namespace MusicMasterShop.Domain.Entities
{
    public sealed class Carrinho : BaseEntity
    {
        protected Carrinho(Guid id, bool ativo)
        {
            Ativo = ativo;
            Id = id;
        }

        public ICollection<ValueObjects.CarrinhoProduto> Produtos { get; private set; } = [];
        public bool Ativo { get; set; }
        public Guid UsuarioId { get; private set; }
        public Usuario Usuario { get; private set; } = null!;

        public static Carrinho Create(int quantidade, Produto produto, Usuario usuario)
        {
            var carrinho = new Carrinho(Guid.NewGuid(), true);
            carrinho.SetCreateDate(DateTime.Now);
            carrinho.SetNavigationProperties(
                [ValueObjects.CarrinhoProduto.Create(quantidade, produto)],
                usuario);
            return carrinho;
        }
        public void SetNavigationProperties(
            ICollection<ValueObjects.CarrinhoProduto> produtos,
            Usuario usuario)
        {
            if (produtos != null)
                Produtos = produtos;
            if (usuario != null)
            {
                Usuario = usuario;
                UsuarioId = usuario.Id;
            }
        }

        public void AddOrUpdateProduto(Produto produto, int quantidade)
        {
            ValueObjects.CarrinhoProduto? item = Produtos.FirstOrDefault(
                carrinhoProduto => carrinhoProduto.Produto.Id == produto.Id);

            if (item is null)
            {
                Produtos.Add(ValueObjects.CarrinhoProduto.Create(quantidade, produto));
            }
            else
            {
                item.UpdateQuantidade(quantidade);
            }

            SetUpdateDate(DateTime.Now);
        }

        public void RemoveProduto(Guid produtoId)
        {
            ValueObjects.CarrinhoProduto? item = Produtos.FirstOrDefault(
                carrinhoProduto => carrinhoProduto.Produto.Id == produtoId);

            if (item is null)
                return;

            Produtos.Remove(item);
            SetUpdateDate(DateTime.Now);
        }

        public void Finalizar()
        {
            Ativo = false;
            SetUpdateDate(DateTime.Now);
        }
    }
}
