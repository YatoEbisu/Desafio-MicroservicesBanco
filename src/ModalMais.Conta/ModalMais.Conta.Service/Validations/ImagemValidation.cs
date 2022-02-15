using FluentValidation;
using ModalMais.Conta.Domain.Entities;
using ModalMais.Conta.Service.Dtos;

namespace ModalMais.Conta.Service.Validations
{
    internal class ImagemValidation : AbstractValidator<Imagem>
    {
        public ImagemValidation()
        {
            RuleFor(i => i.EnderecoImagem)
                .NotEmpty().WithMessage("O campo {PropertyName} é obrigatório.");

            RuleFor(i => i.Ativo)
                .NotNull().WithMessage("O campo {PropertyName} é obrigatório.");

            RuleFor(i => i.DataRegistro)
                .NotEmpty().WithMessage("O campo {PropertyName} é obrigatório.");

            RuleFor(i => i.Validado)
                .NotNull().WithMessage("O campo {PropertyName} é obrigatório.");
        }
    }

    public class ImagemRequestValidation : AbstractValidator<ImagemRequest>
    {
        public ImagemRequestValidation()
        {
            RuleFor(i => i.Agencia)
                .NotNull().WithMessage("O campo {PropertyName} é obrigatório.");

            RuleFor(i => i.Banco)
                .NotEmpty().WithMessage("O campo {PropertyName} é obrigatório.");

            RuleFor(i => i.Conta)
                .NotNull().WithMessage("O campo {PropertyName} é obrigatório.");

            RuleFor(c => c.CPF)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                .Length(11).WithMessage("O campo {PropertyName} não pode conter mais do que {MaxLength} caracteres.")
                .IsValidCPF().WithMessage("CPF Invalido.");

            RuleFor(c => c.Imagens.Count)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido.")
                .GreaterThan(0).WithMessage("O campo {PropertyName} precisa ser fornecido.");
        }
    }
}