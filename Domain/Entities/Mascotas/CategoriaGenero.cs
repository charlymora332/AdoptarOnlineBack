namespace Domain.Entities.Mascotas
{
    public class CategoriaGenero
    {
        public byte Id { get; set; }
        public string Nombre { get; set; }  // "Macho", "Hembra"

        public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
    }
}