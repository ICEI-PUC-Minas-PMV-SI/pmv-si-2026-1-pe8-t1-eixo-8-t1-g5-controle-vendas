using Microsoft.EntityFrameworkCore;
using MariaAparecida.Retail.Application.Repositories;
using MariaAparecida.Retail.Domain.Entities;
using MariaAparecida.Retail.Domain.Enums;
using MariaAparecida.Retail.Persistence.Data;

namespace MariaAparecida.Retail.Persistence.Repositories;

public class RelatorioRepository : IRelatorioRepository
{
    private readonly MariaAparecidaDbContext _context;

    public RelatorioRepository(MariaAparecidaDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetTotalRecebiveisAsync(DateTime? inicio = null, DateTime? fim = null)
    {
        inicio = inicio.HasValue ? DateTime.SpecifyKind(inicio.Value, DateTimeKind.Utc) : null;
        fim = fim.HasValue ? DateTime.SpecifyKind(fim.Value, DateTimeKind.Utc) : null;

        var vendas = await _context.Vendas
            .Where(v => v.TipoVenda == TipoVendaEnum.Fiado &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Pago &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Cancelado &&
                        (inicio == null || v.DataVenda >= inicio) &&
                        (fim == null || v.DataVenda <= fim))
            .Select(v => new { v.Id, v.ValorTotal })
            .ToListAsync();

        var vendaIds = vendas.Select(v => v.Id).ToList();

        var totalPagos = await _context.Pagamentos
            .Where(p => vendaIds.Contains(p.VendaId))
            .SumAsync(p => p.ValorPago);

        var totalVendas = vendas.Sum(v => v.ValorTotal);
        return Math.Max(totalVendas - totalPagos, 0);
    }

    public async Task<IEnumerable<(Guid clienteId, string nome, decimal total)>> GetRecebivelPorClienteAsync(DateTime? inicio = null, DateTime? fim = null)
    {
        inicio = inicio.HasValue ? DateTime.SpecifyKind(inicio.Value, DateTimeKind.Utc) : null;
        fim = fim.HasValue ? DateTime.SpecifyKind(fim.Value, DateTimeKind.Utc) : null;

        var vendas = await _context.Vendas
            .Where(v => v.TipoVenda == TipoVendaEnum.Fiado &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Pago &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Cancelado &&
                        (inicio == null || v.DataVenda >= inicio) &&
                        (fim == null || v.DataVenda <= fim))
            .Include(v => v.Cliente)
            .Select(v => new { v.Id, v.ClienteId, v.ValorTotal, v.Cliente!.Nome })
            .ToListAsync();

        var vendaIds = vendas.Select(v => v.Id).ToList();

        var pagamentos = await _context.Pagamentos
            .Where(p => vendaIds.Contains(p.VendaId))
            .GroupBy(p => p.VendaId)
            .Select(g => new { VendaId = g.Key, Total = g.Sum(p => p.ValorPago) })
            .ToListAsync();

        var result = vendas
            .GroupBy(v => new { v.ClienteId, v.Nome })
            .Select(g =>
            {
                var vendaIdsForClient = g.Select(v => v.Id).ToList();
                var totalPagos = pagamentos
                    .Where(p => vendaIdsForClient.Contains(p.VendaId))
                    .Sum(p => p.Total);
                var total = g.Sum(v => v.ValorTotal) - totalPagos;
                return (g.Key.ClienteId, g.Key.Nome, total);
            })
            .ToList();

        return result.Where(r => r.total > 0);
    }

    public async Task<IEnumerable<Venda>> GetVendasAtrasadasAsync(int diasAtraso = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-diasAtraso);

        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Pagamentos)
            .Where(v => v.DataVenda <= cutoffDate &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Pago &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Cancelado &&
                        v.TipoVenda == TipoVendaEnum.Fiado)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venda>> GetVendasPorPeriodoAsync(DateTime inicio, DateTime fim, int tipoVenda = 0)
    {
        inicio = DateTime.SpecifyKind(inicio, DateTimeKind.Utc);
        fim = DateTime.SpecifyKind(fim, DateTimeKind.Utc);

        var query = _context.Vendas
            .Where(v => v.DataVenda >= inicio && v.DataVenda <= fim);

        if (tipoVenda > 0)
        {
            var tipo = (TipoVendaEnum)tipoVenda;
            query = query.Where(v => v.TipoVenda == tipo);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<(Guid clienteId, int diasAtraso)>> GetClientesComAtrasoAsync(int diasAtraso = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-diasAtraso);

        var result = await _context.Vendas
            .Where(v => v.DataVenda <= cutoffDate &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Pago &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Cancelado)
            .GroupBy(v => v.ClienteId)
            .Select(g => new
            {
                ClienteId = g.Key,
                DiasAtraso = (int)(DateTime.UtcNow - g.Min(v => v.DataVenda)).TotalDays
            })
            .ToListAsync();

        return result.Select(r => (r.ClienteId, r.DiasAtraso));
    }

    public async Task<decimal> GetTotalPagosAsync(DateTime? inicio = null, DateTime? fim = null)
    {
        inicio = inicio.HasValue ? DateTime.SpecifyKind(inicio.Value, DateTimeKind.Utc) : null;
        fim = fim.HasValue ? DateTime.SpecifyKind(fim.Value, DateTimeKind.Utc) : null;

        return await _context.Pagamentos
            .Where(p => (inicio == null || p.DataPagamento >= inicio) &&
                        (fim == null || p.DataPagamento <= fim))
            .SumAsync(p => p.ValorPago);
    }

    public async Task<decimal> GetTotalReceitaAsync(DateTime? inicio = null, DateTime? fim = null)
    {
        inicio = inicio.HasValue ? DateTime.SpecifyKind(inicio.Value, DateTimeKind.Utc) : null;
        fim = fim.HasValue ? DateTime.SpecifyKind(fim.Value, DateTimeKind.Utc) : null;

        var totalVendasDinheiro = await _context.Vendas
            .Where(v => v.TipoVenda == TipoVendaEnum.Dinheiro &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Cancelado &&
                        (inicio == null || v.DataVenda >= inicio) &&
                        (fim == null || v.DataVenda <= fim))
            .SumAsync(v => v.ValorTotal);

        var totalPagamentosFiado = await _context.Pagamentos
            .Include(p => p.Venda)
            .Where(p => p.Venda != null &&
                        p.Venda.TipoVenda == TipoVendaEnum.Fiado &&
                        (inicio == null || p.DataPagamento >= inicio) &&
                        (fim == null || p.DataPagamento <= fim))
            .SumAsync(p => p.ValorPago);

        return totalVendasDinheiro + totalPagamentosFiado;
    }

    public async Task<IEnumerable<(int ano, int mes, decimal receita)>> GetReceitaMensalAsync(DateTime inicio, DateTime fim)
    {
        inicio = DateTime.SpecifyKind(inicio, DateTimeKind.Utc);
        fim = DateTime.SpecifyKind(fim, DateTimeKind.Utc);

        var vendasDinheiro = await _context.Vendas
            .Where(v => v.TipoVenda == TipoVendaEnum.Dinheiro &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Cancelado &&
                        v.DataVenda >= inicio &&
                        v.DataVenda <= fim)
            .GroupBy(v => new { v.DataVenda.Year, v.DataVenda.Month })
            .Select(g => new
            {
                Ano = g.Key.Year,
                Mes = g.Key.Month,
                Receita = g.Sum(v => v.ValorTotal)
            })
            .ToListAsync();

        var pagamentosFiado = await _context.Pagamentos
            .Include(p => p.Venda)
            .Where(p => p.Venda != null &&
                        p.Venda.TipoVenda == TipoVendaEnum.Fiado &&
                        p.DataPagamento >= inicio &&
                        p.DataPagamento <= fim)
            .GroupBy(p => new { p.DataPagamento.Year, p.DataPagamento.Month })
            .Select(g => new
            {
                Ano = g.Key.Year,
                Mes = g.Key.Month,
                Receita = g.Sum(p => p.ValorPago)
            })
            .ToListAsync();

        return vendasDinheiro
            .Concat(pagamentosFiado)
            .GroupBy(x => new { x.Ano, x.Mes })
            .Select(g => (g.Key.Ano, g.Key.Mes, g.Sum(x => x.Receita)))
            .OrderBy(x => x.Ano)
            .ThenBy(x => x.Mes)
            .ToList();
    }
}
