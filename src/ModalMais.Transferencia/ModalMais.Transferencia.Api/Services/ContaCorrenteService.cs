using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModalMais.Transferencia.Api.DTOs;
using ModalMais.Transferencia.Api.Interfaces;
using Notie.Contracts;
using Notie.Models;
using Refit;

namespace ModalMais.Transferencia.Api.Services
{
    public class ContaCorrenteService : IContaCorrenteService

    {
        private readonly IContaApi _api;
        private readonly AbstractNotifier _notifier;

        public ContaCorrenteService(IContaApi api, AbstractNotifier notifier)
        {
            _api = api;
            _notifier = notifier;
        }

        public async Task<ContaPixResponse> ObterContaPix(ContaPixRequest model)
        {
            try
            {
                var conta = await _api.ObterContaPix(model);
                return conta;
            }
            catch (ApiException ex)
            {
                var errors = await ex.GetContentAsAsync<List<Notification>>();
                _notifier.AddNotifications(errors);
                return null;
            }
            catch (Exception ex)
            {
                throw new("Erro na comunicação com a API de Contas", ex);
            }
        }
    }
}