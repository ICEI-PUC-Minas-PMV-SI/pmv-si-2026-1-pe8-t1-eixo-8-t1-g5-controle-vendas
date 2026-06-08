using MariaAparecida.Retail.Domain.Abstractions;
using MariaAparecida.Retail.Domain.Enums;

namespace MariaAparecida.Retail.Domain.Entities;

public class Cliente : IAggregateRoot
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public decimal LimiteCredito { get; set; } = 0;
    public StatusInadimplenciaEnum StatusInadimplencia { get; set; } = StatusInadimplenciaEnum.Adimplente;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? DeletadoEm { get; set; }

    // Navigation properties
    public ICollection<Venda> Vendas { get; set; } = new List<Venda>();
    public ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
}
