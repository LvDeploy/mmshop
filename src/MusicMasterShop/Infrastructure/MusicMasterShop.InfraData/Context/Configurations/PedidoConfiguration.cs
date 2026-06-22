using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicMasterShop.Domain.Entities;

namespace MusicMasterShop.InfraData.Context.Configurations;

internal sealed class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.HasKey(pedido => pedido.Id);

        builder
            .HasOne(pedido => pedido.Carrinho)
            .WithOne()
            .HasForeignKey<Pedido>(pedido => pedido.CarrinhoId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
