using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MariaAparecida.Retail.Domain.Entities;

namespace MariaAparecida.Retail.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.NomeCompleto)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.SenhaHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(u => u.Ativo)
            .HasDefaultValue(true);

        builder.Property(u => u.CriadoEm)
            .HasDefaultValueSql("now() at time zone 'utc'");

        builder.Property(u => u.AtualizadoEm)
            .HasDefaultValueSql("now() at time zone 'utc'");
    }
}
