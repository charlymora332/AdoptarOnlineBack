namespace Application.DTOs.Mascota
{
    public class MascotaInfoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public byte EdadMeses { get; set; }
        public byte EdadAnios { get; set; }

        // Lista de nombres de personalidades
        public List<string> Personalidades { get; set; } = new List<string>();

        public string DescripcionLarga { get; set; }

        public string ImagenUrl { get; set; }
    }
}