using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MariaAparecida.Retail.Domain.Entities;

namespace MariaAparecida.Retail.Persistence.Configurations;

public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.ClienteId)
            .IsRequired();

        builder.Property(v => v.DataVenda)
            .IsRequired();

        builder.Property(v => v.ValorTotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(v => v.TipoVenda)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(v => v.Descricao)
            .HasMaxLength(500);

        builder.Property(v => v.StatusPagamento)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(v => v.PercentualRisco)
            .HasPrecision(5, 2)
            .HasDefaultValue(0);

        builder.Property(v => v.CriadoEm)
            .HasDefaultValueSql("now() at time zone 'utc'");

        builder.Property(v => v.AtualizadoEm)
            .HasDefaultValueSql("now() at time zone 'utc'");

        builder.HasIndex(v => v.ClienteId);
        builder.HasIndex(v => v.DataVenda);
        builder.HasIndex(v => v.TipoVenda);
        builder.HasIndex(v => v.StatusPagamento);
        builder.HasIndex(v => v.DeletadoEm);

        // Soft delete filter
        builder.HasQueryFilter(v => v.DeletadoEm == null);

        // Relationships
        builder.HasOne(v => v.Cliente)
            .WithMany(c => c.Vendas)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.Pagamentos)
            .WithOne(p => p.Venda)
            .HasForeignKey(p => p.VendaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
