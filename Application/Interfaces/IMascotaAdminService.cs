using Application.DTOs.Mascota;

namespace Application.Interfaces
{
    public interface IMascotaAdminService
    {
        Task<MascotaDetallesAdminDto?> ObtenerPorIdAsync(int id);

        Task<List<MascotaPreviewAdminDto>> ObtenerTodasAsync();

        Task ActualizarAsync(int id, bool aprobado, bool disponible);

        Task EliminarAsync(int id);
    }
}