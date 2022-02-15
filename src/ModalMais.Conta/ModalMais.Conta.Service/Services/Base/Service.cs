using ModalMais.Conta.Domain.Entities;
using Notie.Contracts;

namespace ModalMais.Conta.Service.Services
{
    public class Service<T> where T : BaseEntity
    {
        protected readonly AbstractNotifier _notifier;

        public Service(AbstractNotifier notifier)
        {
            _notifier = notifier;
        }
    }
}