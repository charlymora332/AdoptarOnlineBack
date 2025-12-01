using Application.DTOs.Mascota;
using Domain.Entities.Mascotas;

namespace Application.Interfaces
{
    /// <summary>
    /// Contrato del repositorio para manejar operaciones de Mascota.
    /// Define qué se puede hacer, sin importar cómo se implemente.
    /// </summary>
    public interface IMascotaAdminRepository

    //            Task<MascotaRequestDto> CrearAsync(MascotaRequestDto mascota);

    //Task<MascotaDetallesAdminDto?> ObtenerDetallesPorIdAdminAsync(int id);
    //Task<MascotaDetallesAdminDto?> ObtenerDetallesPorIdUserAsync(int id);
    //Task<List<MascotaPreviewAdminDto>> ObtenerListadoAdminAsync();
    //Task<List<MascotaPreviewAdminDto>> ObtenerListadoUserAsync();

    //Task ActualizarEstadoAdminAsync(int id, bool aprobado, bool disponible);
    //Task ActualizarDatosAdminAsync(int id);
    //Task ActualizarDatosUserAsync(int id);

    //Task EliminarPorIdUserAsync(int id);
    //Task EliminarPorIdAdminAsync(int id);

    {
        Task<MascotaDetallesAdminDto?> ObtenerInfoMascotaAsync(int id);

        Task<Mascota?> GetByIdAsync(int id);

        Task<List<MascotaPreviewAdminDto>> ObtenerTodasAsync();

        //Task AddAsync(Mascota mascota);

        Task ActualizarAsync(int id, bool aprobado, bool disponible);

        Task EliminarAsync(int id);
    }
}