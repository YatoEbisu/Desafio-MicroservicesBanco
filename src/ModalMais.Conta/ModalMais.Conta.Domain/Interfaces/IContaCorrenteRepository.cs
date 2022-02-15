using System.Collections.Generic;
using System.Threading.Tasks;
using ModalMais.Conta.Domain.Entities;

namespace ModalMais.Conta.Domain.Interfaces
{
    public interface IContaCorrenteRepository : IRepository<ContaCorrente>
    {
        Task<ContaCorrente> GetByCPF(string CPF);
        Task<ContaCorrente> GetByEmail(string email);
        Task<ContaCorrente> GetByPix(string chave);

        Task AdicionarImagensPorCPF(string CPF, List<Imagem> obj);
        Task InativarImagemPorCPF(string cpf);

        Task AddPix(Pix obj, string cpf);
        Task UpdateDadosCliente(Cliente cliente);
    }
}