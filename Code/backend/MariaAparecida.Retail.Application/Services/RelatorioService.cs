using MariaAparecida.Retail.Application.DTOs;
using MariaAparecida.Retail.Application.Repositories;
using MariaAparecida.Retail.Domain.Enums;
using System.Globalization;

namespace MariaAparecida.Retail.Application.Services;

public interface IRelatorioService
{
    Task<decimal> CalcularTotalRecebiveisAsync(DateTime? inicio = null, DateTime? fim = null);
    Task<decimal> CalcularTaxaPadraoAsync(DateTime? inicio = null, DateTime? fim = null);
    Task<int> CalcularPeriodoMedioColetaAsync(DateTime? inicio = null, DateTime? fim = null);
    Task<decimal> CalcularIndiceConcentracaoAsync(DateTime? inicio = null, DateTime? fim = null);
    Task<IEnumerable<(Guid clienteId, string nome, decimal valor)>> ListarClientesInadimplentesAsync(int diasAtraso = 30);
    Task<decimal> AnalisarRiscoValorAsync(DateTime? inicio = null, DateTime? fim = null);
    Task<DashboardKpisDto> CalcularDashboardKpisAsync(DateTime? inicio = null, DateTime? fim = null);
    Task<IEnumerable<(Guid clienteId, string nome, decimal total)>> GetRecebivelPorClienteAsync(DateTime? inicio = null, DateTime? fim = null);
    Task<decimal> CalcularTotalReceitaAsync(DateTime? inicio = null, DateTime? fim = null);
    Task<int> GetTotalClientesAsync();
    Task<IEnumerable<ReceitaMensalDto>> GetReceitaMensalAsync(int meses = 6);
}

public class RelatorioService : IRelatorioService
{
    private readonly IRelatorioRepository _repository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IVendaRepository _vendaRepository;

    public RelatorioService(
        IRelatorioRepository repository,
        IClienteRepository clienteRepository,
        IVendaRepository vendaRepository)
    {
        _repository = repository;
        _clienteRepository = clienteRepository;
        _vendaRepository = vendaRepository;
    }

    public async Task<decimal> CalcularTotalRecebiveisAsync(DateTime? inicio = null, DateTime? fim = null)
    {
        var total = await _repository.GetTotalRecebiveisAsync(inicio, fim);
        return total;
    }

    public async Task<decimal> CalcularTaxaPadraoAsync(DateTime? inicio = null, DateTime? fim = null)
    {
        var vendas = await _vendaRepository.GetComFiltrosAsync(null, inicio, fim, pageSize: int.MaxValue);
        if (!vendas.Any())
            return 0m;

        var totalVendas = vendas.Sum(v => v.ValorTotal);
        var vendasPago = vendas.Where(v => v.StatusPagamento == StatusPagamentoVendaEnum.Pago).Sum(v => v.ValorTotal);

        var taxa = totalVendas > 0 ? (vendasPago / totalVendas) * 100 : 0m;
        return taxa;
    }

    public async Task<int> CalcularPeriodoMedioColetaAsync(DateTime? inicio = null, DateTime? fim = null)
    {
        var vendas = await _vendaRepository.GetComFiltrosAsync(null, inicio, fim, (int)TipoVendaEnum.Fiado, pageSize: int.MaxValue);
        if (!vendas.Any())
            return 0;

        // Calculate average days from sale date to payment (assuming today for unpaid)
        var diasLista = new List<int>();
        foreach (var venda in vendas)
        {
            if (venda.StatusPagamento == StatusPagamentoVendaEnum.Pago)
            {
                // This would require tracking payment date, which we don't have in current schema
                // For now, estimate based on current date
                diasLista.Add((int)(DateTime.UtcNow - venda.DataVenda).TotalDays);
            }
            else
            {
                diasLista.Add((int)(DateTime.UtcNow - venda.DataVenda).TotalDays);
            }
        }

        var periodioMedio = diasLista.Any() ? (int)diasLista.Average() : 0;
        return periodioMedio;
    }

    public async Task<decimal> CalcularIndiceConcentracaoAsync(DateTime? inicio = null, DateTime? fim = null)
    {
        var recebivelPorCliente = await _repository.GetRecebivelPorClienteAsync(inicio, fim);
        if (!recebivelPorCliente.Any())
            return 0m;

        var totalRecebivel = recebivelPorCliente.Sum(x => x.total);
        if (totalRecebivel == 0)
            return 0m;

        var hhiPercentual = recebivelPorCliente
            .Sum(x => (decimal)Math.Pow((double)(x.total / totalRecebivel), 2)) * 100;

        return hhiPercentual;
    }

