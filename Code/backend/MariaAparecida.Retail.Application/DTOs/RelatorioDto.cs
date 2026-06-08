namespace MariaAparecida.Retail.Application.DTOs;

public class DashboardKpisDto
{
    public decimal TotalReceber { get; set; }
    public decimal ReceitaMes { get; set; }
    public decimal TicketMedio { get; set; }
    public decimal TaxaInadimplencia { get; set; }
    public decimal TaxaPadraoPercentual { get; set; }
    public decimal PeriodoMedioColeta { get; set; }
    public decimal IndiceConcentracaoPercentual { get; set; }
    public int TotalInadimplentes { get; set; }
    public DateTime DataReferencia { get; set; }
}

public class ReceitaMensalDto
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public string NomeMes { get; set; } = null!;
    public decimal Receita { get; set; }
}

public class TaxaPadraoDto
{
    public decimal TaxaPadraoPercentual { get; set; }
}

public class PeriodoMedioColetaDto
{
    public decimal DiasMedio { get; set; }
}

public class ConcentracaoDto
{
    public decimal IndiceConcentracaoPercentual { get; set; }
    public List<TopClienteDto> TopClientes { get; set; } = new();
}

public class TopClienteDto
{
    public Guid ClienteId { get; set; }
    public string NomeCliente { get; set; } = null!;
    public decimal TotalRecebivelBruto { get; set; }
    public decimal PercentualDoTotal { get; set; }
}

public class InadimplenciaDto
{
    public Guid ClienteId { get; set; }
    public string NomeCliente { get; set; } = null!;
    public decimal TotalDevedor { get; set; }
    public int DiasAtraso { get; set; }
    public int TotalVendasAtrasadas { get; set; }
}

public class RiscoInadimplenciaDto
{
    public string FaixaValor { get; set; } = null!;
    public int TotalVendas { get; set; }
    public int VendasInadimplentes { get; set; }
    public decimal PercentualRisco { get; set; }
}
