using FluentValidation;
using MariaAparecida.Retail.Application.DTOs;

namespace MariaAparecida.Retail.Application.Validators;

public class CreateClienteValidator : AbstractValidator<CreateClienteDto>
{
    public CreateClienteValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(150).WithMessage("Nome não pode exceder 150 caracteres")
            .Matches(@"^[a-zA-ZáéíóúâêôãõçÁÉÍÓÚÂÊÔÃÕÇ\s]+$").WithMessage("Nome deve conter apenas letras");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Email inválido");

        RuleFor(x => x.Telefone)
            .Matches(@"^\d{10,11}$").When(x => !string.IsNullOrWhiteSpace(x.Telefone))
            .WithMessage("Telefone deve ter 10 ou 11 dígitos");

        RuleFor(x => x.Endereco)
            .MaximumLength(255).WithMessage("Endereço não pode exceder 255 caracteres");

        RuleFor(x => x.LimiteCredito)
            .GreaterThanOrEqualTo(0).WithMessage("Limite de crédito não pode ser negativo")
            .LessThanOrEqualTo(1000000).WithMessage("Limite de crédito não pode exceder 1.000.000");
    }
}

public class UpdateClienteValidator : AbstractValidator<UpdateClienteDto>
{
    public UpdateClienteValidator()
    {
        RuleFor(x => x.Nome)
            .MinimumLength(3).When(x => !string.IsNullOrWhiteSpace(x.Nome))
            .WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(150).When(x => !string.IsNullOrWhiteSpace(x.Nome))
            .WithMessage("Nome não pode exceder 150 caracteres")
            .Matches(@"^[a-zA-ZáéíóúâêôãõçÁÉÍÓÚÂÊÔÃÕÇ\s]+$").When(x => !string.IsNullOrWhiteSpace(x.Nome))
            .WithMessage("Nome deve conter apenas letras");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Email inválido");

        RuleFor(x => x.Telefone)
            .Matches(@"^\d{10,11}$").When(x => !string.IsNullOrWhiteSpace(x.Telefone))
            .WithMessage("Telefone deve ter 10 ou 11 dígitos");

        RuleFor(x => x.Endereco)
            .MaximumLength(255).When(x => !string.IsNullOrWhiteSpace(x.Endereco))
            .WithMessage("Endereço não pode exceder 255 caracteres");
    }
}

public class UpdateLimiteCreditoValidator : AbstractValidator<UpdateLimiteCreditoDto>
{
    public UpdateLimiteCreditoValidator()
    {
        RuleFor(x => x.NovoLimite)
            .GreaterThanOrEqualTo(0).WithMessage("Limite de crédito não pode ser negativo")
            .LessThanOrEqualTo(1000000).WithMessage("Limite de crédito não pode exceder 1.000.000");
    }
}
