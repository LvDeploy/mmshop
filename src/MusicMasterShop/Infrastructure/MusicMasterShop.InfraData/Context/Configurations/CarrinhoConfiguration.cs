using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicMasterShop.Domain.Entities;

namespace MusicMasterShop.InfraData.Context.Configurations;

internal sealed class CarrinhoConfiguration : IEntityTypeConfiguration<Carrinho>
{
    public void Configure(EntityTypeBuilder<Carrinho> builder)
    {
        builder.HasKey(carrinho => carrinho.Id);

        builder
            .HasOne(carrinho => carrinho.Usuario)
            .WithMany()
            .HasForeignKey(carrinho => carrinho.UsuarioId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder
           .HasMany(carrinho => carrinho.Produtos)
           .WithMany()
           .UsingEntity<Dictionary<string, object>>(
               "CarrinhoProdutos",
               right => right
                   .HasOne<Produto>()
                   .WithMany()
                   .HasForeignKey("ProdutoId")
                   .OnDelete(DeleteBehavior.Restrict),
               left => left
                   .HasOne<Carrinho>()
                   .WithMany()
                   .HasForeignKey("CarrinhoId")
                   .OnDelete(DeleteBehavior.Cascade),
               join => join.HasKey("CarrinhoId", "ProdutoId"));
    }
}
