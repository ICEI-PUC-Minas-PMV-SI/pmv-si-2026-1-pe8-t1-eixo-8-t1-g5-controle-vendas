using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MariaAparecida.Retail.Application.DTOs;
using MariaAparecida.Retail.Application.Services;

namespace MariaAparecida.Retail.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _service;
    private readonly ILogger<RelatoriosController> _logger;

    public RelatoriosController(IRelatorioService service, ILogger<RelatoriosController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Get comprehensive KPI dashboard
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<DashboardKpisDto>>> GetDashboard(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        try
        {
            var dashboard = await _service.CalcularDashboardKpisAsync(dataInicio, dataFim);
            return Ok(ApiResponse<DashboardKpisDto>.SuccessResponse(dashboard, "Dashboard retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard KPIs");
            return BadRequest(ApiResponse<DashboardKpisDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get real monthly revenue for the dashboard chart
    /// </summary>
    [HttpGet("receita-mensal")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReceitaMensalDto>>>> GetReceitaMensal(
        [FromQuery] int meses = 6)
    {
        try
        {
            var receitaMensal = await _service.GetReceitaMensalAsync(meses);
            return Ok(ApiResponse<IEnumerable<ReceitaMensalDto>>.SuccessResponse(receitaMensal, "Receita mensal retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving receita mensal");
            return BadRequest(ApiResponse<IEnumerable<ReceitaMensalDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get total recebiveis (accounts receivable)
    /// </summary>
    [HttpGet("recebiveis")]
    public async Task<ActionResult<ApiResponse<decimal>>> GetRecebiveis(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        try
        {
            var total = await _service.CalcularTotalRecebiveisAsync(dataInicio, dataFim);
            return Ok(ApiResponse<decimal>.SuccessResponse(total, "Total recebiveis retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recebiveis");
            return BadRequest(ApiResponse<decimal>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get taxa padrão (payment rate)
    /// </summary>
    [HttpGet("taxa-padrao")]
    public async Task<ActionResult<ApiResponse<decimal>>> GetTaxaPadrao(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        try
        {
            var taxa = await _service.CalcularTaxaPadraoAsync(dataInicio, dataFim);
            return Ok(ApiResponse<decimal>.SuccessResponse(taxa, "Taxa padrão retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving taxa padrão");
            return BadRequest(ApiResponse<decimal>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get periodo médio de coleta (average collection period)
    /// </summary>
    [HttpGet("periodo-coleta")]
    public async Task<ActionResult<ApiResponse<int>>> GetPeriodoColeta(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        try
        {
            var periodo = await _service.CalcularPeriodoMedioColetaAsync(dataInicio, dataFim);
            return Ok(ApiResponse<int>.SuccessResponse(periodo, "Período médio de coleta retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving período de coleta");
            return BadRequest(ApiResponse<int>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get índice de concentração (concentration index)
    /// </summary>
    [HttpGet("concentracao")]
    public async Task<ActionResult<ApiResponse<dynamic>>> GetConcentracao(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        try
        {
            var indiceConcentracao = await _service.CalcularIndiceConcentracaoAsync(dataInicio, dataFim);
            var recebivelPorCliente = await _service.GetRecebivelPorClienteAsync(dataInicio, dataFim);
            
            var totalRecebivel = recebivelPorCliente.Sum(r => r.total);
            
            var topClientes = recebivelPorCliente
                .OrderByDescending(x => x.total)
                .Take(10)
                .Select(x => new
                {
                    cliente = new { id = x.clienteId, nome = x.nome },
                    totalDevendo = x.total,
                    percentualTotal = totalRecebivel > 0 
                        ? (x.total / totalRecebivel)
                        : 0
                })
                .ToList();
            
            var resultado = new
            {
                indiceConcentracao,
                topClientes
            };
            
            return Ok(ApiResponse<dynamic>.SuccessResponse(resultado, "Índice de concentração retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving concentração");
            return BadRequest(ApiResponse<dynamic>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get clientes inadimplentes (defaulting customers)
    /// </summary>
    [HttpGet("inadimplencia")]
    public async Task<ActionResult<ApiResponse<IEnumerable<dynamic>>>> GetInadimplentes(
        [FromQuery] int diasAtraso = 30)
    {
        try
        {
            var inadimplentes = await _service.ListarClientesInadimplentesAsync(diasAtraso);
            var result = inadimplentes.Select(x => new
            {
                cliente = new { id = x.clienteId, nome = x.nome },
                totalDevendo = x.valor,
                diasAtraso
            });
            
            return Ok(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(result, "Clientes inadimplentes retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving clientes inadimplentes");
            return BadRequest(ApiResponse<IEnumerable<dynamic>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get risco médio (average risk)
    /// </summary>
    [HttpGet("risco")]
    public async Task<ActionResult<ApiResponse<decimal>>> GetRisco(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        try
        {
            var risco = await _service.AnalisarRiscoValorAsync(dataInicio, dataFim);
            return Ok(ApiResponse<decimal>.SuccessResponse(risco, "Risco médio retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving risco");
            return BadRequest(ApiResponse<decimal>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get custom report for date interval
    /// </summary>
    [HttpGet("intervalo")]
    public async Task<ActionResult<ApiResponse<dynamic>>> GetIntervaloReport(
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim,
        [FromQuery] int diasAtraso = 30)
    {
        try
        {
            if (dataFim < dataInicio)
                return BadRequest(ApiResponse<dynamic>.ErrorResponse("Data fim deve ser posterior à data início"));

            var totalRecebimentos = await _service.CalcularTotalReceitaAsync(dataInicio, dataFim);
            var totalDevido = await _service.CalcularTotalRecebiveisAsync(dataInicio, dataFim);
            var inadimplentes = await _service.ListarClientesInadimplentesAsync(diasAtraso);
            var clientesAtivos = await _service.GetTotalClientesAsync();
            
            var listaInadimplentes = inadimplentes.Select(x => new
            {
                cliente = new { id = x.clienteId, nome = x.nome },
                diasAtraso,
                totalDevendo = x.valor
            }).ToList();

            var report = new
            {
                periodoInicio = dataInicio.ToString("yyyy-MM-dd"),
                periodoFim = dataFim.ToString("yyyy-MM-dd"),
                totalRecebimentos,
                totalDevido,
                clientesAtivos,
                inadimplentes = listaInadimplentes
            };

            return Ok(ApiResponse<dynamic>.SuccessResponse(report, "Relatório do intervalo retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving intervalo report");
            return BadRequest(ApiResponse<dynamic>.ErrorResponse(ex.Message));
        }
    }
}
