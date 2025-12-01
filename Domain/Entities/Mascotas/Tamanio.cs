namespace Domain.Entities.Mascotas
{
    public class Tamanio
    {
        public byte Id { get; set; }
        public string TamanioMascota { get; set; }  // "Macho", "Hembra"

        public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
    }
}