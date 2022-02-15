using System.Collections.Generic;
using System.Threading.Tasks;
using ModalMais.Conta.Domain.Entities;
using ModalMais.Conta.Domain.Interfaces;
using ModalMais.Conta.Infra.Data.Contexts;
using MongoDB.Driver;

namespace ModalMais.Conta.Infra.Data.Repositories
{
    public class ContaCorrenteRepository : Repository<ContaCorrente>, IContaCorrenteRepository
    {
        public ContaCorrenteRepository(MongoDbContext context) : base(context)
        {
        }

        public async Task<ContaCorrente> GetByCPF(string CPF)
        {
            return (await _collection.FindAsync(Builders<ContaCorrente>.Filter.Eq("Cliente.CPF", CPF)))
                .FirstOrDefault();
        }

        public async Task<ContaCorrente> GetByEmail(string email)
        {
            return (await _collection.FindAsync(Builders<ContaCorrente>.Filter.Eq("Cliente.Email", email)))
                .SingleOrDefault();
        }

        public async Task AdicionarImagensPorCPF(string cpf, List<Imagem> listaImagens)
        {
            var ContaCorrenteBuilder = Builders<ContaCorrente>.Filter;
            var filter = ContaCorrenteBuilder.Eq(x => x.Cliente.CPF, cpf);

            var update = Builders<ContaCorrente>.Update.PushEach(x => x.Imagens, listaImagens);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task InativarImagemPorCPF(string cpf)
        {
            var ContaCorrenteBuilder = Builders<ContaCorrente>.Filter;
            var ImagemBuilder = Builders<Imagem>.Filter;

            var filter = ContaCorrenteBuilder.Eq(x => x.Cliente.CPF, cpf)
                         &
                         ContaCorrenteBuilder.ElemMatch(
                             x => x.Imagens,
                             ImagemBuilder.Eq(x => x.Ativo, true)
                         );

            var update = Builders<ContaCorrente>.Update.Set(x => x.Imagens[-1].Ativo, false);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task AddPix(Pix obj, string cpf)
        {
            var ContaCorrenteBuilder = Builders<ContaCorrente>.Filter;
            var filter = ContaCorrenteBuilder.Eq(x => x.Cliente.CPF, cpf);

            var update = Builders<ContaCorrente>.Update.Push(x => x.ChavesPix, obj);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<ContaCorrente> GetByPix(string chave)
        {
            var filter = Builders<ContaCorrente>.Filter.And(
                Builders<ContaCorrente>.Filter.ElemMatch(x => x.ChavesPix, Builders<Pix>.Filter
                    .And(
                        Builders<Pix>.Filter.Eq(y => y.Chave, chave)
                    )));

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task UpdateDadosCliente(Cliente cliente)
        {
            var ContaCorrenteBuilder = Builders<ContaCorrente>.Filter;

            var filter = ContaCorrenteBuilder.Eq(x => x.Cliente.CPF, cliente.CPF);

            var update = Builders<ContaCorrente>.Update.Set(x => x.Cliente, cliente);

            await _collection.UpdateOneAsync(filter, update);
        }
    }
}