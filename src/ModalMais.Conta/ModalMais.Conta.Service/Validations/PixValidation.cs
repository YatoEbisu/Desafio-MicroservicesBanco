using FluentValidation;
using FluentValidation.Validators;
using ModalMais.Conta.Domain.Entities;
using ModalMais.Conta.Infra.Data.Utils;
using ModalMais.Conta.Service.Dtos;

namespace ModalMais.Conta.Service.Validations
{
    public class PixValidation : AbstractValidator<Pix>
    {
        public PixValidation()
        {
            RuleFor(p => p.Tipo).IsInEnum();

            When(p => p.Tipo == TipoChave.CPF, () =>
            {
                RuleFor(p => p.Chave)
                    .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                    .Length(11).WithMessage(
                        "O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.")
                    .IsValidCPF().WithMessage("CPF Invalido.");
            });
            When(p => p.Tipo == TipoChave.Celular, () =>
            {
                RuleFor(p => p.Chave)
                    .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                    .MaximumLength(11)
                    .WithMessage("O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.")
                    .Matches(@"^[0-9]{2}9[1-9]{1}[0-9]{7}$")
                    .WithMessage("o campo {PropertyName} deve ser válido e somente números.")
                    .Must(c => CodigosDDD.Contains(c)).WithMessage("Código de área inválido.");
            });
            When(p => p.Tipo == TipoChave.Email, () =>
            {
                RuleFor(p => p.Chave)
                    .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                    .MaximumLength(77)
                    .WithMessage("O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.")
                    .EmailAddress(EmailValidationMode.Net4xRegex).WithMessage("Email inválido.");
            });
        }
    }

    public class PixRequestValidation : AbstractValidator<PixRequest>
    {
        public PixRequestValidation()
        {
            RuleFor(p => p.Tipo).IsInEnum();

            RuleFor(p => p.NumeroConta)
                .NotNull().WithMessage("O campo {PropertyName} é obrigatório.");

            When(p => p.Tipo == TipoChave.CPF, () =>
            {
                RuleFor(p => p.Chave)
                    .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                    .Length(11).WithMessage(
                        "O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.")
                    .IsValidCPF().WithMessage("CPF Invalido.");
            });
            When(p => p.Tipo == TipoChave.Celular, () =>
            {
                RuleFor(p => p.Chave)
                    .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                    .MaximumLength(11)
                    .WithMessage("O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.")
                    .Matches(@"^[0-9]{2}9[1-9]{1}[0-9]{7}$")
                    .WithMessage("o campo {PropertyName} deve ser válido e somente números.")
                    .Must(c => CodigosDDD.Contains(c)).WithMessage("Código de área inválido.");
            });
            When(p => p.Tipo == TipoChave.Email, () =>
            {
                RuleFor(p => p.Chave)
                    .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                    .MaximumLength(77)
                    .WithMessage("O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.")
                    .EmailAddress(EmailValidationMode.Net4xRegex).WithMessage("Email inválido.");
            });
        }
    }
}