namespace Domain.Entities.Mascotas
{
    public class Mascota
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public byte EdadMeses { get; set; }
        public string ImagenUrl { get; set; }
        public string DescripcionCorta { get; set; }
        public string DescripcionLarga { get; set; }

        // Relaciones con otras tablas
        public byte CategoriaEdadId { get; set; }

        public CategoriaEdad CategoriaEdad { get; set; }
        public byte TamanioId { get; set; }
        public Tamanio Tamanio { get; set; }
        public byte TipoAnimalId { get; set; }
        public CategoriaTipoAnimal? CategoriaTipoAnimal { get; set; }

        public byte CategoriaGeneroId { get; set; }
        public CategoriaGenero CategoriaGenero { get; set; }

        public ICollection<MascotaPersonalidad> MascotaPersonalidades { get; set; } = new List<MascotaPersonalidad>();

        //public ICollection<MascotaPersonalidad> PersonalidadesId { get; set; } = new List<MascotaPersonalidad>();

        public bool Disponible { get; set; }
        public bool Aprobado { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public string Ciudad { get; set; }
    }
}