using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using ModalMais.Transferencia.Api.Entities;
using ModalMais.Transferencia.Api.Interfaces;
using Newtonsoft.Json;

namespace ModalMais.Transferencia.Api.Repository
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly DistributedCacheEntryOptions memoryCacheEntryOptions;

        public RedisRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            memoryCacheEntryOptions = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) };
        }

        public async Task<List<TransferenciaPix>> GetExtrato(string key)
        {
            var listExtratos = new List<TransferenciaPix>();
            var result = await _distributedCache.GetStringAsync(key);
            if (result != null)
                listExtratos = JsonConvert.DeserializeObject<List<TransferenciaPix>>(result);

            return listExtratos;
        }

        public async Task SetExtrato(string key, IEnumerable<TransferenciaPix> value)
        {
            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(value), memoryCacheEntryOptions);
        }

        public async Task RemoveExtrato(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
    }
}