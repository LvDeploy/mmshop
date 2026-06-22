using MusicMasterShop.Domain.Entities.Base;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Domain.Entities
{
    public sealed class Usuario : BaseEntity
    {
        protected Usuario(Guid id, string email, string nome, string senha, TipoUsuario tipo, bool ativo)
        {
            Email = email;
            Nome = nome;
            Senha = senha;
            Tipo = tipo;
            Ativo = ativo;
            Id = id;
        }

        public string Email { get; private set; } = string.Empty;
        public string Nome { get; private set; } = string.Empty;
        public string Senha { get; private set; } = string.Empty;
        public TipoUsuario Tipo { get; private set; } 
        public bool Ativo { get; private set; }

        public static Usuario Create(string email, string nome, string senha, TipoUsuario tipo)
        {
            var usuario = new Usuario(Guid.NewGuid(), email, nome, senha, tipo, true);
            usuario.SetCreateDate(DateTime.Now);
            return usuario;
        }

        public static Usuario Update(Guid id, string email, string nome, string senha, TipoUsuario tipo, DateTime createAt)
        {
            var usuario = new Usuario(id, email, nome, senha, tipo, true);
            usuario.SetUpdateDate(DateTime.Now);
            return usuario;
        }

        public void SetPassword(string passwordHash)
        {
            Senha = passwordHash;
        }
    }
}
