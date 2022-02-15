using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ModalMais.Conta.Domain.Entities;
using ModalMais.Conta.Domain.Interfaces;
using ModalMais.Conta.Service.Validations;
using Notie.Contracts;

namespace ModalMais.Conta.Service.Services
{
    public class ContaCorrenteService : Service<ContaCorrente>, IContaCorrenteService
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public ContaCorrenteService(IContaCorrenteRepository contaCorrenteRepository, AbstractNotifier notifier) :
            base(notifier)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public async Task Add(ContaCorrente obj)
        {
            if (await _contaCorrenteRepository.GetByCPF(obj.Cliente.CPF) != null)
                _notifier.AddNotification(new("Cliente.CPF", "Já existe uma conta corrente com esse CPF"));

            if (await _contaCorrenteRepository.GetByEmail(obj.Cliente.Email) != null)
                _notifier.AddNotification(new("Cliente.Email", "Já existe uma conta corrente com esse Email"));

            var validation = new ClienteValidation();
            var result = await validation.ValidateAsync(obj.Cliente);
            if (!result.IsValid)
                _notifier.AddNotificationsByFluent(result);

            if (_notifier.HasNotifications)
                return;

            await _contaCorrenteRepository.Add(obj);
        }

        public async Task Update(Cliente cliente)
        {
            var validation = new ClienteValidation();
            var result = await validation.ValidateAsync(cliente);
            if (!result.IsValid) _notifier.AddNotificationsByFluent(result);

            if (_notifier.HasNotifications) return;

            var contaCorrente = await _contaCorrenteRepository.GetByCPF(cliente.CPF);
            if (contaCorrente == null)
                _notifier.AddNotification(new("Conta", "CPF não cadastrado."));

            await _contaCorrenteRepository.UpdateDadosCliente(cliente);
        }

        public async Task AddPix(string numeroConta, Pix obj)
        {
            var validation = new PixValidation();
            var result = await validation.ValidateAsync(obj);
            if (!result.IsValid)
                _notifier.AddNotificationsByFluent(result);

            if (obj.Tipo == TipoChave.ChaveAleatoria)
                obj.Chave = Guid.NewGuid().ToString();

            var conta = await _contaCorrenteRepository.Find("NumeroConta", numeroConta);

            if (conta == null)
            {
                _notifier.AddNotification(new("NumeroConta", "A conta informada não está cadastrada."));
                return;
            }

            if (obj.Tipo == TipoChave.CPF && !conta.Cliente.CPF.Equals(obj.Chave))
                _notifier.AddNotification(new("Chave", "O CPF informado não pertence a conta."));

            if (_notifier.HasNotifications)
                return;

            await _contaCorrenteRepository.AddPix(obj, conta.Cliente.CPF);
        }

        public async Task<ContaCorrente> FindByPix(Pix obj)
        {
            var validation = new PixValidation();
            var result = await validation.ValidateAsync(obj);
            if (!result.IsValid)
            {
                _notifier.AddNotificationsByFluent(result);
                return null;
            }

            var contaCorrente = await _contaCorrenteRepository.GetByPix(obj.Chave);

            if (contaCorrente == null && result.IsValid)
                _notifier.AddNotification(new("Conta", "Conta não encontrada para a chave informada."));

            return _notifier.HasNotifications ? null : contaCorrente;
        }

        public async Task<List<Imagem>> SalvarImagem(List<IFormFile> imagens, string cpf, string banco,
            string numeroConta, string agencia)
        {
            var listaNovasImagens = new List<Imagem>();
            const long maxFileLength = 4194304;

            // validar CPF cadastrado no sistema
            var contaCorrente = await _contaCorrenteRepository.GetByCPF(cpf);

            if (contaCorrente == null)
                _notifier.AddNotification(new("Img_CPF", "CPF não cadastrado."));
            // validar CPF vinculado a conta informada
            else if (contaCorrente.NumeroBanco != banco || contaCorrente.Agencia != agencia ||
                     contaCorrente.NumeroConta != numeroConta)
                _notifier.AddNotification(new("Img_Dados", "CPF não compatível com dados Bancários."));

            // fazer mapping
            foreach (var i in imagens)
            {
                var isValid = true;
                var imagem = new Imagem($"https://www.amazonS3.com/{Guid.NewGuid()}{i.FileName}");

                var validator = new ImagemValidation();
                var validation = validator.Validate(imagem);

                validation.Errors.ForEach(e => _notifier.AddNotification(new(e.ErrorCode, e.ErrorMessage)));

                // validar imagem menor 4MB
                if (i.Length > maxFileLength)
                {
                    isValid = false;
                    _notifier.AddNotification(new("fileSize", $"O arquivo {i.FileName} tem que ser menor do que 4MB."));
                }

                // validar imagem PNG
                if (i.ContentType != "image/png")
                {
                    isValid = false;
                    _notifier.AddNotification(new("fileType", $"O arquivo {i.FileName} tem que ser PNG."));
                }

                if (isValid && validation.IsValid)
                {
                    imagem.DefineDataValidacao();

                    if (ValidarImagensPorAlgoritmo(imagem)) imagem.DefineValidado();

                    listaNovasImagens.Add(imagem);
                }
            }

            if (_notifier.HasNotifications) return listaNovasImagens;

            if (listaNovasImagens.FindAll(i => i.Validado).Any())
            {
                // consultar ultima imagem valida do banco e setar invalida
                if (contaCorrente.Imagens.Find(i => i.Ativo) != null)
                    await _contaCorrenteRepository.InativarImagemPorCPF(cpf);

                // se ja tiver imagem válida, incluir a nova como ativa e desativar a anterior
                listaNovasImagens.FindAll(i => i.Validado).LastOrDefault().DefineAtivo();
            }

            await _contaCorrenteRepository.AdicionarImagensPorCPF(cpf, listaNovasImagens);

            return listaNovasImagens;
        }

        private bool ValidarImagensPorAlgoritmo(Imagem imagens)
        {
            var randomNumber = new Random().Next(100);
            return randomNumber % 2 == 0;
        }
    }
}