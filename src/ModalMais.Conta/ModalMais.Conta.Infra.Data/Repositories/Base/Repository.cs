using System.Threading.Tasks;
using ModalMais.Conta.Domain.Entities;
using ModalMais.Conta.Domain.Interfaces;
using ModalMais.Conta.Infra.Data.Contexts;
using MongoDB.Driver;

namespace ModalMais.Conta.Infra.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;

        protected Repository(MongoDbContext context)
        {
            _collection = context._database.GetCollection<T>(BaseEntity.GetTableName<T>());
        }

        public async Task Add(T obj)
        {
            await _collection.InsertOneAsync(obj);
        }

        public async Task<T> Find(string key, string value)
        {
            return (await _collection.FindAsync(Builders<T>.Filter.Eq(key, value))).SingleOrDefault();
        }
    }
}