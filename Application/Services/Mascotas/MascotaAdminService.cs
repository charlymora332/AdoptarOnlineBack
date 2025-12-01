using Application.DTOs.Mascota;
using Application.Interfaces;
using Domain.Entities.Mascotas;
using Domain.Exceptions;

namespace Application.Services.Mascotas
{
    public class MascotaAdminService : IMascotaAdminService
    {
        private readonly IMascotaRepository _mascotaAdminRepository;

        public MascotaAdminService(IMascotaRepository mascotaAdminRepository)
        {
            _mascotaAdminRepository = mascotaAdminRepository;
        }

        public async Task<List<MascotaPreviewAdminDto>> ObtenerTodasAsync()
        {
            try
            {
                var mascotas = await _mascotaAdminRepository.GetMascotasAdminAsync();
                return mascotas;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error al intentar obtener las mascotas", ex);
            }
        }

        public async Task EliminarAsync(int id)
        {
            await _mascotaAdminRepository.EliminarAsync(id);
        }

        public async Task ActualizarAsync(int id, bool aprobado, bool disponible)
        {
            await _mascotaAdminRepository.ActualizarAsync(id, aprobado, disponible);
        }

        public async Task<MascotaDetallesAdminDto?> ObtenerPorIdAsync(int id)
        {
            var mascota = await _mascotaAdminRepository.GetMascotaAdminByIdAsync(id);

            if (mascota == null) return null;

            return mascota;
        }
    }
}