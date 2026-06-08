namespace MariaAparecida.Retail.Application.DTOs;

public class CreatePagamentoDto
{
    public Guid VendaId { get; set; }
    public Guid ClienteId { get; set; }
    public DateTime DataPagamento { get; set; }
    public decimal ValorPago { get; set; }
    public int MetodoPagamento { get; set; }
    public string? ComprovanteArquivo { get; set; }
    public string? Observacoes { get; set; }
}

public class UpdatePagamentoDto
{
    public decimal ValorPago { get; set; }
    public int MetodoPagamento { get; set; }
    public string? ComprovanteArquivo { get; set; }
    public string? Observacoes { get; set; }
}

public class PagamentoDto
{
    public Guid Id { get; set; }
    public Guid VendaId { get; set; }
    public Guid ClienteId { get; set; }
    public DateTime DataPagamento { get; set; }
    public decimal ValorPago { get; set; }
    public int MetodoPagamento { get; set; }
    public string? ComprovanteArquivo { get; set; }
    public string? Observacoes { get; set; }
    public DateTime CriadoEm { get; set; }
}

public class PagamentoDetailDto
{
    public Guid Id { get; set; }
    public Guid VendaId { get; set; }
    public Guid ClienteId { get; set; }
    public DateTime DataPagamento { get; set; }
    public decimal ValorPago { get; set; }
    public int MetodoPagamento { get; set; }
    public string? ComprovanteArquivo { get; set; }
    public string? Observacoes { get; set; }
    public VendaDto? Venda { get; set; }
    public DateTime CriadoEm { get; set; }
}
