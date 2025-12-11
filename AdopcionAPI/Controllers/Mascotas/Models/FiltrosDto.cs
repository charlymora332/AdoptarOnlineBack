using System.ComponentModel.DataAnnotations;

namespace AdopcionAPI.Controllers.Mascotas.Models
{
    public class FiltrosDTO
    {
        [Range(1, 2)]
        public byte? Genero { get; set; }

        [Range(1, 3)]
        public List<byte>? CategoriaEdad { get; set; }

        [Range(1, 3)]
        public List<byte>? TipoMascota { get; set; }

        [Range(1, 6)]
        public List<byte>? PersonalidadesMascotasId { get; set; }

        [Range(1, 3)]
        public List<byte>? TamanioMascota { get; set; }

        [Range(1, int.MaxValue)]
        public int? Pagina { get; set; }

        [Range(1, 100)]
        public int? CantidadConsulta { get; set; }
    }
}