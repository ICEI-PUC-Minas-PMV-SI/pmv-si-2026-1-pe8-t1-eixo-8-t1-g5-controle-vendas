using MariaAparecida.Retail.Application.DTOs;
using MariaAparecida.Retail.Application.Repositories;
using MariaAparecida.Retail.Domain.Entities;
using MariaAparecida.Retail.Domain.Enums;

namespace MariaAparecida.Retail.Application.Services;

public interface IPagamentoService
{
    Task<PagamentoDto> RegistrarPagamentoAsync(CreatePagamentoDto dto);
    Task<PagamentoDetailDto> GetPagamentoAsync(Guid id);
    Task<IEnumerable<PagamentoDto>> GetPagamentosAsync(DateTime? dataInicio = null, DateTime? dataFim = null, int? metodo = null, int pageNumber = 1, int pageSize = 10);
    Task<IEnumerable<PagamentoDto>> GetPagamentosVendaAsync(Guid vendaId);
    Task<PagamentoDto> UpdatePagamentoAsync(Guid id, UpdatePagamentoDto dto);
    Task ReversarPagamentoAsync(Guid id);
}

public class PagamentoService : IPagamentoService
{
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly IVendaRepository _vendaRepository;

    public PagamentoService(IPagamentoRepository pagamentoRepository, IVendaRepository vendaRepository)
    {
        _pagamentoRepository = pagamentoRepository;
        _vendaRepository = vendaRepository;
    }

    public async Task<PagamentoDto> RegistrarPagamentoAsync(CreatePagamentoDto dto)
    {
        if (dto.ValorPago <= 0)
            throw new ArgumentException("Valor do pagamento deve ser maior que zero");

        if (dto.VendaId == Guid.Empty)
            throw new ArgumentException("VendaId é obrigatório");

        var venda = await _vendaRepository.GetByIdAsync(dto.VendaId);
        if (venda == null)
            throw new KeyNotFoundException("Venda não encontrada");

        if (venda.TipoVenda != TipoVendaEnum.Fiado)
            throw new InvalidOperationException("Pagamentos só podem ser registrados para vendas a fiado");

        if (venda.StatusPagamento == StatusPagamentoVendaEnum.Cancelado)
            throw new InvalidOperationException("Não é possível registrar pagamento para venda cancelada");

        if (dto.ClienteId != venda.ClienteId)
            throw new ArgumentException("ClienteId do pagamento deve corresponder ao cliente da venda");

        if (!Enum.IsDefined(typeof(MetodoPagamentoEnum), dto.MetodoPagamento))
            throw new ArgumentException("Método de pagamento inválido");

        var totalPago = await _pagamentoRepository.GetTotalPagoByVendaIdAsync(dto.VendaId);
        var saldoPendente = venda.ValorTotal - totalPago;

        if (dto.ValorPago > saldoPendente)
            throw new InvalidOperationException($"Valor do pagamento não pode exceder o saldo pendente de {saldoPendente:C}");

        var pagamento = new Pagamento
        {
            Id = Guid.NewGuid(),
            VendaId = dto.VendaId,
            ClienteId = dto.ClienteId,
            DataPagamento = DateTime.SpecifyKind(dto.DataPagamento, DateTimeKind.Utc),
            ValorPago = dto.ValorPago,
            MetodoPagamento = (MetodoPagamentoEnum)dto.MetodoPagamento,
            ComprovanteArquivo = dto.ComprovanteArquivo,
            Observacoes = dto.Observacoes,
            CriadoEm = DateTime.UtcNow
        };

        await _pagamentoRepository.AddAsync(pagamento);

        await AtualizarStatusVendaAsync(venda, totalPago + dto.ValorPago);

        return new PagamentoDto
        {
            Id = pagamento.Id,
            VendaId = pagamento.VendaId,
            ClienteId = pagamento.ClienteId,
            DataPagamento = pagamento.DataPagamento,
            ValorPago = pagamento.ValorPago,
            MetodoPagamento = (int)pagamento.MetodoPagamento,
            ComprovanteArquivo = pagamento.ComprovanteArquivo,
            Observacoes = pagamento.Observacoes,
            CriadoEm = pagamento.CriadoEm
        };
    }

    public async Task<PagamentoDetailDto> GetPagamentoAsync(Guid id)
    {
        var pagamento = await _pagamentoRepository.GetByIdAsync(id);
        if (pagamento == null)
            throw new KeyNotFoundException("Pagamento não encontrado");

        return new PagamentoDetailDto
        {
            Id = pagamento.Id,
            VendaId = pagamento.VendaId,
            ClienteId = pagamento.ClienteId,
            DataPagamento = pagamento.DataPagamento,
            ValorPago = pagamento.ValorPago,
            MetodoPagamento = (int)pagamento.MetodoPagamento,
            ComprovanteArquivo = pagamento.ComprovanteArquivo,
            Observacoes = pagamento.Observacoes,
            CriadoEm = pagamento.CriadoEm
        };
    }

