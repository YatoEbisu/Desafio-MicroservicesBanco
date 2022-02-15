using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using ModalMais.Transferencia.Api.AutoMapper;
using ModalMais.Transferencia.Api.Interfaces;
using ModalMais.Transferencia.Api.Repository;
using ModalMais.Transferencia.Api.Services;
using Moq.AutoMock;
using Notie;
using Notie.Contracts;
using StackExchange.Redis;
using Xunit;

namespace ModalMais.Transferencia.Test.Fixtures
{
    [CollectionDefinition(nameof(MyFixtureCollection))]
    public class MyFixtureCollection : ICollectionFixture<MyFixture>
    {
    }

    internal class MyFixture
    {
        public readonly IContaCorrenteService _contaCorrenteService;
        private readonly IDistributedCache _distributedCache;
        private readonly AutoMocker _mocker;
        public readonly IRedisRepository _redisRepository;
        public readonly ITransferenciaRepository _transferenciaRepository;
        public readonly ITransferenciaService _transferenciaService;
        public readonly IMapper Mapper;
        public readonly AbstractNotifier Notifier;

        public MyFixture()
        {
            // AutoMapper
            _mocker = new();
            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperConfig()); });
            Mapper = mockMapper.CreateMapper();

            // Notifier
            Notifier = new Notifier();

            // Redis
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _redisRepository = new RedisRepository(_distributedCache);

            // SQL
            //_transferenciaRepository = new TransferenciaRepository(Factory.CreateContext());

            // ContaCorrente Service
            _contaCorrenteService = new ContaCorrenteService(
                _mocker.CreateInstance<IContaApi>(), Notifier);

            // Transferencia Service
            _transferenciaService = new TransferenciaService(
                _contaCorrenteService, _transferenciaRepository, Notifier, _redisRepository);
        }
    }
}