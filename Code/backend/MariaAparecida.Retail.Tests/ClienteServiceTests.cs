using MariaAparecida.Retail.Application.DTOs;
using MariaAparecida.Retail.Application.Repositories;
using MariaAparecida.Retail.Application.Services;
using MariaAparecida.Retail.Domain.Entities;
using MariaAparecida.Retail.Domain.Enums;
using Moq;

namespace MariaAparecida.Retail.Tests;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly IClienteService _service;

    public ClienteServiceTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _service = new ClienteService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateCliente_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var createDto = new CreateClienteDto
        {
            Nome = "Maria Silva",
            Email = "maria@example.com",
            Telefone = "31999999999",
            Endereco = "Rua A, 123",
            LimiteCredito = 500m
        };

        Cliente? capturedCliente = null;
        _repositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Cliente?)null);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Cliente>()))
            .Callback<Cliente>(c => capturedCliente = c)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateClienteAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Maria Silva", result.Nome);
        Assert.Equal("maria@example.com", result.Email);
        Assert.Equal(500m, result.LimiteCredito);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public async Task CreateCliente_WithDuplicateEmail_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateClienteDto
        {
            Nome = "Maria Silva",
            Email = "duplicate@example.com",
            LimiteCredito = 500m
        };

        var existingCliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = "Existing",
            Email = "duplicate@example.com",
            LimiteCredito = 500m,
            StatusInadimplencia = StatusInadimplenciaEnum.Adimplente
        };

        _repositoryMock
            .Setup(r => r.GetByEmailAsync("duplicate@example.com"))
            .ReturnsAsync(existingCliente);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateClienteAsync(createDto));
    }

    [Fact]
    public async Task CreateCliente_WithMissingNome_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateClienteDto
        {
            Nome = "",
            Email = "maria@example.com",
            LimiteCredito = 500m
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateClienteAsync(createDto));
    }

    [Fact]
    public async Task GetClienteComSaldo_WithValidId_ShouldReturnClienteWithSaldo()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var cliente = new Cliente
        {
            Id = clienteId,
            Nome = "Maria Silva",
            Email = "maria@example.com",
            Telefone = "31999999999",
            LimiteCredito = 500m,
            StatusInadimplencia = StatusInadimplenciaEnum.Adimplente
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(clienteId))
            .ReturnsAsync(cliente);

        _repositoryMock
            .Setup(r => r.CalcularSaldoDevedorAsync(clienteId))
            .ReturnsAsync(150m);

        // Act
        var result = await _service.GetClienteComSaldoAsync(clienteId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(clienteId, result.Id);
        Assert.Equal("Maria Silva", result.Nome);
        Assert.Equal(150m, result.SaldoDevedor);
        _repositoryMock.Verify(r => r.CalcularSaldoDevedorAsync(clienteId), Times.Once);
    }

    [Fact]
    public async Task GetClienteComSaldo_WithInvalidId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(clienteId))
            .ReturnsAsync((Cliente?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetClienteComSaldoAsync(clienteId));
    }

    [Fact]
    public async Task UpdateCliente_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var existingCliente = new Cliente
        {
            Id = clienteId,
            Nome = "Maria Silva",
            Email = "maria@example.com",
            Telefone = "31999999999",
            LimiteCredito = 500m,
            StatusInadimplencia = StatusInadimplenciaEnum.Adimplente
        };

        var updateDto = new UpdateClienteDto
        {
            Nome = "Maria Santos",
            Telefone = "31988888888"
        };

        Cliente? capturedCliente = null;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(clienteId))
            .ReturnsAsync(existingCliente);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Cliente>()))
            .Callback<Cliente>(c => capturedCliente = c)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateClienteAsync(clienteId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Maria Santos", result.Nome);
        Assert.Equal("31988888888", result.Telefone);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public async Task UpdateLimiteCredito_WithValidLimit_ShouldUpdateSuccessfully()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var existingCliente = new Cliente
        {
            Id = clienteId,
            Nome = "Maria Silva",
            Email = "maria@example.com",
            LimiteCredito = 500m,
            StatusInadimplencia = StatusInadimplenciaEnum.Adimplente
        };

        var novoLimite = 1000m;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(clienteId))
            .ReturnsAsync(existingCliente);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Cliente>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateLimiteCreditoAsync(clienteId, novoLimite);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1000m, result.LimiteCredito);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCliente_WithValidId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var existingCliente = new Cliente
        {
            Id = clienteId,
            Nome = "Maria Silva",
            Email = "maria@example.com",
            LimiteCredito = 500m,
            StatusInadimplencia = StatusInadimplenciaEnum.Adimplente
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(clienteId))
            .ReturnsAsync(existingCliente);

        _repositoryMock
            .Setup(r => r.DeleteAsync(clienteId))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteClienteAsync(clienteId);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(clienteId), Times.Once);
    }

    [Fact]
    public async Task ValidarLimiteCredito_WhenCreditAvailable_ShouldNotThrow()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var novaVendaValor = 200m;

        _repositoryMock
            .Setup(r => r.GetTotalVendasFiadoPendentesAsync(clienteId))
            .ReturnsAsync(200m);

        var cliente = new Cliente
        {
            Id = clienteId,
            Nome = "Maria Silva",
            LimiteCredito = 500m,
            StatusInadimplencia = StatusInadimplenciaEnum.Adimplente
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(clienteId))
            .ReturnsAsync(cliente);

        // Act & Assert
        await _service.ValidarLimiteCreditoAsync(clienteId, novaVendaValor);
    }

    [Fact]
    public async Task ValidarLimiteCredito_WhenCreditExceeded_ShouldThrow()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var novaVendaValor = 400m;

        var cliente = new Cliente
        {
            Id = clienteId,
            Nome = "Maria Silva",
            LimiteCredito = 500m,
            StatusInadimplencia = StatusInadimplenciaEnum.Adimplente
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(clienteId))
            .ReturnsAsync(cliente);

        _repositoryMock
            .Setup(r => r.GetTotalVendasFiadoPendentesAsync(clienteId))
            .ReturnsAsync(300m);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.ValidarLimiteCreditoAsync(clienteId, novaVendaValor));
    }

    [Fact]
    public async Task GetTotal_ShouldReturnCorrectCount()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetTotalAsync())
            .ReturnsAsync(5);

        // Act
        var result = await _service.GetTotalAsync();

        // Assert
        Assert.Equal(5, result);
        _repositoryMock.Verify(r => r.GetTotalAsync(), Times.Once);
    }
}
