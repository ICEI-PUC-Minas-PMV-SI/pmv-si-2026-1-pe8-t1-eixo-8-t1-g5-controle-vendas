using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MariaAparecida.Retail.Domain.Entities;

namespace MariaAparecida.Retail.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Email)
            .HasMaxLength(255);

        builder.Property(c => c.Telefone)
            .HasMaxLength(20);

        builder.Property(c => c.Endereco)
            .HasMaxLength(500);

        builder.Property(c => c.LimiteCredito)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(c => c.StatusInadimplencia)
            .HasConversion<int>();

        builder.Property(c => c.CriadoEm)
            .HasDefaultValueSql("now() at time zone 'utc'");

        builder.Property(c => c.AtualizadoEm)
            .HasDefaultValueSql("now() at time zone 'utc'");

        builder.HasIndex(c => c.Nome);
        builder.HasIndex(c => c.Telefone);
        builder.HasIndex(c => c.StatusInadimplencia);
        builder.HasIndex(c => c.DeletadoEm);

        // Soft delete filter
        builder.HasQueryFilter(c => c.DeletadoEm == null);

        // Relationships
        builder.HasMany(c => c.Vendas)
            .WithOne(v => v.Cliente)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Pagamentos)
            .WithOne(p => p.Cliente)
            .HasForeignKey(p => p.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
