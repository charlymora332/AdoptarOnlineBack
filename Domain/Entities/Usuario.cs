using Domain.Entities.Mascotas;

namespace Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; } // PK
        public string Nombre { get; set; }
        public string Correo { get; set; } // se usará para login
        public string Telefono { get; set; }
        public string Password { get; set; } // contraseña hasheada
        public string Rol { get; set; } = "Usuario"; // por defecto
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public List<Mascota> Mascotas { get; set; } = new List<Mascota>();
    }
}