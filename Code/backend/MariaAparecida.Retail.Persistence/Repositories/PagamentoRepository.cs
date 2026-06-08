using Microsoft.EntityFrameworkCore;
using MariaAparecida.Retail.Application.Repositories;
using MariaAparecida.Retail.Domain.Entities;
using MariaAparecida.Retail.Domain.Enums;
using MariaAparecida.Retail.Persistence.Data;

namespace MariaAparecida.Retail.Persistence.Repositories;

public class PagamentoRepository : IPagamentoRepository
{
    private readonly MariaAparecidaDbContext _context;

    public PagamentoRepository(MariaAparecidaDbContext context)
    {
        _context = context;
    }

    public async Task<Pagamento?> GetByIdAsync(Guid id)
    {
        return await _context.Pagamentos.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Pagamento>> GetByVendaIdAsync(Guid vendaId)
    {
        return await _context.Pagamentos
            .Where(p => p.VendaId == vendaId)
            .OrderBy(p => p.DataPagamento)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pagamento>> GetComFiltrosAsync(
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        int? metodo = null,
        int? statusVenda = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        dataInicio = dataInicio.HasValue ? DateTime.SpecifyKind(dataInicio.Value, DateTimeKind.Utc) : null;
        dataFim = dataFim.HasValue ? DateTime.SpecifyKind(dataFim.Value, DateTimeKind.Utc) : null;

        var query = _context.Pagamentos
            .Include(p => p.Venda)
            .AsQueryable();

        if (dataInicio.HasValue)
        {
            query = query.Where(p => p.DataPagamento >= dataInicio);
        }

        if (dataFim.HasValue)
        {
            query = query.Where(p => p.DataPagamento <= dataFim);
        }

        if (metodo.HasValue)
        {
            query = query.Where(p => (int)p.MetodoPagamento == metodo);
        }

        if (statusVenda.HasValue)
        {
            query = query.Where(p => p.Venda != null && (int)p.Venda.StatusPagamento == statusVenda);
        }

        return await query
            .OrderByDescending(p => p.DataPagamento)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pagamento>> GetPendentesAsync(int pageNumber = 1, int pageSize = 10)
    {
        return await _context.Pagamentos
            .Include(p => p.Venda)
            .Where(p => p.Venda != null && 
                        p.Venda.StatusPagamento != StatusPagamentoVendaEnum.Pago)
            .OrderByDescending(p => p.DataPagamento)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalPagoByVendaIdAsync(Guid vendaId)
    {
        return await _context.Pagamentos
            .Where(p => p.VendaId == vendaId)
            .SumAsync(p => p.ValorPago);
    }

    public async Task<int> GetTotalAsync()
    {
        return await _context.Pagamentos.CountAsync();
    }

    public async Task AddAsync(Pagamento pagamento)
    {
        _context.Pagamentos.Add(pagamento);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Pagamento pagamento)
    {
        _context.Pagamentos.Update(pagamento);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var pagamento = await _context.Pagamentos.FirstOrDefaultAsync(p => p.Id == id);
        if (pagamento != null)
        {
            pagamento.DeletadoEm = DateTime.UtcNow;
            await UpdateAsync(pagamento);
        }
    }
}
