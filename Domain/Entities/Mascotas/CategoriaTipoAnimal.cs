namespace Domain.Entities.Mascotas
{
    /// <summary>
    /// Tipo de animal (Perro, Gato, Conejo, Ave).
    /// </summary>
    public class CategoriaTipoAnimal
    {
        public byte Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
    }
}