    public async Task<IEnumerable<PagamentoDto>> GetPagamentosAsync(DateTime? dataInicio = null, DateTime? dataFim = null, int? metodo = null, int pageNumber = 1, int pageSize = 10)
    {
        var pagamentos = await _pagamentoRepository.GetComFiltrosAsync(dataInicio, dataFim, metodo, pageNumber: pageNumber, pageSize: pageSize);

        return pagamentos.Select(p => new PagamentoDto
        {
            Id = p.Id,
            VendaId = p.VendaId,
            ClienteId = p.ClienteId,
            DataPagamento = p.DataPagamento,
            ValorPago = p.ValorPago,
            MetodoPagamento = (int)p.MetodoPagamento,
            ComprovanteArquivo = p.ComprovanteArquivo,
            Observacoes = p.Observacoes,
            CriadoEm = p.CriadoEm
        });
    }

    public async Task<IEnumerable<PagamentoDto>> GetPagamentosVendaAsync(Guid vendaId)
    {
        var pagamentos = await _pagamentoRepository.GetByVendaIdAsync(vendaId);

        return pagamentos.Select(p => new PagamentoDto
        {
            Id = p.Id,
            VendaId = p.VendaId,
            ClienteId = p.ClienteId,
            DataPagamento = p.DataPagamento,
            ValorPago = p.ValorPago,
            MetodoPagamento = (int)p.MetodoPagamento,
            ComprovanteArquivo = p.ComprovanteArquivo,
            Observacoes = p.Observacoes,
            CriadoEm = p.CriadoEm
        });
    }

    public async Task<PagamentoDto> UpdatePagamentoAsync(Guid id, UpdatePagamentoDto dto)
    {
        var pagamento = await _pagamentoRepository.GetByIdAsync(id);
        if (pagamento == null)
            throw new KeyNotFoundException("Pagamento não encontrado");

        if (dto.ValorPago <= 0)
            throw new ArgumentException("Valor do pagamento deve ser maior que zero");

        if (!Enum.IsDefined(typeof(MetodoPagamentoEnum), dto.MetodoPagamento))
            throw new ArgumentException("Método de pagamento inválido");

        var venda = await _vendaRepository.GetByIdAsync(pagamento.VendaId);
        if (venda == null)
            throw new KeyNotFoundException("Venda não encontrada");

        if (venda.TipoVenda != TipoVendaEnum.Fiado)
            throw new InvalidOperationException("Pagamentos só podem ser atualizados para vendas a fiado");

        var totalPagoAtual = await _pagamentoRepository.GetTotalPagoByVendaIdAsync(pagamento.VendaId);
        var totalPagoOutrosPagamentos = totalPagoAtual - pagamento.ValorPago;
        var novoTotalPago = totalPagoOutrosPagamentos + dto.ValorPago;

        if (novoTotalPago > venda.ValorTotal)
        {
            var saldoDisponivel = venda.ValorTotal - totalPagoOutrosPagamentos;
            throw new InvalidOperationException($"Valor do pagamento não pode exceder o saldo pendente de {saldoDisponivel:C}");
        }

        pagamento.ValorPago = dto.ValorPago;
        pagamento.MetodoPagamento = (MetodoPagamentoEnum)dto.MetodoPagamento;
        pagamento.ComprovanteArquivo = dto.ComprovanteArquivo;
        pagamento.Observacoes = dto.Observacoes;

        await _pagamentoRepository.UpdateAsync(pagamento);
        await AtualizarStatusVendaAsync(venda, novoTotalPago);

        return new PagamentoDto
        {
            Id = pagamento.Id,
            VendaId = pagamento.VendaId,
            ClienteId = pagamento.ClienteId,
            DataPagamento = pagamento.DataPagamento,
            ValorPago = pagamento.ValorPago,
            MetodoPagamento = (int)pagamento.MetodoPagamento,
            ComprovanteArquivo = pagamento.ComprovanteArquivo,
            Observacoes = pagamento.Observacoes,
            CriadoEm = pagamento.CriadoEm
        };
    }

    public async Task ReversarPagamentoAsync(Guid id)
    {
        var pagamento = await _pagamentoRepository.GetByIdAsync(id);
        if (pagamento == null)
            throw new KeyNotFoundException("Pagamento não encontrado");

        // Get venda to update status
        var venda = await _vendaRepository.GetByIdAsync(pagamento.VendaId);
        if (venda != null)
        {
            await _pagamentoRepository.DeleteAsync(id);
            
            var remainingTotal = await _pagamentoRepository.GetTotalPagoByVendaIdAsync(venda.Id);
            await AtualizarStatusVendaAsync(venda, remainingTotal);
        }
        else
        {
            await _pagamentoRepository.DeleteAsync(id);
        }
    }

    private async Task AtualizarStatusVendaAsync(Venda venda, decimal totalPago)
    {
        if (venda.TipoVenda == TipoVendaEnum.Dinheiro)
        {
            venda.StatusPagamento = StatusPagamentoVendaEnum.Pago;
        }
        else if (totalPago >= venda.ValorTotal)
        {
            venda.StatusPagamento = StatusPagamentoVendaEnum.Pago;
        }
        else if (totalPago > 0)
        {
            venda.StatusPagamento = StatusPagamentoVendaEnum.PartialmentePago;
        }
        else
        {
            venda.StatusPagamento = StatusPagamentoVendaEnum.Pendente;
        }

        venda.AtualizadoEm = DateTime.UtcNow;
        await _vendaRepository.UpdateAsync(venda);
    }
}
