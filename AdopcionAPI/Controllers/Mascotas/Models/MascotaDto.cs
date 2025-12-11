using System.ComponentModel.DataAnnotations;

namespace AdopcionAPI.Controllers.Mascotas.Models
{
    // 📍 Ubicación: AdopcionAPI.Controllers.Mascotas.Models (Capa de Presentación)

    public class MascotaDto // Nombre claro y conciso
    {
        // 1. Campos obligatorios
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no debe exceder {1} caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La edad es obligatoria.")]
        [Range(1, 600, ErrorMessage = "La edad debe estar entre 1 y 600 meses.")] // 50 años
        public byte EdadMeses { get; set; }

        [Required(ErrorMessage = "Se requiere el tamaño.")]
        public byte TamanioId { get; set; }

        // NOTA: Cambiamos TipoAnimal y Genero a IDs si son categorías
        // Si son strings, asegúrate de que sean válidos en la Capa de Aplicación.
        [Required]
        public string TipoAnimal { get; set; }

        [Required]
        public string Genero { get; set; }

        // 2. Colecciones (Usamos new() para inicialización)
        public List<byte> PersonalidadesId { get; set; } = new();

        // 3. Campos opcionales/largos
        [StringLength(500, ErrorMessage = "La descripción no debe exceder {1} caracteres.")]
        public string Descripcion { get; set; }

        // 4. Datos del Publicador
        [Required, EmailAddress]
        public string CorreoPublicador { get; set; }

        [Required]
        public string Ciudad { get; set; }

        // 5. El archivo de imagen (IFormFile)
        // El atributo [Required] aquí es crucial.
        [Required(ErrorMessage = "La imagen es obligatoria.")]
        public IFormFile Imagen { get; set; }

        // Eliminamos ImagenUrl y DescripciónLarga del Request inicial si vienen por el Form.
        // Si ImagenUrl se usa en otro contexto, pertenece a otro DTO o se calcula después.
    }
}