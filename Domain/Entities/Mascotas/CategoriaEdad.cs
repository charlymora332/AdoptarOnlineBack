namespace Domain.Entities.Mascotas
{
    /// <summary>
    /// Categoría de edad de la mascota (Cachorro, Joven, Viejo).
    /// </summary>
    public class CategoriaEdad
    {
        public byte Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
    }
}