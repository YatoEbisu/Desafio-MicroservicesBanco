using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ModalMais.Conta.Domain.Entities;

namespace ModalMais.Conta.Domain.Interfaces
{
    public interface IContaCorrenteService
    {
        Task Add(ContaCorrente obj);

        Task<List<Imagem>> SalvarImagem(List<IFormFile> imagens, string cpf, string banco, string numeroConta,
            string agencia);

        Task AddPix(string numeroConta, Pix obj);
        Task<ContaCorrente> FindByPix(Pix obj);
        Task Update(Cliente cliente);
    }
}