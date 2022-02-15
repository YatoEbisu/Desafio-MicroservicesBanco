using System.Threading.Tasks;

namespace ModalMais.Conta.Domain.Interfaces
{
    public interface IRepository<T>
    {
        Task Add(T obj);
        Task<T> Find(string key, string value);
    }
}