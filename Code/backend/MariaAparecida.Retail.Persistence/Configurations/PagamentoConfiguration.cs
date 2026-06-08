using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MariaAparecida.Retail.Domain.Entities;

namespace MariaAparecida.Retail.Persistence.Configurations;

public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.VendaId)
            .IsRequired();

        builder.Property(p => p.ClienteId)
            .IsRequired();

        builder.Property(p => p.DataPagamento)
            .IsRequired();

        builder.Property(p => p.ValorPago)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.MetodoPagamento)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.ComprovanteArquivo)
            .HasMaxLength(500);

        builder.Property(p => p.Observacoes)
            .HasMaxLength(500);

        builder.Property(p => p.CriadoEm)
            .HasDefaultValueSql("now() at time zone 'utc'");

        builder.HasIndex(p => p.VendaId);
        builder.HasIndex(p => p.ClienteId);
        builder.HasIndex(p => p.DataPagamento);
        builder.HasIndex(p => p.DeletadoEm);

        // Soft delete filter
        builder.HasQueryFilter(p => p.DeletadoEm == null);

        // Relationships
        builder.HasOne(p => p.Venda)
            .WithMany(v => v.Pagamentos)
            .HasForeignKey(p => p.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Cliente)
            .WithMany(c => c.Pagamentos)
            .HasForeignKey(p => p.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
