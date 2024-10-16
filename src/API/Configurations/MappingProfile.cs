using API.DTOs.Requests;
using AutoMapper;
using Domain.Entities;
using Domain.Models.Inputs;

namespace API.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MotoRequest, MotoInput>();

            CreateMap<MotoInput, Moto>()
                .ForMember(dest => dest.Ano, opt => opt.MapFrom(src => src.Ano.ToString()));

        }
    }
}
