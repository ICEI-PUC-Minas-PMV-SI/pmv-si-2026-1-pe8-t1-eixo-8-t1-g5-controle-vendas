using MariaAparecida.Retail.Domain.Abstractions;

namespace MariaAparecida.Retail.Domain.Entities;

public class RelatoriosDiarios : IAggregateRoot
{
    public Guid Id { get; set; }
    public DateTime DataRelatorio { get; set; }
    public decimal TotalRecebimentos { get; set; }
    public decimal TotalAReceber { get; set; }
    public int TotalInadimplentes { get; set; }
    public decimal TaxaPadraoPercentual { get; set; }
    public decimal IndiceConcentracaoPercentual { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
