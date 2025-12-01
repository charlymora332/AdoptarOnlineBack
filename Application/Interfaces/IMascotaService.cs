using Application.DTOs.Mascota;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    /// <summary>
    /// Contrato del servicio de aplicación para manejar lógica de negocio
    /// relacionada con las mascotas.
    /// </summary>
    public interface IMascotaService
    {
        // Métodos existentes
        Task<MascotaRequestDto?> ObtenerPorIdAsync(int id);

        //Task<List<MascotaPreviewDto>> ObtenerTodasAsync();

        Task<int> CrearAsync(MascotaRequestDto mascota);

        Task<IEnumerable<MascotaPreviewDto>> ObtenerPreviewTodasMascotasAsync(
    byte? generoId,
    List<byte>? categoriasEdadId,
    List<byte>? tiposMascotaId,
    List<byte>? personalidadesMascotasId,
    List<byte>? tamaniosMascotaId,
    int? pagina,
    int? cantidadConsulta);

        //Task<IEnumerable<MascotaPreviewDto>> ObtenerPreviewTodasMascotasAsync();

        Task<MascotaInfoDto?> ObtenerMascotaMasInfoAsync(int id);
    }
}