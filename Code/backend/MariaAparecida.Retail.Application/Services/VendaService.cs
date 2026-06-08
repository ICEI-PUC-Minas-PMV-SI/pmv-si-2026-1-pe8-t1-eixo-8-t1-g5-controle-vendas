using MariaAparecida.Retail.Application.DTOs;
using MariaAparecida.Retail.Application.Repositories;
using MariaAparecida.Retail.Domain.Entities;
using MariaAparecida.Retail.Domain.Enums;

namespace MariaAparecida.Retail.Application.Services;

public interface IVendaService
{
    Task<VendaDto> RegistrarVendaAsync(CreateVendaDto dto);
    Task<VendaDetailDto> GetVendaComHistoricoPagamentosAsync(Guid id);
    Task<VendaDto> GetVendaAsync(Guid id);
    Task<IEnumerable<VendaDto>> GetVendasAsync(Guid? clienteId = null, DateTime? dataInicio = null, DateTime? dataFim = null, int? tipoVenda = null, int? statusPagamento = null, int pageNumber = 1, int pageSize = 10);
    Task<VendaDto> AtualizarStatusVendaAsync(Guid id, UpdateVendaStatusDto dto);
    Task<VendaDto> UpdateVendaAsync(Guid id, UpdateVendaDto dto);
    Task DeleteVendaAsync(Guid id);
}

