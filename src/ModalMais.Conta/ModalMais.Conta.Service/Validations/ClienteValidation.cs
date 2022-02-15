using FluentValidation;
using FluentValidation.Validators;
using ModalMais.Conta.Domain.Entities;
using ModalMais.Conta.Infra.Data.Utils;

namespace ModalMais.Conta.Service.Validations
{
    public class ClienteValidation : AbstractValidator<Cliente>
    {
        public ClienteValidation()
        {
            RuleFor(c => c.Nome)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                .MaximumLength(50)
                .WithMessage("O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.");

            RuleFor(c => c.Sobrenome)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                .MaximumLength(50)
                .WithMessage("O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                .MaximumLength(77)
                .WithMessage("O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.")
                .EmailAddress(EmailValidationMode.Net4xRegex).WithMessage("Email inválido.");

            RuleFor(c => c.Celular)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                .MaximumLength(11)
                .WithMessage("O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.")
                .Matches(@"^[0-9]{2}9[1-9]{1}[0-9]{7}$")
                .WithMessage("o campo {PropertyName} deve ser válido e somente números.")
                .Must(c => CodigosDDD.Contains(c)).WithMessage("Código de área inválido.");

            //RuleFor(c => CodigosDDD.Codigos.Contains(c.Celular)).Equal(true).WithMessage("scascasca");

            RuleFor(c => c.CPF)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                .Length(11).WithMessage("O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.")
                .IsValidCPF().WithMessage("CPF Invalido.");
        }
    }
}