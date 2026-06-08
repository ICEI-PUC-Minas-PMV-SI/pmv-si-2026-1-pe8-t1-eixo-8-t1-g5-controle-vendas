using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MariaAparecida.Retail.Application.DTOs;
using MariaAparecida.Retail.Application.Services;

namespace MariaAparecida.Retail.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PagamentosController : ControllerBase
{
    private readonly IPagamentoService _service;
    private readonly ILogger<PagamentosController> _logger;

    public PagamentosController(IPagamentoService service, ILogger<PagamentosController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Get all pagamentos with filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<PagamentoDto>>>> GetPagamentos(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] int? metodo,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (pageSize > 100) pageSize = 100;

            var pagamentos = await _service.GetPagamentosAsync(dataInicio, dataFim, metodo, pageNumber, pageSize);
            return Ok(ApiResponse<IEnumerable<PagamentoDto>>.SuccessResponse(
                pagamentos,
                $"Retrieved {pagamentos.Count()} pagamentos"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pagamentos");
            return BadRequest(ApiResponse<IEnumerable<PagamentoDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get pagamento by id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PagamentoDetailDto>>> GetPagamento(Guid id)
    {
        try
        {
            var pagamento = await _service.GetPagamentoAsync(id);
            return Ok(ApiResponse<PagamentoDetailDto>.SuccessResponse(pagamento));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<PagamentoDetailDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pagamento {PagamentoId}", id);
            return BadRequest(ApiResponse<PagamentoDetailDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Register new pagamento (payment/receipt)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PagamentoDto>>> CreatePagamento(CreatePagamentoDto dto)
    {
        try
        {
            var pagamento = await _service.RegistrarPagamentoAsync(dto);
            return CreatedAtAction(nameof(GetPagamento), new { id = pagamento.Id },
                ApiResponse<PagamentoDto>.SuccessResponse(pagamento, "Pagamento registered successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<PagamentoDto>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid pagamento amount for venda");
            return BadRequest(ApiResponse<PagamentoDto>.ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<PagamentoDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pagamento");
            return StatusCode(500, ApiResponse<PagamentoDto>.ErrorResponse("Error creating pagamento"));
        }
    }

    /// <summary>
    /// Update pagamento details
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<PagamentoDto>>> UpdatePagamento(Guid id, UpdatePagamentoDto dto)
    {
        try
        {
            var pagamento = await _service.UpdatePagamentoAsync(id, dto);
            return Ok(ApiResponse<PagamentoDto>.SuccessResponse(pagamento, "Pagamento updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<PagamentoDto>.ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<PagamentoDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating pagamento {PagamentoId}", id);
            return BadRequest(ApiResponse<PagamentoDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Reverse (delete) pagamento and update venda status
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePagamento(Guid id)
    {
        try
        {
            await _service.ReversarPagamentoAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting pagamento {PagamentoId}", id);
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get all pagamentos for a specific venda
    /// </summary>
    [HttpGet("venda/{vendaId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PagamentoDto>>>> GetPagamentosVenda(Guid vendaId)
    {
        try
        {
            var pagamentos = await _service.GetPagamentosVendaAsync(vendaId);
            return Ok(ApiResponse<IEnumerable<PagamentoDto>>.SuccessResponse(
                pagamentos,
                $"Retrieved {pagamentos.Count()} pagamentos for venda"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pagamentos for venda {VendaId}", vendaId);
            return BadRequest(ApiResponse<IEnumerable<PagamentoDto>>.ErrorResponse(ex.Message));
        }
    }
}
