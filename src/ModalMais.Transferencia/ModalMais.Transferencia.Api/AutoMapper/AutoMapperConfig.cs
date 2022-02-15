using AutoMapper;
using ModalMais.Transferencia.Api.DTOs;
using ModalMais.Transferencia.Api.Entities;

namespace ModalMais.Transferencia.Api.AutoMapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<TransferenciaPix, TransferenciaPixRequest>().ReverseMap();
            CreateMap<TransferenciaPix, TransferenciaPixResponse>().ReverseMap();
            CreateMap<Extrato, ExtratoRequest>().ReverseMap();
            CreateMap<Extrato, ExtratoResponse>().ReverseMap();
        }
    }
}