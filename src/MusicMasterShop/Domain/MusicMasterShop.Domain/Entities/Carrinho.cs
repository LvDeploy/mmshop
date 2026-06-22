using MusicMasterShop.Domain.Entities.Base;

namespace MusicMasterShop.Domain.Entities
{
    public sealed class Carrinho : BaseEntity
    {
        protected Carrinho(Guid id, int quantidade, bool ativo)
        {
            Quantidade = quantidade;
            Ativo = ativo;
            Id = id;
        }

        public int Quantidade { get; private set; }
        public ICollection<Produto> Produtos { get; private set; } = [];
        public bool Ativo { get; set; }
        public Guid UsuarioId { get; private set; }
        public Usuario Usuario { get; private set; } = null!;

        public static Carrinho Create(int quantidade, Produto produto, Usuario usuario)
        {
            var carrinho = new Carrinho(Guid.NewGuid(), quantidade, true);
            carrinho.SetCreateDate(DateTime.Now);
            carrinho.SetNavigationProperties(new List<Produto>() { produto }, usuario);
            return carrinho;
        }
        public void SetNavigationProperties(ICollection<Produto> produtos, Usuario usuario)
        {
            if (produtos != null)
                Produtos = produtos;
            if (usuario != null)
            {
                Usuario = usuario;
                UsuarioId = usuario.Id;
            }
        }
    }
}
