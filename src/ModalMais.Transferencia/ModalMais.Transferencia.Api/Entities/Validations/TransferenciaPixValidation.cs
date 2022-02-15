using FluentValidation;
using FluentValidation.Validators;
using ModalMais.Transferencia.Api.Utils;

namespace ModalMais.Transferencia.Api.Entities.Validations
{
    public class TransferenciaPixValidation : AbstractValidator<TransferenciaPix>
    {
        public TransferenciaPixValidation()
        {
            RuleFor(t => t.TipoChave)
                .IsInEnum().WithMessage("Campo {PropertyValue} inválido")
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.");

            RuleFor(t => t.Valor)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                .InclusiveBetween(1, TransferenciaPix.LIMITE_UNITARIO)
                .WithMessage("O campo {PropertyName} deve estar entre {From} e {To}.");

            RuleFor(t => t.Descricao)
                .MaximumLength(30)
                .WithMessage("O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.");

            When(p => p.TipoChave == TipoChave.CPF, () =>
            {
                RuleFor(p => p.Chave)
                    .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                    .Length(11).WithMessage(
                        "O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.")
                    .IsValidCPF().WithMessage("CPF Invalido.");
            });
            When(p => p.TipoChave == TipoChave.Celular, () =>
            {
                RuleFor(p => p.Chave)
                    .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                    .MaximumLength(11)
                    .WithMessage("O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.")
                    .Matches(@"^[0-9]{2}9[1-9]{1}[0-9]{7}$")
                    .WithMessage("o campo {PropertyName} deve ser válido e somente números.")
                    .Must(CodigosDDD.Contains).WithMessage("Código de área inválido.");
            });
            When(p => p.TipoChave == TipoChave.Email, () =>
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