public class VendaService : IVendaService
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IClienteService _clienteService;
    private readonly IPagamentoRepository _pagamentoRepository;

    public VendaService(IVendaRepository vendaRepository, IClienteService clienteService, IPagamentoRepository pagamentoRepository)
    {
        _vendaRepository = vendaRepository;
        _clienteService = clienteService;
        _pagamentoRepository = pagamentoRepository;
    }

    public async Task<VendaDto> RegistrarVendaAsync(CreateVendaDto dto)
    {
        if (dto.ValorTotal <= 0)
            throw new ArgumentException("Valor deve ser maior que zero");

        if (dto.ClienteId == Guid.Empty)
            throw new ArgumentException("ClienteId é obrigatório");

        if (!Enum.IsDefined(typeof(TipoVendaEnum), dto.TipoVenda))
            throw new ArgumentException("Tipo de venda inválido");

        var cliente = await _clienteService.GetClienteAsync(dto.ClienteId);
        if (cliente == null)
            throw new KeyNotFoundException("Cliente não encontrado");

        if (dto.TipoVenda == (int)TipoVendaEnum.Fiado)
        {
            await _clienteService.ValidarLimiteCreditoAsync(dto.ClienteId, dto.ValorTotal);
        }

        var venda = new Venda
        {
            Id = Guid.NewGuid(),
            ClienteId = dto.ClienteId,
            DataVenda = DateTime.SpecifyKind(dto.DataVenda, DateTimeKind.Utc),
            ValorTotal = dto.ValorTotal,
            TipoVenda = (TipoVendaEnum)dto.TipoVenda,
            Descricao = dto.Descricao,
            StatusPagamento = dto.TipoVenda == (int)TipoVendaEnum.Dinheiro
                ? StatusPagamentoVendaEnum.Pago
                : StatusPagamentoVendaEnum.Pendente,
            PercentualRisco = CalcularPercentualRisco((StatusInadimplenciaEnum)cliente.StatusInadimplencia),
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };

        await _vendaRepository.AddAsync(venda);

        venda.Cliente = null;
        return await MapToDtoAsync(venda);
    }

    public async Task<VendaDetailDto> GetVendaComHistoricoPagamentosAsync(Guid id)
    {
        var venda = await _vendaRepository.GetByIdWithPagamentosAsync(id);
        if (venda == null)
            throw new KeyNotFoundException("Venda não encontrada");

        var saldoDevedor = CalcularSaldoDevedor(venda);

        return new VendaDetailDto
        {
            Id = venda.Id,
            ClienteId = venda.ClienteId,
            ClienteNome = venda.Cliente?.Nome,
            DataVenda = venda.DataVenda,
            ValorTotal = venda.ValorTotal,
            TipoVenda = (int)venda.TipoVenda,
            Descricao = venda.Descricao,
            StatusPagamento = (int)venda.StatusPagamento,
            SaldoDevedor = saldoDevedor,
            PercentualRisco = venda.PercentualRisco,
            Pagamentos = venda.Pagamentos?.Select(p => new PagamentoDto
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
            }).ToList() ?? new List<PagamentoDto>(),
            CriadoEm = venda.CriadoEm,
            AtualizadoEm = venda.AtualizadoEm
        };
    }

    public async Task<VendaDto> GetVendaAsync(Guid id)
    {
        var venda = await _vendaRepository.GetByIdAsync(id);
        if (venda == null)
            throw new KeyNotFoundException("Venda não encontrada");

        return await MapToDtoAsync(venda);
    }

    public async Task<IEnumerable<VendaDto>> GetVendasAsync(Guid? clienteId = null, DateTime? dataInicio = null, DateTime? dataFim = null, int? tipoVenda = null, int? statusPagamento = null, int pageNumber = 1, int pageSize = 10)
    {
        if (tipoVenda.HasValue && !Enum.IsDefined(typeof(TipoVendaEnum), tipoVenda.Value))
            throw new ArgumentException("Tipo de venda inválido");

        if (statusPagamento.HasValue && !Enum.IsDefined(typeof(StatusPagamentoVendaEnum), statusPagamento.Value))
            throw new ArgumentException("Status de pagamento inválido");

        var vendas = await _vendaRepository.GetComFiltrosAsync(clienteId, dataInicio, dataFim, tipoVenda, statusPagamento, pageNumber, pageSize);
        var dtos = new List<VendaDto>();

        foreach (var venda in vendas)
        {
            dtos.Add(await MapToDtoAsync(venda));
        }

        return dtos;
    }

    public async Task<VendaDto> AtualizarStatusVendaAsync(Guid id, UpdateVendaStatusDto dto)
    {
        var venda = await _vendaRepository.GetByIdAsync(id);
        if (venda == null)
            throw new KeyNotFoundException("Venda não encontrada");

        if (!Enum.IsDefined(typeof(StatusPagamentoVendaEnum), dto.NovoStatus))
            throw new ArgumentException("Status de pagamento inválido");

        venda.StatusPagamento = (StatusPagamentoVendaEnum)dto.NovoStatus;
        venda.AtualizadoEm = DateTime.UtcNow;

        await _vendaRepository.UpdateAsync(venda);

        return await MapToDtoAsync(venda);
    }

    public async Task<VendaDto> UpdateVendaAsync(Guid id, UpdateVendaDto dto)
    {
        var venda = await _vendaRepository.GetByIdWithPagamentosAsync(id);
        if (venda == null)
            throw new KeyNotFoundException("Venda não encontrada");

        var totalPago = venda.Pagamentos?.Sum(p => p.ValorPago) ?? 0m;
        var novoClienteId = dto.ClienteId ?? venda.ClienteId;
        var novoValorTotal = dto.ValorTotal ?? venda.ValorTotal;
        var novoTipoVenda = dto.TipoVenda.HasValue
            ? (TipoVendaEnum)dto.TipoVenda.Value
            : venda.TipoVenda;

        if (novoClienteId == Guid.Empty)
            throw new ArgumentException("ClienteId é obrigatório");

        if (novoValorTotal <= 0)
            throw new ArgumentException("Valor deve ser maior que zero");

        if (!Enum.IsDefined(typeof(TipoVendaEnum), novoTipoVenda))
            throw new ArgumentException("Tipo de venda inválido");

        if (totalPago > novoValorTotal)
            throw new InvalidOperationException("Valor total não pode ser menor que o total já pago");

        if (novoClienteId != venda.ClienteId && totalPago > 0)
            throw new InvalidOperationException("Não é possível alterar o cliente de uma venda com pagamentos registrados");

        var cliente = await _clienteService.GetClienteAsync(novoClienteId);

        if (novoTipoVenda == TipoVendaEnum.Fiado)
        {
            var valorParaValidar = novoValorTotal;

            if (venda.TipoVenda == TipoVendaEnum.Fiado && novoClienteId == venda.ClienteId)
            {
                valorParaValidar = novoValorTotal - venda.ValorTotal;
            }

            if (valorParaValidar > 0)
                await _clienteService.ValidarLimiteCreditoAsync(novoClienteId, valorParaValidar);
        }

        venda.ClienteId = novoClienteId;
        venda.DataVenda = dto.DataVenda.HasValue 
            ? DateTime.SpecifyKind(dto.DataVenda.Value, DateTimeKind.Utc) 
            : venda.DataVenda;
        venda.ValorTotal = novoValorTotal;
        venda.TipoVenda = novoTipoVenda;

        if (dto.Descricao != null)
            venda.Descricao = dto.Descricao;

        venda.PercentualRisco = CalcularPercentualRisco((StatusInadimplenciaEnum)cliente.StatusInadimplencia);
        venda.StatusPagamento = CalcularStatusPagamento(venda.TipoVenda, venda.ValorTotal, totalPago);
        venda.AtualizadoEm = DateTime.UtcNow;

        await _vendaRepository.UpdateAsync(venda);

        venda.Cliente = null;
        return await MapToDtoAsync(venda);
    }

    public async Task DeleteVendaAsync(Guid id)
    {
        var venda = await _vendaRepository.GetByIdAsync(id);
        if (venda == null)
            throw new KeyNotFoundException("Venda não encontrada");

        await _vendaRepository.DeleteAsync(id);
    }

    private async Task<VendaDto> MapToDtoAsync(Venda venda)
    {
        var totalPago = await _pagamentoRepository.GetTotalPagoByVendaIdAsync(venda.Id);
        var saldoDevedor = venda.TipoVenda == TipoVendaEnum.Fiado
            ? Math.Max(venda.ValorTotal - totalPago, 0)
            : 0m;

        return new VendaDto
        {
            Id = venda.Id,
            ClienteId = venda.ClienteId,
            ClienteNome = venda.Cliente?.Nome,
            DataVenda = venda.DataVenda,
            ValorTotal = venda.ValorTotal,
            TipoVenda = (int)venda.TipoVenda,
            Descricao = venda.Descricao,
            StatusPagamento = (int)venda.StatusPagamento,
            SaldoDevedor = saldoDevedor,
            PercentualRisco = venda.PercentualRisco,
            CriadoEm = venda.CriadoEm,
            AtualizadoEm = venda.AtualizadoEm
        };
    }

    private static decimal CalcularSaldoDevedor(Venda venda)
    {
        if (venda.TipoVenda != TipoVendaEnum.Fiado)
            return 0m;

        var totalPago = venda.Pagamentos?.Sum(p => p.ValorPago) ?? 0m;
        return Math.Max(venda.ValorTotal - totalPago, 0);
    }

    private static StatusPagamentoVendaEnum CalcularStatusPagamento(TipoVendaEnum tipoVenda, decimal valorTotal, decimal totalPago)
    {
        if (tipoVenda == TipoVendaEnum.Dinheiro)
            return StatusPagamentoVendaEnum.Pago;

        if (totalPago >= valorTotal)
            return StatusPagamentoVendaEnum.Pago;

        return totalPago > 0
            ? StatusPagamentoVendaEnum.PartialmentePago
            : StatusPagamentoVendaEnum.Pendente;
    }

    private static decimal CalcularPercentualRisco(StatusInadimplenciaEnum status)
    {
        return status switch
        {
            StatusInadimplenciaEnum.Adimplente => 5m,
            StatusInadimplenciaEnum.Atraso => 25m,
            StatusInadimplenciaEnum.Inadimplente => 75m,
            _ => 5m
        };
    }
}
