using AutoMapper;
using AdoptionApp;
using Application.Mascotas.Models.Requests;

namespace AdopcionAPI.Mappers;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        //CreateMap<MascotaDto, MascotaRequestDto>().ReverseMap();
    }
}