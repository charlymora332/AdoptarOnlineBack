using System.ComponentModel.DataAnnotations;

namespace Application.Mascotas.Models.Requests
{
    public class FiltrosRequestDTO
    {
        public byte? GeneroId { get; set; }

        public List<byte>? CategoriaEdadId { get; set; }

        public List<byte>? TipoMascotaId { get; set; }

        public List<byte>? PersonalidadesMascotasId { get; set; }

        public List<byte>? TamanioMascotaId { get; set; }

        public int? Pagina { get; set; }

        public int? CantidadConsulta { get; set; }
    }
}