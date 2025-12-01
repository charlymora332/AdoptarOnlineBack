namespace Domain.Entities
{
    public class Imagen
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}