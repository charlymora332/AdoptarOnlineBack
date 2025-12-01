namespace Domain.Entities.Mascotas
{
    /// <summary>
    /// Tabla intermedia para relación muchos a muchos
    /// entre Mascota y CategoriaPersonalidad.
    /// </summary>
    public class MascotaPersonalidad
    {
        public int MascotaId { get; set; }
        public Mascota? Mascota { get; set; }

        public byte CategoriaPersonalidadId { get; set; }

        public CategoriaPersonalidad? CategoriaPersonalidad { get; set; }
    }
}