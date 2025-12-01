using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Mascota
{
    /// <summary>
    /// Objeto de transferencia para exponer datos de Mascota hacia el cliente.
    /// Evita exponer directamente las entidades de dominio.
    /// </summary>
    public class MascotaRequestDto
    {
        public string Nombre { get; set; }
        public byte EdadMeses { get; set; }
        public byte TamanioId { get; set; }

        public string TipoAnimalId { get; set; }
        public List<byte> PersonalidadesId { get; set; } = new();
        public string Genero { get; set; }

        public string Descripcion { get; set; }
        public string ImagenUrl { get; set; }

        public string CorreoPublicador { get; set; }
        public string Ciudad { get; set; }

        public IFormFile Imagen { get; set; }
    }
}