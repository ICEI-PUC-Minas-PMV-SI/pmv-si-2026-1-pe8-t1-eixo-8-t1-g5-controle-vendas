using MariaAparecida.Retail.Domain.Abstractions;

namespace MariaAparecida.Retail.Domain.Entities;

public class Usuario : IAggregateRoot
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string SenhaHash { get; set; } = null!;
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
}