    public async Task<IEnumerable<(Guid clienteId, string nome, decimal valor)>> ListarClientesInadimplentesAsync(int diasAtraso = 30)
    {
        var vendasAtrasadas = await _repository.GetVendasAtrasadasAsync(diasAtraso);
        var clientesInadimplentes = vendasAtrasadas
            .Select(v => new
            {
                Venda = v,
                Saldo = Math.Max(v.ValorTotal - (v.Pagamentos?.Sum(p => p.ValorPago) ?? 0m), 0)
            })
            .Where(v => v.Saldo > 0)
            .GroupBy(v => v.Venda.ClienteId)
            .Select(g => (
                g.Key,
                g.First().Venda.Cliente?.Nome ?? "Desconhecido",
                g.Sum(v => v.Saldo)))
            .ToList();

        return clientesInadimplentes;
    }

    public async Task<decimal> AnalisarRiscoValorAsync(DateTime? inicio = null, DateTime? fim = null)
    {
        var vendas = await _vendaRepository.GetComFiltrosAsync(null, inicio, fim, pageSize: int.MaxValue);
        if (!vendas.Any())
            return 0m;

        // Risk = weighted average of percentual_risco for unpaid vendas
        var vendaEmAtraso = vendas
            .Where(v => v.StatusPagamento == StatusPagamentoVendaEnum.Pendente || 
                        v.StatusPagamento == StatusPagamentoVendaEnum.PartialmentePago)
            .ToList();

        if (!vendaEmAtraso.Any())
            return 0m;

        var risco = vendaEmAtraso.Average(v => v.PercentualRisco);
        return risco;
    }

    public async Task<DashboardKpisDto> CalcularDashboardKpisAsync(DateTime? inicio = null, DateTime? fim = null)
    {
        var totalRecebiveis = await CalcularTotalRecebiveisAsync(inicio, fim);
        var hoje = DateTime.UtcNow;
        var inicioReceita = inicio ?? new DateTime(hoje.Year, hoje.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fimReceita = fim ?? hoje;
        var receitaMes = await CalcularTotalReceitaAsync(inicioReceita, fimReceita);
        var vendasPeriodo = await _vendaRepository.GetComFiltrosAsync(null, inicioReceita, fimReceita, pageSize: int.MaxValue);
        var ticketMedio = vendasPeriodo.Any()
            ? vendasPeriodo.Average(v => v.ValorTotal)
            : 0m;
        var taxaPadrao = await CalcularTaxaPadraoAsync(inicio, fim);
        var periodoMedioColeta = await CalcularPeriodoMedioColetaAsync(inicio, fim);
        var indiceConcentracao = await CalcularIndiceConcentracaoAsync(inicio, fim);
        var clientesInadimplentes = await ListarClientesInadimplentesAsync();
        var totalClientes = await _clienteRepository.GetTotalAsync();
        var totalInadimplentes = clientesInadimplentes.Count();
        var taxaInadimplencia = totalClientes > 0
            ? ((decimal)totalInadimplentes / totalClientes) * 100
            : 0m;

        return new DashboardKpisDto
        {
            TotalReceber = totalRecebiveis,
            ReceitaMes = receitaMes,
            TicketMedio = ticketMedio,
            TaxaInadimplencia = taxaInadimplencia,
            TaxaPadraoPercentual = taxaPadrao,
            PeriodoMedioColeta = periodoMedioColeta,
            IndiceConcentracaoPercentual = indiceConcentracao,
            TotalInadimplentes = totalInadimplentes,
            DataReferencia = DateTime.UtcNow
        };
    }

    public async Task<IEnumerable<(Guid clienteId, string nome, decimal total)>> GetRecebivelPorClienteAsync(DateTime? inicio = null, DateTime? fim = null)
    {
        return await _repository.GetRecebivelPorClienteAsync(inicio, fim);
    }

    public async Task<decimal> CalcularTotalReceitaAsync(DateTime? inicio = null, DateTime? fim = null)
    {
        return await _repository.GetTotalReceitaAsync(inicio, fim);
    }

    public async Task<int> GetTotalClientesAsync()
    {
        return await _clienteRepository.GetTotalAsync();
    }

    public async Task<IEnumerable<ReceitaMensalDto>> GetReceitaMensalAsync(int meses = 6)
    {
        var totalMeses = Math.Clamp(meses, 1, 24);
        var hoje = DateTime.UtcNow;
        var inicio = new DateTime(hoje.Year, hoje.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-(totalMeses - 1));
        var fim = new DateTime(hoje.Year, hoje.Month, DateTime.DaysInMonth(hoje.Year, hoje.Month), 23, 59, 59, DateTimeKind.Utc);
        var cultura = new CultureInfo("pt-BR");
        var receitas = (await _repository.GetReceitaMensalAsync(inicio, fim))
            .ToDictionary(x => (x.ano, x.mes), x => x.receita);

        return Enumerable.Range(0, totalMeses)
            .Select(offset =>
            {
                var data = inicio.AddMonths(offset);
                var chave = (data.Year, data.Month);

                return new ReceitaMensalDto
                {
                    Ano = data.Year,
                    Mes = data.Month,
                    NomeMes = cultura.DateTimeFormat.GetAbbreviatedMonthName(data.Month),
                    Receita = receitas.TryGetValue(chave, out var receita) ? receita : 0m
                };
            })
            .ToList();
    }
}
