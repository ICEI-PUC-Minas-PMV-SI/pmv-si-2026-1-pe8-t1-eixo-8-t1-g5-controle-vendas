using FluentValidation;
using MariaAparecida.Retail.Application.DTOs;

namespace MariaAparecida.Retail.Application.Validators;

public class CreateVendaValidator : AbstractValidator<CreateVendaDto>
{
    public CreateVendaValidator()
    {
        RuleFor(x => x.ClienteId)
            .NotEmpty().WithMessage("ClienteId é obrigatório");

        RuleFor(x => x.DataVenda)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Data da venda não pode ser no futuro");

        RuleFor(x => x.ValorTotal)
            .GreaterThan(0).WithMessage("Valor deve ser maior que zero")
            .LessThanOrEqualTo(1000000).WithMessage("Valor não pode exceder 1.000.000");

        RuleFor(x => x.TipoVenda)
            .GreaterThan(0).WithMessage("Tipo de venda inválido");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descrição não pode exceder 500 caracteres");
    }
}

public class UpdateVendaValidator : AbstractValidator<UpdateVendaDto>
{
    public UpdateVendaValidator()
    {
        RuleFor(x => x.ClienteId)
            .NotEqual(Guid.Empty).When(x => x.ClienteId.HasValue)
            .WithMessage("ClienteId é obrigatório");

        RuleFor(x => x.DataVenda)
            .LessThanOrEqualTo(DateTime.Now).When(x => x.DataVenda.HasValue)
            .WithMessage("Data da venda não pode ser no futuro");

        RuleFor(x => x.ValorTotal)
            .GreaterThan(0).When(x => x.ValorTotal.HasValue)
            .WithMessage("Valor deve ser maior que zero")
            .LessThanOrEqualTo(1000000).When(x => x.ValorTotal.HasValue)
            .WithMessage("Valor não pode exceder 1.000.000");

        RuleFor(x => x.TipoVenda)
            .InclusiveBetween(1, 2).When(x => x.TipoVenda.HasValue)
            .WithMessage("Tipo de venda inválido");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descrição não pode exceder 500 caracteres");
    }
}

public class UpdateVendaStatusValidator : AbstractValidator<UpdateVendaStatusDto>
{
    public UpdateVendaStatusValidator()
    {
        RuleFor(x => x.NovoStatus)
            .GreaterThan(0).WithMessage("Status de pagamento inválido");
    }
}
