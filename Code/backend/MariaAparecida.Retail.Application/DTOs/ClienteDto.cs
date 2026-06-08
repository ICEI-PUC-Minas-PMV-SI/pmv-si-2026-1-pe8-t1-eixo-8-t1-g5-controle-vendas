using MariaAparecida.Retail.Domain.Enums;

namespace MariaAparecida.Retail.Application.DTOs;

public class CreateClienteDto
{
    public string Nome { get; set; } = null!;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public decimal LimiteCredito { get; set; } = 0;
}

public class UpdateClienteDto
{
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
}

public class UpdateLimiteCreditoDto
{
    public decimal NovoLimite { get; set; }
}

public class ClienteDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public decimal LimiteCredito { get; set; }
    public int StatusInadimplencia { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}

public class ClienteDetailDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public decimal LimiteCredito { get; set; }
    public decimal SaldoDevedor { get; set; }
    public int StatusInadimplencia { get; set; }
    public int DiasMediaAtraso { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
