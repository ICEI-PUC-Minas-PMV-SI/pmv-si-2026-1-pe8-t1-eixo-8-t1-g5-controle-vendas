using MariaAparecida.Retail.Application.DTOs;
using MariaAparecida.Retail.Application.Repositories;
using MariaAparecida.Retail.Domain.Entities;
using MariaAparecida.Retail.Domain.Enums;

namespace MariaAparecida.Retail.Application.Services;

public interface IClienteService
{
    Task<ClienteDto> CreateClienteAsync(CreateClienteDto dto);
    Task<ClienteDetailDto> GetClienteComSaldoAsync(Guid id);
    Task<ClienteDto> GetClienteAsync(Guid id);
    Task<IEnumerable<ClienteDto>> GetClientesAsync(string? nome = null, int? status = null, int pageNumber = 1, int pageSize = 10);
    Task<ClienteDto> UpdateClienteAsync(Guid id, UpdateClienteDto dto);
    Task<ClienteDto> UpdateLimiteCreditoAsync(Guid id, decimal novoLimite);
    Task DeleteClienteAsync(Guid id);
    Task ValidarLimiteCreditoAsync(Guid clienteId, decimal novaVendaValor);
    Task<int> GetTotalAsync();
}

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClienteDto> CreateClienteAsync(CreateClienteDto dto)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome é obrigatório");

        if (dto.LimiteCredito < 0)
            throw new ArgumentException("Limite de crédito não pode ser negativo");

        // Check for duplicate email if provided
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var existingCliente = await _repository.GetByEmailAsync(dto.Email);
            if (existingCliente != null)
                throw new InvalidOperationException("Email já cadastrado");
        }

        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            Endereco = dto.Endereco,
            LimiteCredito = dto.LimiteCredito,
            StatusInadimplencia = StatusInadimplenciaEnum.Adimplente,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };

        await _repository.AddAsync(cliente);

        return MapToDto(cliente);
    }

    public async Task<ClienteDetailDto> GetClienteComSaldoAsync(Guid id)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null || cliente.DeletadoEm != null)
            throw new KeyNotFoundException($"Cliente com ID {id} não encontrado");

        var saldoDevedor = await _repository.CalcularSaldoDevedorAsync(id);
        var diasAtraso = 0;

        // Calculate dias de atraso
        var diasMediaAtraso = 30;
        var overdueDays = await _repository.CountOverdueAsync(id, diasMediaAtraso);
        if (overdueDays > 0)
        {
            diasAtraso = diasMediaAtraso;
        }

        return new ClienteDetailDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email,
            Telefone = cliente.Telefone,
            Endereco = cliente.Endereco,
            LimiteCredito = cliente.LimiteCredito,
            SaldoDevedor = saldoDevedor,
            StatusInadimplencia = (int)cliente.StatusInadimplencia,
            DiasMediaAtraso = diasAtraso,
            CriadoEm = cliente.CriadoEm,
            AtualizadoEm = cliente.AtualizadoEm
        };
    }

    public async Task<ClienteDto> GetClienteAsync(Guid id)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null || cliente.DeletadoEm != null)
            throw new KeyNotFoundException($"Cliente com ID {id} não encontrado");

        return MapToDto(cliente);
    }

    public async Task<IEnumerable<ClienteDto>> GetClientesAsync(
        string? nome = null,
        int? status = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var clientes = await _repository.GetWithFiltersAsync(nome, status, pageNumber, pageSize);
        return clientes.Select(MapToDto);
    }

    public async Task<ClienteDto> UpdateClienteAsync(Guid id, UpdateClienteDto dto)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null || cliente.DeletadoEm != null)
            throw new KeyNotFoundException($"Cliente com ID {id} não encontrado");

        if (dto.Nome != null)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("Nome é obrigatório");

            cliente.Nome = dto.Nome;
        }

        if (dto.Email != null)
        {
            var email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email;

            if (email != null)
            {
                var existing = await _repository.GetByEmailAsync(email);
                if (existing != null && existing.Id != id)
                    throw new InvalidOperationException("Email já cadastrado por outro cliente");
            }

            cliente.Email = email;
        }

        if (dto.Telefone != null)
            cliente.Telefone = string.IsNullOrWhiteSpace(dto.Telefone) ? null : dto.Telefone;

        if (dto.Endereco != null)
            cliente.Endereco = string.IsNullOrWhiteSpace(dto.Endereco) ? null : dto.Endereco;

        await _repository.UpdateAsync(cliente);

        return MapToDto(cliente);
    }

    public async Task<ClienteDto> UpdateLimiteCreditoAsync(Guid id, decimal novoLimite)
    {
        if (novoLimite < 0)
            throw new ArgumentException("Limite de crédito não pode ser negativo");

        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null || cliente.DeletadoEm != null)
            throw new KeyNotFoundException($"Cliente com ID {id} não encontrado");

        cliente.LimiteCredito = novoLimite;
        await _repository.UpdateAsync(cliente);

        return MapToDto(cliente);
    }

    public async Task DeleteClienteAsync(Guid id)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null || cliente.DeletadoEm != null)
            throw new KeyNotFoundException($"Cliente com ID {id} não encontrado");

        await _repository.DeleteAsync(id);
    }

    public async Task ValidarLimiteCreditoAsync(Guid clienteId, decimal novaVendaValor)
    {
        var cliente = await _repository.GetByIdAsync(clienteId);
        if (cliente == null || cliente.DeletadoEm != null)
            throw new KeyNotFoundException($"Cliente com ID {clienteId} não encontrado");

        var totalPendingFiado = await _repository.GetTotalVendasFiadoPendentesAsync(clienteId);

        if ((totalPendingFiado + novaVendaValor) > cliente.LimiteCredito)
            throw new InvalidOperationException("Limite de crédito excedido. " +
                $"Limite: R$ {cliente.LimiteCredito:F2}, Pendente: R$ {totalPendingFiado:F2}, Nova venda: R$ {novaVendaValor:F2}");
    }

    public async Task<int> GetTotalAsync()
    {
        return await _repository.GetTotalAsync();
    }

    private static ClienteDto MapToDto(Cliente cliente)
    {
        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email,
            Telefone = cliente.Telefone,
            Endereco = cliente.Endereco,
            LimiteCredito = cliente.LimiteCredito,
            StatusInadimplencia = (int)cliente.StatusInadimplencia,
            CriadoEm = cliente.CriadoEm,
            AtualizadoEm = cliente.AtualizadoEm
        };
    }
}
