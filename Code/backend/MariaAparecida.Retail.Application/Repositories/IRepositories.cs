using MariaAparecida.Retail.Domain.Abstractions;
using MariaAparecida.Retail.Domain.Entities;

namespace MariaAparecida.Retail.Application.Repositories;

public interface IRepository<T> where T : IAggregateRoot
{
}

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByIdAsync(Guid id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> AuthenticateAsync(string email, string senhaHash);
    Task AddAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
}

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> GetByIdAsync(Guid id);
    Task<Cliente?> GetByEmailAsync(string email);
    Task<IEnumerable<Cliente>> GetWithFiltersAsync(
        string? nomeFiltro = null,
        int? statusInadimplencia = null,
        int pageNumber = 1,
        int pageSize = 10);
    Task<decimal> CalcularSaldoDevedorAsync(Guid clienteId);
    Task<int> CountOverdueAsync(Guid clienteId, int diasAtraso = 30);
    Task<int> CountTotalVendasFiadoAsync(Guid clienteId);
    Task<decimal> GetTotalVendasFiadoPendentesAsync(Guid clienteId);
    Task<int> GetTotalAsync();
    Task AddAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(Guid id);
}

public interface IVendaRepository : IRepository<Venda>
{
    Task<Venda?> GetByIdAsync(Guid id);
    Task<Venda?> GetByIdWithPagamentosAsync(Guid id);
    Task<IEnumerable<Venda>> GetByClienteIdAsync(Guid clienteId, int pageNumber = 1, int pageSize = 10);
    Task<IEnumerable<Venda>> GetComFiltrosAsync(
        Guid? clienteId = null,
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        int? tipoVenda = null,
        int? statusPagamento = null,
        int pageNumber = 1,
        int pageSize = 10);
    Task<IEnumerable<Venda>> GetVendasAtrasadasAsync(int diasAtraso = 30);
    Task<IEnumerable<Venda>> GetVendasFiadoNaoPagasAsync();
    Task<decimal> GetTotalVendasFiadoPendentesAsync(Guid clienteId);
    Task AddAsync(Venda venda);
    Task UpdateAsync(Venda venda);
    Task DeleteAsync(Guid id);
}

public interface IPagamentoRepository : IRepository<Pagamento>
{
    Task<Pagamento?> GetByIdAsync(Guid id);
    Task<IEnumerable<Pagamento>> GetByVendaIdAsync(Guid vendaId);
    Task<IEnumerable<Pagamento>> GetComFiltrosAsync(
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        int? metodo = null,
        int? statusVenda = null,
        int pageNumber = 1,
        int pageSize = 10);
    Task<IEnumerable<Pagamento>> GetPendentesAsync(int pageNumber = 1, int pageSize = 10);
    Task<decimal> GetTotalPagoByVendaIdAsync(Guid vendaId);
    Task<int> GetTotalAsync();
    Task AddAsync(Pagamento pagamento);
    Task UpdateAsync(Pagamento pagamento);
    Task DeleteAsync(Guid id);
}

public interface IRelatorioRepository
{
    Task<decimal> GetTotalRecebiveisAsync(DateTime? inicio = null, DateTime? fim = null);
    Task<IEnumerable<(Guid clienteId, string nome, decimal total)>> GetRecebivelPorClienteAsync(DateTime? inicio = null, DateTime? fim = null);
    Task<IEnumerable<Venda>> GetVendasAtrasadasAsync(int diasAtraso = 30);
    Task<IEnumerable<Venda>> GetVendasPorPeriodoAsync(DateTime inicio, DateTime fim, int tipoVenda = 0);
    Task<IEnumerable<(Guid clienteId, int diasAtraso)>> GetClientesComAtrasoAsync(int diasAtraso = 30);
    Task<decimal> GetTotalPagosAsync(DateTime? inicio = null, DateTime? fim = null);
    Task<decimal> GetTotalReceitaAsync(DateTime? inicio = null, DateTime? fim = null);
    Task<IEnumerable<(int ano, int mes, decimal receita)>> GetReceitaMensalAsync(DateTime inicio, DateTime fim);
}
