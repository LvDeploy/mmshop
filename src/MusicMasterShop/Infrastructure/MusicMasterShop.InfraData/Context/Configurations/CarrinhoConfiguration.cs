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

        builder.OwnsMany(carrinho => carrinho.Produtos, item =>
        {
            item.ToTable("CarrinhoProdutos");

            item.WithOwner()
                .HasForeignKey("CarrinhoId");

            item.Property<int>("Id")
                .ValueGeneratedOnAdd();

            item.HasKey("CarrinhoId", "Id");

            item.Property(carrinhoProduto => carrinhoProduto.Quantidade)
                .IsRequired();

            item.HasOne(carrinhoProduto => carrinhoProduto.Produto)
                .WithMany()
                .HasForeignKey("ProdutoId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
