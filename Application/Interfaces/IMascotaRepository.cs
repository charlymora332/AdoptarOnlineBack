using Application.DTOs.Mascota;
using Domain.Entities.Mascotas;

namespace Application.Interfaces
{
    /// <summary>
    /// Contrato del repositorio para manejar operaciones de Mascota.
    /// Define qué se puede hacer, sin importar cómo se implemente.
    /// </summary>
    public interface IMascotaRepository
    {
        Task<Mascota?> GetByIdAsync(int id);

        Task<List<MascotaPreviewDto>> GetMascotasUserAsync(byte? generoId,
    List<byte>? categoriasEdadId,
    List<byte>? tiposMascotaId,
    List<byte>? personalidadesMascotasId,
    List<byte>? tamaniosMascotaId,
    int? pagina,
    int? cantidadConsulta);

        Task AddAsync(Mascota mascota);

        Task<MascotaInfoDto?> GetMascotaDetallesUserAsync(int id);

        Task ActualizarAsync(int id, bool aprobado, bool disponible);

        Task<MascotaDetallesAdminDto?> GetMascotaAdminByIdAsync(int id);

        Task<List<MascotaPreviewAdminDto>> GetMascotasAdminAsync();

        Task EliminarAsync(int id);
    }
}

//Task<List<MascotaPreviewDto>> ObtenerPreviewTodasMascotasAsync();
//Task<List<MascotaPreviewDto>> GetAllAsync();

// Métodos específicos para preview y detalle