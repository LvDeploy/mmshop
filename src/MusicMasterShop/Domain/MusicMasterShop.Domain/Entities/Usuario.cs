using MusicMasterShop.Domain.Entities.Base;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Domain.Entities
{
    public sealed class Usuario : BaseEntity
    {
        protected Usuario(Guid id, string email, string nome, string senha, TipoUsuario tipo, bool ativo, DateTime createdAt, DateTime? updatedAt)
        {
            Email = email;
            Nome = nome;
            Senha = senha;
            Tipo = tipo;
            Ativo = ativo;
            Id = id;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public string Email { get; private set; } = string.Empty;
        public string Nome { get; private set; } = string.Empty;
        public string Senha { get; private set; } = string.Empty;
        public TipoUsuario Tipo { get; private set; } 
        public bool Ativo { get; private set; }

        public static Usuario Create(string email, string nome, string senha, TipoUsuario tipo)
        {
            return new Usuario(Guid.NewGuid(), email, nome, senha, tipo, true, DateTime.Now, null);
        }

        public static Usuario Update(Guid id, string email, string nome, string senha, TipoUsuario tipo, DateTime createAt)
        {
            return new Usuario(id, email, nome, senha, tipo, true, createAt, DateTime.Now);
        }

        public static Usuario Restore(Guid id, string email, string nome, TipoUsuario tipo, bool ativo, DateTime createAt, DateTime updateAt)
        {
            return new Usuario(id ,email, nome, string.Empty, tipo, ativo, createAt, updateAt);
        }
    }
}
