using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicMasterShop.Domain.Entities;

namespace MusicMasterShop.InfraData.Context.Configurations;

internal sealed class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.HasKey(produto => produto.Id);

        builder
            .HasOne(produto => produto.Categoria)
            .WithMany(categoria => categoria.Produtos)
            .HasForeignKey(produto => produto.CategoriaId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
