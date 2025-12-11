using Application.Mascotas.Models.Requests;
using Application.Mascotas.Models.Responses;
using Domain.Entities.Mascotas;
using Microsoft.AspNetCore.Http;

namespace Application.Mascotas.Intefaces
{
    /// <summary>
    /// Contrato del servicio de aplicación para manejar lógica de negocio
    /// relacionada con las mascotas.
    /// </summary>
    public interface IMascotaService
    {
        // Métodos existentes
        Task<int> CrearAsync(MascotaRequestDto mascota);

        Task ActualizarAsync(int id, bool aprobado, bool disponible);

        Task EliminarAsync(int id);

        Task<MascotaDetallesAdminDto?> ObtenerPorIdAdminAsync(int id);

        Task<List<MascotaPreviewAdminDto>> ObtenerTodasAdminAsync();

        Task<MascotaInfoDto?> ObtenerPorIdUserAsync(int id);

        Task<IEnumerable<MascotaPreviewDto>> ObtenerTodasUserAsync(FiltrosRequestDTO filtros);

        //Task<IEnumerable<MascotaPreviewDto>> ObtenerPreviewTodasMascotasAsync();

        //Task<MascotaInfoDto?> ObtenerMascotaMasInfoAsync(int id);
    }
}