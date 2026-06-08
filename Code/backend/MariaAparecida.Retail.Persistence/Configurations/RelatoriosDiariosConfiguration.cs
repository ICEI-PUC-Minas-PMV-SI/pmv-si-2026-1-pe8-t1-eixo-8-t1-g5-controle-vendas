using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MariaAparecida.Retail.Domain.Entities;

namespace MariaAparecida.Retail.Persistence.Configurations;

public class RelatoriosDiariosConfiguration : IEntityTypeConfiguration<RelatoriosDiarios>
{
    public void Configure(EntityTypeBuilder<RelatoriosDiarios> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.DataRelatorio)
            .IsRequired();

        builder.HasIndex(r => r.DataRelatorio)
            .IsUnique();

        builder.Property(r => r.TotalRecebimentos)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(r => r.TotalAReceber)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(r => r.TotalInadimplentes)
            .IsRequired();

        builder.Property(r => r.TaxaPadraoPercentual)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(r => r.IndiceConcentracaoPercentual)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(r => r.CriadoEm)
            .HasDefaultValueSql("now() at time zone 'utc'");
    }
}
