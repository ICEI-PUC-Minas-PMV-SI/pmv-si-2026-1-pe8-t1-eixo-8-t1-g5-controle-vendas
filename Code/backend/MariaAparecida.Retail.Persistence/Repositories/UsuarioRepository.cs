using MariaAparecida.Retail.Application.Repositories;
using MariaAparecida.Retail.Domain.Entities;
using MariaAparecida.Retail.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MariaAparecida.Retail.Persistence.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly MariaAparecidaDbContext _context;

    public UsuarioRepository(MariaAparecidaDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByIdAsync(Guid id)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id && u.Ativo);
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.Ativo);
    }

    public async Task<Usuario?> AuthenticateAsync(string email, string senhaHash)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.SenhaHash == senhaHash && u.Ativo);
    }

    public async Task AddAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }
}
