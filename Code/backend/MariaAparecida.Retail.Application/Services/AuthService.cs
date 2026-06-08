using MariaAparecida.Retail.Application.DTOs;
using MariaAparecida.Retail.Application.Repositories;
using MariaAparecida.Retail.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace MariaAparecida.Retail.Application.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto dto, string jwtSecret, int expirationMinutes);
    Task<Guid> RegisterAsync(string email, string senhaPlain);
}

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AuthService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto, string jwtSecret, int expirationMinutes)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
            throw new ArgumentException("Email e senha são obrigatórios");

        var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);
        if (usuario == null)
            throw new UnauthorizedAccessException("Credenciais inválidas");

        if (!BC.Verify(dto.Senha, usuario.SenhaHash))
            throw new UnauthorizedAccessException("Credenciais inválidas");

        var token = GenerateJwtToken(usuario, jwtSecret, expirationMinutes);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
            UsuarioId = usuario.Id,
            Email = usuario.Email
        };
    }

    public async Task<Guid> RegisterAsync(string email, string senhaPlain)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senhaPlain))
            throw new ArgumentException("Email e senha são obrigatórios");

        var existingUsuario = await _usuarioRepository.GetByEmailAsync(email);
        if (existingUsuario != null)
            throw new InvalidOperationException("Email já cadastrado");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = email,
            SenhaHash = BC.HashPassword(senhaPlain),
            Ativo = true,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };

        await _usuarioRepository.AddAsync(usuario);
        return usuario.Id;
    }

    private string GenerateJwtToken(Usuario usuario, string jwtSecret, int expirationMinutes)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSecret);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
