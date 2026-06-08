using Microsoft.AspNetCore.Mvc;
using MariaAparecida.Retail.Application.DTOs;
using MariaAparecida.Retail.Application.Services;

namespace MariaAparecida.Retail.Api.Controllers;

/// <summary>
/// Authentication management endpoints for user registration, login, and logout.
/// Provides JWT token generation and validation for secure API access.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the AuthController.
    /// </summary>
    /// <param name="authService">Service for authentication operations</param>
    /// <param name="configuration">Application configuration for JWT settings</param>
    /// <param name="logger">Logger for authentication events</param>
    public AuthController(IAuthService authService, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _authService = authService;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token for subsequent API calls.
    /// Token expires after the configured expiration time (default: 480 minutes).
    /// </summary>
    /// <param name="dto">Login credentials (email and password)</param>
    /// <returns>JWT token and user details on successful authentication</returns>
    /// <response code="200">Login successful, returns JWT token</response>
    /// <response code="400">Invalid request format or missing credentials</response>
    /// <response code="401">Invalid email or password</response>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(LoginDto dto)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "your-256-bit-secret-key-for-jwt-authentication-very-long";
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "480");

            var response = await _authService.LoginAsync(dto, secretKey, expirationMinutes);
            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login bem-sucedido"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized login attempt for email: {Email}", dto.Email);
            return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse("Email ou senha incorretos"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid login request");
            return BadRequest(ApiResponse<LoginResponseDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login");
            return StatusCode(500, ApiResponse<LoginResponseDto>.ErrorResponse("Erro ao processar login"));
        }
    }

    /// <summary>
    /// Registers a new user account with email and password.
    /// Password must be secure (minimum requirements enforced by validator).
    /// Email must be unique in the system.
    /// </summary>
    /// <param name="dto">Registration data (email and password)</param>
    /// <returns>Newly created user ID</returns>
    /// <response code="201">User successfully registered</response>
    /// <response code="400">Invalid email or password format</response>
    /// <response code="409">Email already registered</response>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<Guid>>> Register(LoginDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
                return BadRequest(ApiResponse<Guid>.ErrorResponse("Email e senha são obrigatórios"));

            var usuarioId = await _authService.RegisterAsync(dto.Email, dto.Senha);
            return CreatedAtAction(nameof(Register), ApiResponse<Guid>.SuccessResponse(usuarioId, "Usuário registrado com sucesso"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Duplicate email registration attempt: {Email}", dto.Email);
            return Conflict(ApiResponse<Guid>.ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid registration request");
            return BadRequest(ApiResponse<Guid>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration");
            return StatusCode(500, ApiResponse<Guid>.ErrorResponse("Erro ao registrar usuário"));
        }
    }

    /// <summary>
    /// Logs out the current user by invalidating their session.
    /// Note: Since JWT tokens are stateless, the client is responsible for removing the token.
    /// This endpoint provides a logout event for logging purposes.
    /// </summary>
    /// <returns>Logout confirmation message</returns>
    /// <response code="200">Logout successful</response>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        _logger.LogInformation("Logout request");
        return Ok(new { message = "Logout bem-sucedido. Por favor, remova o token do cliente." });
    }
}
