using Microsoft.EntityFrameworkCore;
using MariaAparecida.Retail.Application.Repositories;
using MariaAparecida.Retail.Domain.Entities;
using MariaAparecida.Retail.Domain.Enums;
using MariaAparecida.Retail.Persistence.Data;

namespace MariaAparecida.Retail.Persistence.Repositories;

public class VendaRepository : IVendaRepository
{
    private readonly MariaAparecidaDbContext _context;

    public VendaRepository(MariaAparecidaDbContext context)
    {
        _context = context;
    }

    public async Task<Venda?> GetByIdAsync(Guid id)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<Venda?> GetByIdWithPagamentosAsync(Guid id)
    {
        return await _context.Vendas
            .Include(v => v.Pagamentos)
            .Include(v => v.Cliente)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Venda>> GetByClienteIdAsync(Guid clienteId, int pageNumber = 1, int pageSize = 10)
    {
        return await _context.Vendas
            .Where(v => v.ClienteId == clienteId)
            .OrderByDescending(v => v.DataVenda)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venda>> GetComFiltrosAsync(
        Guid? clienteId = null,
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        int? tipoVenda = null,
        int? statusPagamento = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        dataInicio = dataInicio.HasValue ? DateTime.SpecifyKind(dataInicio.Value, DateTimeKind.Utc) : null;
        dataFim = dataFim.HasValue ? DateTime.SpecifyKind(dataFim.Value, DateTimeKind.Utc) : null;

        var query = _context.Vendas
            .Include(v => v.Cliente)
            .AsQueryable();

        if (clienteId.HasValue)
        {
            query = query.Where(v => v.ClienteId == clienteId);
        }

        if (dataInicio.HasValue)
        {
            query = query.Where(v => v.DataVenda >= dataInicio);
        }

        if (dataFim.HasValue)
        {
            query = query.Where(v => v.DataVenda <= dataFim);
        }

        if (tipoVenda.HasValue)
        {
            var tipo = (TipoVendaEnum)tipoVenda;
            query = query.Where(v => v.TipoVenda == tipo);
        }

        if (statusPagamento.HasValue)
        {
            var status = (StatusPagamentoVendaEnum)statusPagamento;
            query = query.Where(v => v.StatusPagamento == status);
        }

        return await query
            .OrderByDescending(v => v.DataVenda)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
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
            .OrderBy(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venda>> GetVendasFiadoNaoPagasAsync()
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Pagamentos)
            .Where(v => v.TipoVenda == TipoVendaEnum.Fiado &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Pago &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Cancelado)
            .OrderBy(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalVendasFiadoPendentesAsync(Guid clienteId)
    {
        var vendas = await _context.Vendas
            .Where(v => v.ClienteId == clienteId &&
                        v.TipoVenda == TipoVendaEnum.Fiado &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Pago &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Cancelado)
            .Select(v => new { v.Id, v.ValorTotal })
            .ToListAsync();

        if (!vendas.Any())
            return 0m;

        var vendaIds = vendas.Select(v => v.Id).ToList();
        var totalPago = await _context.Pagamentos
            .Where(p => vendaIds.Contains(p.VendaId))
            .SumAsync(p => p.ValorPago);

        return Math.Max(vendas.Sum(v => v.ValorTotal) - totalPago, 0);
    }

    public async Task AddAsync(Venda venda)
    {
        _context.Vendas.Add(venda);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Venda venda)
    {
        venda.AtualizadoEm = DateTime.UtcNow;
        _context.Vendas.Update(venda);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var venda = await _context.Vendas.FirstOrDefaultAsync(v => v.Id == id);
        if (venda != null)
        {
            venda.DeletadoEm = DateTime.UtcNow;
            await UpdateAsync(venda);
        }
    }
}
