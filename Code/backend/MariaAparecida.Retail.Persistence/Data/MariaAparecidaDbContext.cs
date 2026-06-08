using Microsoft.EntityFrameworkCore;
using MariaAparecida.Retail.Domain.Entities;
using MariaAparecida.Retail.Persistence.Configurations;

namespace MariaAparecida.Retail.Persistence.Data;

public class MariaAparecidaDbContext : DbContext
{
    public MariaAparecidaDbContext(DbContextOptions<MariaAparecidaDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Venda> Vendas { get; set; } = null!;
    public DbSet<Pagamento> Pagamentos { get; set; } = null!;
    public DbSet<RelatoriosDiarios> RelatoriosDiarios { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
        modelBuilder.ApplyConfiguration(new ClienteConfiguration());
        modelBuilder.ApplyConfiguration(new VendaConfiguration());
        modelBuilder.ApplyConfiguration(new PagamentoConfiguration());
        modelBuilder.ApplyConfiguration(new RelatoriosDiariosConfiguration());
    }
}
