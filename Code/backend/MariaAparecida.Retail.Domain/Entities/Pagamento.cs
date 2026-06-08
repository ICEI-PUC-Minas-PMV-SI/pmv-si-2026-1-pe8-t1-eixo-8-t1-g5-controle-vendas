using MariaAparecida.Retail.Domain.Abstractions;
using MariaAparecida.Retail.Domain.Enums;

namespace MariaAparecida.Retail.Domain.Entities;

public class Pagamento : IAggregateRoot
{
    public Guid Id { get; set; }
    public Guid VendaId { get; set; }
    public Guid ClienteId { get; set; }
    public DateTime DataPagamento { get; set; }
    public decimal ValorPago { get; set; }
    public MetodoPagamentoEnum MetodoPagamento { get; set; }
    public string? ComprovanteArquivo { get; set; }
    public string? Observacoes { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? DeletadoEm { get; set; }

    // Navigation properties
    public Venda? Venda { get; set; }
    public Cliente? Cliente { get; set; }
}
