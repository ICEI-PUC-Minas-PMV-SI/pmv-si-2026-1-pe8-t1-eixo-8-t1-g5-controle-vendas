using MariaAparecida.Retail.Application.DTOs;
using MariaAparecida.Retail.Application.Validators;
using FluentValidation;

namespace MariaAparecida.Retail.Tests;

public class ValidatorTests
{
    [Fact]
    public void CreateClienteValidator_WithValidData_ShouldNotFail()
    {
        // Arrange
        var validator = new CreateClienteValidator();
        var dto = new CreateClienteDto
        {
            Nome = "Valid Name",
            Email = "valid@example.com",
            Telefone = "31999999999",
            Endereco = "Rua A, 123",
            LimiteCredito = 1000m
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateClienteValidator_WithoutNome_ShouldFail()
    {
        // Arrange
        var validator = new CreateClienteValidator();
        var dto = new CreateClienteDto
        {
            Nome = "",
            Email = "valid@example.com",
            Telefone = "31999999999",
            Endereco = "Rua A, 123",
            LimiteCredito = 1000m
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Nome");
    }

    [Fact]
    public void CreateClienteValidator_WithInvalidEmail_ShouldFail()
    {
        // Arrange
        var validator = new CreateClienteValidator();
        var dto = new CreateClienteDto
        {
            Nome = "Valid Name",
            Email = "invalid-email",
            Telefone = "31999999999",
            Endereco = "Rua A, 123",
            LimiteCredito = 1000m
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void CreateClienteValidator_WithNegativeLimiteCredito_ShouldFail()
    {
        // Arrange
        var validator = new CreateClienteValidator();
        var dto = new CreateClienteDto
        {
            Nome = "Valid Name",
            Email = "valid@example.com",
            Telefone = "31999999999",
            Endereco = "Rua A, 123",
            LimiteCredito = -100m
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LimiteCredito");
    }

    [Fact]
    public void LoginValidator_WithValidData_ShouldNotFail()
    {
        // Arrange
        var validator = new LoginValidator();
        var dto = new LoginDto
        {
            Email = "user@example.com",
            Senha = "ValidPassword123!"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void LoginValidator_WithoutEmail_ShouldFail()
    {
        // Arrange
        var validator = new LoginValidator();
        var dto = new LoginDto
        {
            Email = "",
            Senha = "ValidPassword123!"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateVendaValidator_WithValidData_ShouldNotFail()
    {
        // Arrange
        var validator = new CreateVendaValidator();
        var dto = new CreateVendaDto
        {
            ClienteId = Guid.NewGuid(),
            DataVenda = DateTime.UtcNow.AddDays(-1), // Past date
            ValorTotal = 1000m,
            TipoVenda = 1, // Dinheiro
            Descricao = "Valid sale"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateVendaValidator_WithZeroValor_ShouldFail()
    {
        // Arrange
        var validator = new CreateVendaValidator();
        var dto = new CreateVendaDto
        {
            ClienteId = Guid.NewGuid(),
            DataVenda = DateTime.UtcNow,
            ValorTotal = 0,
            TipoVenda = 1,
            Descricao = "Invalid sale"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreatePagamentoValidator_WithValidData_ShouldNotFail()
    {
        // Arrange
        var validator = new CreatePagamentoValidator();
        var dto = new CreatePagamentoDto
        {
            VendaId = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            DataPagamento = DateTime.UtcNow.AddDays(-1), // Past date
            ValorPago = 500m,
            MetodoPagamento = 1 // Dinheiro
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreatePagamentoValidator_WithNegativeValor_ShouldFail()
    {
        // Arrange
        var validator = new CreatePagamentoValidator();
        var dto = new CreatePagamentoDto
        {
            VendaId = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            DataPagamento = DateTime.UtcNow,
            ValorPago = -500m,
            MetodoPagamento = 1
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpdateClienteValidator_WithValidData_ShouldNotFail()
    {
        // Arrange
        var validator = new UpdateClienteValidator();
        var dto = new UpdateClienteDto
        {
            Nome = "Updated Name",
            Email = "updated@example.com",
            Telefone = "31988888888",
            Endereco = "Rua B, 456"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateLimiteCreditoValidator_WithValidData_ShouldNotFail()
    {
        // Arrange
        var validator = new UpdateLimiteCreditoValidator();
        var dto = new UpdateLimiteCreditoDto
        {
            NovoLimite = 5000m
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateLimiteCreditoValidator_WithNegativeLimit_ShouldFail()
    {
        // Arrange
        var validator = new UpdateLimiteCreditoValidator();
        var dto = new UpdateLimiteCreditoDto
        {
            NovoLimite = -1000m
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
    }
}
