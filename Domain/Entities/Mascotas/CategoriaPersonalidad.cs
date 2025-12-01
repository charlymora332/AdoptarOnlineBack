namespace Domain.Entities.Mascotas
{
    /// <summary>
    /// Personalidad de la mascota (Juguetón, Bravo, Cuidador, Dormilón).
    /// </summary>
    public class CategoriaPersonalidad
    {
        public byte Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public ICollection<MascotaPersonalidad> MascotaPersonalidades { get; set; } = new List<MascotaPersonalidad>();
    }
}