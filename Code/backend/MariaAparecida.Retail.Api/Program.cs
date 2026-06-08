using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MariaAparecida.Retail.Persistence.Data;
using MariaAparecida.Retail.Application.Repositories;
using MariaAparecida.Retail.Application.Services;
using MariaAparecida.Retail.Persistence.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using MariaAparecida.Retail.Application.Validators;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Port=5432;Database=maria_aparecida_retail;Username=postgres;Password=postgres";
builder.Services.AddDbContext<MariaAparecidaDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add Repositories
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IVendaRepository, VendaRepository>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();
builder.Services.AddScoped<IRelatorioRepository, RelatorioRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Add Services
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVendaService, VendaService>();
builder.Services.AddScoped<IPagamentoService, PagamentoService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();

// Add Caching
builder.Services.AddMemoryCache();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining(typeof(CreateClienteValidator));
builder.Services.AddFluentValidationAutoValidation();

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? "your-256-bit-secret-key-for-jwt-authentication-very-long");
var tokenExpirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "480");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactLocal", policy =>
    {
        policy
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Maria Aparecida Retail API",
        Version = "v1",
        Description = "Backend API for Maria Aparecida's retail management system"
    });
});

// Add Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Maria Aparecida Retail API v1");
});

app.UseHttpsRedirection();
app.UseCors("AllowReactLocal");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
