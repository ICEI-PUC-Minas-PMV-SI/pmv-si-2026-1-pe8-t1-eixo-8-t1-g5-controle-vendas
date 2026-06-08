using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MariaAparecida.Retail.Application.DTOs;
using MariaAparecida.Retail.Application.Services;

namespace MariaAparecida.Retail.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VendasController : ControllerBase
{
    private readonly IVendaService _service;
    private readonly ILogger<VendasController> _logger;

    public VendasController(IVendaService service, ILogger<VendasController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Get all vendas with filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<VendaDto>>>> GetVendas(
        [FromQuery] Guid? clienteId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] int? tipoVenda,
        [FromQuery] int? statusPagamento,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (pageSize > 100) pageSize = 100;

            var vendas = await _service.GetVendasAsync(clienteId, dataInicio, dataFim, tipoVenda, statusPagamento, pageNumber, pageSize);
            return Ok(ApiResponse<IEnumerable<VendaDto>>.SuccessResponse(
                vendas,
                $"Retrieved {vendas.Count()} vendas"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vendas");
            return BadRequest(ApiResponse<IEnumerable<VendaDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get venda detail with pagamentos history
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<VendaDetailDto>>> GetVenda(Guid id)
    {
        try
        {
            var venda = await _service.GetVendaComHistoricoPagamentosAsync(id);
            return Ok(ApiResponse<VendaDetailDto>.SuccessResponse(venda));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VendaDetailDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving venda {VendaId}", id);
            return BadRequest(ApiResponse<VendaDetailDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Register new venda (sale)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<VendaDto>>> CreateVenda(CreateVendaDto dto)
    {
        try
        {
            var venda = await _service.RegistrarVendaAsync(dto);
            return CreatedAtAction(nameof(GetVenda), new { id = venda.Id },
                ApiResponse<VendaDto>.SuccessResponse(venda, "Venda registered successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VendaDto>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Credit limit validation failed for cliente");
            return BadRequest(ApiResponse<VendaDto>.ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<VendaDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating venda");
            return StatusCode(500, ApiResponse<VendaDto>.ErrorResponse("Error creating venda"));
        }
    }

    /// <summary>
    /// Update venda details
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<VendaDto>>> UpdateVenda(Guid id, UpdateVendaDto dto)
    {
        try
        {
            var venda = await _service.UpdateVendaAsync(id, dto);
            return Ok(ApiResponse<VendaDto>.SuccessResponse(venda, "Venda updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VendaDto>.ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<VendaDto>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<VendaDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating venda {VendaId}", id);
            return BadRequest(ApiResponse<VendaDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Update venda payment status
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<VendaDto>>> UpdateVendaStatus(Guid id, UpdateVendaStatusDto dto)
    {
        try
        {
            var venda = await _service.AtualizarStatusVendaAsync(id, dto);
            return Ok(ApiResponse<VendaDto>.SuccessResponse(venda, "Venda status updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VendaDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating venda status {VendaId}", id);
            return BadRequest(ApiResponse<VendaDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Delete (soft delete) venda
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVenda(Guid id)
    {
        try
        {
            await _service.DeleteVendaAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting venda {VendaId}", id);
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get venda payment history
    /// </summary>
    [HttpGet("{id}/historico-pagamentos")]
    public async Task<ActionResult<ApiResponse<VendaDetailDto>>> GetVendaPaymentHistory(Guid id)
    {
        try
        {
            var venda = await _service.GetVendaComHistoricoPagamentosAsync(id);
            return Ok(ApiResponse<VendaDetailDto>.SuccessResponse(venda, "Payment history retrieved successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VendaDetailDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving venda payment history {VendaId}", id);
            return BadRequest(ApiResponse<VendaDetailDto>.ErrorResponse(ex.Message));
        }
    }
}
