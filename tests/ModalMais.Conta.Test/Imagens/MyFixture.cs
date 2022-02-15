using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.AspNetCore.Http;
using ModalMais.Conta.Api.AutoMapper;
using ModalMais.Conta.Domain.Entities;
using ModalMais.Conta.Domain.Interfaces;
using ModalMais.Conta.Infra.Data.Repositories;
using ModalMais.Conta.Service.Dtos;
using ModalMais.Conta.Service.Services;
using MongoDB.Driver;
using Moq;
using Notie;
using Notie.Contracts;
using Xunit;

namespace ModalMais.Conta.Test.Imagens
{
    [CollectionDefinition(nameof(MyFixtureCollection))]
    public class MyFixtureCollection : ICollectionFixture<MyFixture>
    {
    }

    public class MyFixture : IDisposable
    {
        private readonly DbFixture _dbFixture;
        public readonly IContaCorrenteRepository ContaCorrenteRepository;
        public readonly IContaCorrenteService ContaCorrenteService;
        public readonly IMapper Mapper;
        public readonly AbstractNotifier Notifier;

        public MyFixture()
        {
            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperConfig()); });
            Mapper = mockMapper.CreateMapper();

            Notifier = new Notifier();
            _dbFixture = new();
            ContaCorrenteRepository = new ContaCorrenteRepository(_dbFixture.DbContext);
            ContaCorrenteService = new ContaCorrenteService(ContaCorrenteRepository, Notifier);
        }

        public void Dispose()
        {
            var client = new MongoClient(_dbFixture.ConnectionString);
            client.DropDatabase(_dbFixture.DatabaseName);
        }

        public ImagemRequest GerarImagemRequest(int imagens, ContaCorrente contaCorrente)
        {
            var baseStream = new Mock<Stream>();
            var extensao = "png";
            var nameFaker = new Faker("pt_BR");

            var x = new Faker<FormFile>("pt_BR").CustomInstantiator
            (
                file => new(
                    baseStream.Object,
                    0, //
                    nameFaker.Random.Number(1, 3999999), // Length
                    "name", // Name
                    nameFaker.Name.FullName() + "." + extensao // File Name
                )
                {
                    Headers = new HeaderDictionary(),
                    ContentType = $"image/{extensao}"
                }
            );

            var result = new List<IFormFile>();

            result.AddRange(x.Generate(imagens));

            //x.Generate(imagens).Select(i => { i.ContentType = "nossa string"; return i; }).ToList();

            var imagesRequest = new ImagemRequest
            {
                CPF = contaCorrente.Cliente.CPF,
                Banco = contaCorrente.NumeroBanco,
                Agencia = contaCorrente.Agencia,
                Conta = contaCorrente.NumeroConta,
                Imagens = result
            };

            return imagesRequest;
        }

        public ImagemRequest GerarImagemRequestInvalidoPorExtensao(int imagens, ContaCorrente contaCorrente)
        {
            var baseStream = new Mock<Stream>();
            var extensao = "jpg";
            var nameFaker = new Faker("pt_BR");

            var x = new Faker<FormFile>("pt_BR").CustomInstantiator
            (
                file => new(
                    baseStream.Object,
                    0, //
                    nameFaker.Random.Number(1, 3999999), // Length
                    "name", // Name
                    nameFaker.Name.FullName() + "." + extensao // File Name
                )
                {
                    Headers = new HeaderDictionary(),
                    ContentType = $"image/{extensao}"
                }
            );

            var result = new List<IFormFile>();

            result.AddRange(x.Generate(imagens));

            //x.Generate(imagens).Select(i => { i.ContentType = "nossa string"; return i; }).ToList();

            var imagesRequest = new ImagemRequest
            {
                CPF = contaCorrente.Cliente.CPF,
                Banco = contaCorrente.NumeroBanco,
                Agencia = contaCorrente.Agencia,
                Conta = contaCorrente.NumeroConta,
                Imagens = result
            };

            return imagesRequest;
        }

        public ImagemRequest GerarImagemRequestInvalidoPorTamanho(int imagens, ContaCorrente contaCorrente)
        {
            var baseStream = new Mock<Stream>();
            var extensao = "png";
            var nameFaker = new Faker("pt_BR");
            var x = new Faker<FormFile>("pt_BR").CustomInstantiator
            (
                file => new(
                    baseStream.Object,
                    0, //
                    nameFaker.Random.Number(5000000, 6000000), // Length
                    "name", // Name
                    nameFaker.Name.FullName() + "." + extensao // File Name
                )
                {
                    Headers = new HeaderDictionary(),
                    ContentType = $"image/{extensao}"
                }
            );

            var result = new List<IFormFile>();

            result.AddRange(x.Generate(imagens));

            //x.Generate(imagens).Select(i => { i.ContentType = "nossa string"; return i; }).ToList();

            var imagesRequest = new ImagemRequest
            {
                CPF = contaCorrente.Cliente.CPF,
                Banco = contaCorrente.NumeroBanco,
                Agencia = contaCorrente.Agencia,
                Conta = contaCorrente.NumeroConta,
                Imagens = result
            };

            return imagesRequest;
        }

        public IEnumerable<ContaCorrente> GerarContaCorrendeValida(int contas)
        {
            /*
             * string cpf, string nome, string sobrenome, string celular, string email
             */
            var paramFaker = new Faker("pt_BR");
            var nome = paramFaker.Name.FirstName();
            var sobrenome = paramFaker.Name.LastName();

            var ClienteFake = new Faker<Cliente>("pt_BR")
                .CustomInstantiator(cliente => new(
                    "",
                    nome,
                    sobrenome,
                    "",
                    cliente.Internet.Email(nome, sobrenome)
                ))
                .RuleFor(p => p.CPF, f => f.Person.Cpf(false))
                .RuleFor(p => p.Celular, f => f.Phone.PhoneNumber("##9########"));

            var ContaCorrenteFaker = new Faker<ContaCorrente>("pt_BR")
                .CustomInstantiator(cliente => new()
                {
                    Cliente = ClienteFake.Generate()
                });


            return ContaCorrenteFaker.Generate(contas);
        }

        public static ContaCorrente GerarContaCorrenteValida()
        {
            var paramFaker = new Faker("pt_BR");
            var nome = paramFaker.Name.FirstName();
            var sobrenome = paramFaker.Name.LastName();

            var ClienteFake = new Faker<Cliente>("pt_BR")
                .CustomInstantiator(cliente => new(
                    "",
                    nome,
                    sobrenome,
                    "",
                    cliente.Internet.Email(nome, sobrenome)
                ))
                .RuleFor(p => p.CPF, f => f.Person.Cpf(false))
                .RuleFor(p => p.Celular, f => f.Phone.PhoneNumber("889########"));

            var ContaCorrenteFaker = new Faker<ContaCorrente>("pt_BR")
                .CustomInstantiator(cliente => new()
                {
                    Cliente = ClienteFake.Generate()
                });


            return ContaCorrenteFaker.Generate();
        }
    }
}