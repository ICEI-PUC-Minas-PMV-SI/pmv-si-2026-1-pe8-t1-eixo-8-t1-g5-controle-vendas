using Microsoft.EntityFrameworkCore;
using MariaAparecida.Retail.Application.Repositories;
using MariaAparecida.Retail.Domain.Entities;
using MariaAparecida.Retail.Domain.Enums;
using MariaAparecida.Retail.Persistence.Data;

namespace MariaAparecida.Retail.Persistence.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly MariaAparecidaDbContext _context;

    public ClienteRepository(MariaAparecidaDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> GetByIdAsync(Guid id)
    {
        return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cliente?> GetByEmailAsync(string email)
    {
        return await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<IEnumerable<Cliente>> GetWithFiltersAsync(
        string? nomeFiltro = null,
        int? statusInadimplencia = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = _context.Clientes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nomeFiltro))
        {
            query = query.Where(c => c.Nome.Contains(nomeFiltro));
        }

        if (statusInadimplencia.HasValue)
        {
            var status = (StatusInadimplenciaEnum)statusInadimplencia;
            query = query.Where(c => c.StatusInadimplencia == status);
        }

        return await query
            .OrderBy(c => c.Nome)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<decimal> CalcularSaldoDevedorAsync(Guid clienteId)
    {
        var vendas = await _context.Vendas
            .Where(v => v.ClienteId == clienteId &&
                        v.TipoVenda == TipoVendaEnum.Fiado &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Cancelado)
            .Select(v => new
            {
                v.Id,
                v.ValorTotal
            })
            .ToListAsync();

        decimal totalVendas = vendas.Sum(v => v.ValorTotal);

        var pagamentos = await _context.Pagamentos
            .Where(p => vendas.Select(v => v.Id).Contains(p.VendaId))
            .SumAsync(p => p.ValorPago);

        return Math.Max(totalVendas - pagamentos, 0);
    }

    public async Task<int> CountOverdueAsync(Guid clienteId, int diasAtraso = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-diasAtraso);

        return await _context.Vendas
            .Where(v => v.ClienteId == clienteId &&
                        v.DataVenda <= cutoffDate &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Pago &&
                        v.StatusPagamento != StatusPagamentoVendaEnum.Cancelado)
            .CountAsync();
    }

    public async Task<int> CountTotalVendasFiadoAsync(Guid clienteId)
    {
        return await _context.Vendas
            .Where(v => v.ClienteId == clienteId && v.TipoVenda == TipoVendaEnum.Fiado)
            .CountAsync();
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

    public async Task<int> GetTotalAsync()
    {
        return await _context.Clientes.CountAsync();
    }

    public async Task AddAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        cliente.AtualizadoEm = DateTime.UtcNow;
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        if (cliente != null)
        {
            cliente.DeletadoEm = DateTime.UtcNow;
            await UpdateAsync(cliente);
        }
    }
}
