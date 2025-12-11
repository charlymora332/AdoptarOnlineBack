// Proyecto: Application
// Clase: ApplicationProfile.cs

using AutoMapper;
using AdoptionApp; // Asegúrate de importar los namespaces necesarios
using Application.Mascotas.Models.Requests;
using AdopcionAPI.Controllers.Mascotas.Models;

namespace Application.Mappers;

public class ApiProfile : Profile
{
    public ApiProfile()
    {
        CreateMap<MascotaDto, MascotaRequestDto>().ReverseMap();
    }
}