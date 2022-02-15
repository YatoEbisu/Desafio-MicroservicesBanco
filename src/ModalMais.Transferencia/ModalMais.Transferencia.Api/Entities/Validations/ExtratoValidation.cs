using System;
using FluentValidation;
using ModalMais.Transferencia.Api.DTOs;

namespace ModalMais.Transferencia.Api.Entities.Validations
{
    public class ExtratoValidation : AbstractValidator<Extrato>
    {
        public ExtratoValidation()
        {
            RuleFor(y => y.NumeroAgencia)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.");

            RuleFor(y => y.NumeroConta)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.");

            When(y => y.DataInicial.HasValue, () =>
            {
                RuleFor(x => x.DataFinal).NotNull()
                    .WithMessage("O campo {PropertyName} precisa ser fornecido.");
            });

            When(y => y.DataFinal.HasValue, () =>
            {
                RuleFor(p => p.DataInicial)
                    .NotNull().WithMessage("O campo {PropertyName} precisa ser fornecido.");
            });

            When(y => y.DataInicial.HasValue && y.DataFinal.HasValue, () =>
            {
                RuleFor(x => Math.Abs((x.DataInicial.Value.Date - x.DataFinal.Value.Date).TotalDays) > 30).Equal(false)
                    .WithMessage("O período não pode ser superior a 30 dias");
            });
        }
    }

    public class ExtratoRequestValidation : AbstractValidator<ExtratoRequest>
    {
        public ExtratoRequestValidation()
        {
            RuleFor(y => y.NumeroAgencia)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.");

            RuleFor(y => y.NumeroConta)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.");

            When(y => y.DataInicial.HasValue, () =>
            {
                RuleFor(x => x.DataFinal).NotNull()
                    .WithMessage("O campo {PropertyName} precisa ser fornecido.");
            });

            When(y => y.DataFinal.HasValue, () =>
            {
                RuleFor(p => p.DataInicial)
                    .NotNull().WithMessage("O campo {PropertyName} precisa ser fornecido.");
            });

            When(y => y.DataInicial.HasValue && y.DataFinal.HasValue, () =>
            {
                RuleFor(x => Math.Abs((x.DataInicial.Value.Date - x.DataFinal.Value.Date).TotalDays) > 30).Equal(false)
                    .WithMessage("O período não pode ser superior a 30 dias");
            });
        }
    }
}