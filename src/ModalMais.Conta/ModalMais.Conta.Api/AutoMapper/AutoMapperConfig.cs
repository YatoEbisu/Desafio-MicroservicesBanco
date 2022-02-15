using AutoMapper;
using ModalMais.Conta.Domain.Entities;
using ModalMais.Conta.Service.Dtos;

namespace ModalMais.Conta.Api.AutoMapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<ContaCorrente, ContaCorrenteRequest>().ReverseMap();
            CreateMap<ContaCorrente, ContaCorrenteResponse>().ReverseMap();
            CreateMap<Cliente, ClienteRequest>().ReverseMap();
            CreateMap<Cliente, ClienteResponse>().ReverseMap();
            CreateMap<Cliente, AtualizaClienteRequest>().ReverseMap();
            CreateMap<Imagem, ImagemResponse>().ReverseMap();
            CreateMap<Pix, PixRequest>().ReverseMap();
            CreateMap<Pix, PixResponse>().ReverseMap();
            CreateMap<ContaPixRequest, Pix>().ReverseMap();
            CreateMap<ContaCorrente, ContaPixResponse>()
                .ForMember(d => d.Nome, opt => opt.MapFrom(s => s.Cliente.Nome))
                .ForMember(d => d.Sobrenome, opt => opt.MapFrom(s => s.Cliente.Sobrenome));
        }
    }
}