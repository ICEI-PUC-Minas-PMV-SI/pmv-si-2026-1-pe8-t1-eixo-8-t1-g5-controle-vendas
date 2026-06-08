using FluentValidation;
using MariaAparecida.Retail.Application.DTOs;

namespace MariaAparecida.Retail.Application.Validators;

public class CreatePagamentoValidator : AbstractValidator<CreatePagamentoDto>
{
    public CreatePagamentoValidator()
    {
        RuleFor(x => x.VendaId)
            .NotEmpty().WithMessage("VendaId é obrigatório");

        RuleFor(x => x.ClienteId)
            .NotEmpty().WithMessage("ClienteId é obrigatório");

        RuleFor(x => x.ValorPago)
            .GreaterThan(0).WithMessage("Valor deve ser maior que zero")
            .LessThanOrEqualTo(1000000).WithMessage("Valor não pode exceder 1.000.000");

        RuleFor(x => x.DataPagamento)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Data do pagamento não pode ser no futuro");

        RuleFor(x => x.MetodoPagamento)
            .GreaterThan(0).WithMessage("Método de pagamento inválido");

        RuleFor(x => x.Observacoes)
            .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Observacoes))
            .WithMessage("Observações não podem exceder 500 caracteres");
    }
}

public class UpdatePagamentoValidator : AbstractValidator<UpdatePagamentoDto>
{
    public UpdatePagamentoValidator()
    {
        RuleFor(x => x.ValorPago)
            .GreaterThan(0).WithMessage("Valor deve ser maior que zero")
            .LessThanOrEqualTo(1000000).WithMessage("Valor não pode exceder 1.000.000");

        RuleFor(x => x.MetodoPagamento)
            .GreaterThan(0).WithMessage("Método de pagamento inválido");

        RuleFor(x => x.Observacoes)
            .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Observacoes))
            .WithMessage("Observações não podem exceder 500 caracteres");
    }
}
