using MariaAparecida.Retail.Domain.Abstractions;
using MariaAparecida.Retail.Domain.Enums;

namespace MariaAparecida.Retail.Domain.Entities;

public class Venda : IAggregateRoot
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public DateTime DataVenda { get; set; }
    public decimal ValorTotal { get; set; }
    public TipoVendaEnum TipoVenda { get; set; }
    public string? Descricao { get; set; }
    public StatusPagamentoVendaEnum StatusPagamento { get; set; }
    public decimal PercentualRisco { get; set; } = 0;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? DeletadoEm { get; set; }

    // Navigation properties
    public Cliente? Cliente { get; set; }
    public ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
}
