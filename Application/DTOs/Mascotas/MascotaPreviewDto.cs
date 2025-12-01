namespace Application.DTOs.Mascota
{
    public class MascotaPreviewDto
    {
        public int Id { get; set; }
        public string ImagenUrl { get; set; }

        public string Nombre { get; set; }
        public byte Genero { get; set; }
        public byte Tamanio { get; set; }
        public byte TipoAnimal { get; set; }
        public string TipoAnimalString { get; set; }

        public string CategoriaEdad { get; set; }

        // Letra inicial de la categoría de género
        public byte EdadMeses { get; set; }

        public byte EdadAnios { get; set; }

        //public string Personalidades { get; set; }
        public List<string> Personalidades { get; set; } = new List<string>();

        public List<byte> PersonalidadesId { get; set; } = new List<byte>();

        public string DescripcionCorta { get; set; }

        public bool Aprobado { get; set; }
        public bool Disponible { get; set; }
    }
}