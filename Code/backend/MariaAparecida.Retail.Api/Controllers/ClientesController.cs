using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MariaAparecida.Retail.Application.DTOs;
using MariaAparecida.Retail.Application.Services;

namespace MariaAparecida.Retail.Api.Controllers;

/// <summary>
/// Customer management endpoints for CRUD operations, credit limit management, and customer status tracking.
/// All endpoints require JWT authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _service;
    private readonly ILogger<ClientesController> _logger;

    /// <summary>
    /// Initializes a new instance of the ClientesController.
    /// </summary>
    /// <param name="service">Service for customer operations</param>
    /// <param name="logger">Logger for customer events</param>
    public ClientesController(IClienteService service, ILogger<ClientesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a paginated list of customers with optional filtering.
    /// Soft-deleted customers are automatically excluded.
    /// </summary>
    /// <param name="nome">Optional filter by customer name (partial match)</param>
    /// <param name="status">Optional filter by payment status (0=Adimplente, 1=Atraso, 2=Inadimplente)</param>
    /// <param name="pageNumber">Page number for pagination (default: 1, minimum: 1)</param>
    /// <param name="pageSize">Number of records per page (default: 10, maximum: 100)</param>
    /// <returns>Paginated list of customer DTOs</returns>
    /// <response code="200">Customers retrieved successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClienteDto>>>> GetClientes(
        [FromQuery] string? nome,
        [FromQuery] int? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (pageSize > 100) pageSize = 100;

            var clientes = await _service.GetClientesAsync(nome, status, pageNumber, pageSize);
            return Ok(ApiResponse<IEnumerable<ClienteDto>>.SuccessResponse(
                clientes,
                $"Retrieved {clientes.Count()} clientes"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving clientes");
            return BadRequest(ApiResponse<IEnumerable<ClienteDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get cliente detail with calculated saldo devedor
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ClienteDetailDto>>> GetCliente(Guid id)
    {
        try
        {
            var cliente = await _service.GetClienteComSaldoAsync(id);
            return Ok(ApiResponse<ClienteDetailDto>.SuccessResponse(cliente));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ClienteDetailDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cliente {ClienteId}", id);
            return BadRequest(ApiResponse<ClienteDetailDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Create new cliente
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ClienteDto>>> CreateCliente(CreateClienteDto dto)
    {
        try
        {
            var cliente = await _service.CreateClienteAsync(dto);
            return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id },
                ApiResponse<ClienteDto>.SuccessResponse(cliente, "Cliente created successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<ClienteDto>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<ClienteDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating cliente");
            return BadRequest(ApiResponse<ClienteDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Update cliente information
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ClienteDto>>> UpdateCliente(Guid id, UpdateClienteDto dto)
    {
        try
        {
            var cliente = await _service.UpdateClienteAsync(id, dto);
            return Ok(ApiResponse<ClienteDto>.SuccessResponse(cliente, "Cliente updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ClienteDto>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<ClienteDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cliente {ClienteId}", id);
            return BadRequest(ApiResponse<ClienteDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Update cliente credit limit
    /// </summary>
    [HttpPatch("{id}/limite-credito")]
    public async Task<ActionResult<ApiResponse<ClienteDto>>> UpdateLimiteCredito(
        Guid id,
        [FromBody] UpdateLimiteCreditoDto dto)
    {
        try
        {
            var cliente = await _service.UpdateLimiteCreditoAsync(id, dto.NovoLimite);
            return Ok(ApiResponse<ClienteDto>.SuccessResponse(cliente, "Credit limit updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ClienteDto>.ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<ClienteDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating credit limit for cliente {ClienteId}", id);
            return BadRequest(ApiResponse<ClienteDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Soft delete cliente
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteCliente(Guid id)
    {
        try
        {
            await _service.DeleteClienteAsync(id);
            return Ok(ApiResponse.SuccessResponse("Cliente deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting cliente {ClienteId}", id);
            return BadRequest(ApiResponse.ErrorResponse(ex.Message));
        }
    }
}
