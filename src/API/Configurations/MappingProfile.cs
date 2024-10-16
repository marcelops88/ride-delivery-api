using API.DTOs.Requests;
using API.DTOs.Responses;
using AutoMapper;
using Domain.Entities;
using Domain.Models.Inputs;
using Domain.Models.Outputs;
using MongoDB.Bson;

namespace API.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MotoRequest, MotoInput>();

            CreateMap<MotoInput, Moto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ObjectId.GenerateNewId()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => 1));

            CreateMap<Moto, MotoOutput>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Ano, opt => opt.MapFrom(src => int.Parse(src.Ano)));

            CreateMap<MotoOutput, MotoResponse>();

            CreateMap<EntregadorRequest, EntregadorInput>();

            CreateMap<EntregadorOutput, EntregadorResponse>();

            CreateMap<EntregadorInput, Entregador>()
                .ForMember(dest => dest.CNPJ, opt => opt.MapFrom(src => src.Cnpj))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ObjectId.GenerateNewId()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => 1));
        }
    }
}
