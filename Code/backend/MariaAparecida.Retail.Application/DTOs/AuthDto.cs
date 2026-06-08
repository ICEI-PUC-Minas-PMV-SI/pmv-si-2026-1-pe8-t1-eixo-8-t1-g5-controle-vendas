namespace MariaAparecida.Retail.Application.DTOs;

public class LoginDto
{
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
}

public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public Guid UsuarioId { get; set; }
    public string Email { get; set; } = null!;
}
