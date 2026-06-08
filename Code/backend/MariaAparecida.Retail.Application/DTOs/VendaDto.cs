namespace MariaAparecida.Retail.Application.DTOs;

public class CreateVendaDto
{
    public Guid ClienteId { get; set; }
    public DateTime DataVenda { get; set; }
    public decimal ValorTotal { get; set; }
    public int TipoVenda { get; set; } // 1 = Dinheiro, 2 = Fiado
    public string? Descricao { get; set; }
}

public class UpdateVendaDto
{
    public Guid? ClienteId { get; set; }
    public DateTime? DataVenda { get; set; }
    public decimal? ValorTotal { get; set; }
    public int? TipoVenda { get; set; }
    public string? Descricao { get; set; }
}

public class UpdateVendaStatusDto
{
    public int NovoStatus { get; set; }
}

public class VendaDto
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public DateTime DataVenda { get; set; }
    public decimal ValorTotal { get; set; }
    public int TipoVenda { get; set; }
    public string? Descricao { get; set; }
    public int StatusPagamento { get; set; }
    public decimal SaldoDevedor { get; set; }
    public decimal PercentualRisco { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}

public class VendaDetailDto
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public DateTime DataVenda { get; set; }
    public decimal ValorTotal { get; set; }
    public int TipoVenda { get; set; }
    public string? Descricao { get; set; }
    public int StatusPagamento { get; set; }
    public decimal SaldoDevedor { get; set; }
    public decimal PercentualRisco { get; set; }
    public List<PagamentoDto> Pagamentos { get; set; } = new();
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
