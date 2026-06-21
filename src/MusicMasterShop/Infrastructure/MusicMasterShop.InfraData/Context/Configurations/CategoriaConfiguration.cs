using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicMasterShop.Domain.Entities;

namespace MusicMasterShop.InfraData.Context.Configurations;

internal sealed class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.HasKey(categoria => categoria.Id);
    }
}
