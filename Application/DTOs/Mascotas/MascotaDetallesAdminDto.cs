using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Mascota
{
    public class MascotaDetallesAdminDto
    {
        public int Id { get; set; }
        public string ImagenUrl { get; set; }

        public string Nombre { get; set; }
        public string Genero { get; set; }

        public byte Tamanio { get; set; }
        public byte TipoAnimal { get; set; }

        //public string TipoAnimalString { get; set; }

        public string CategoriaEdad { get; set; }

        //Letra inicial de la categoría de género
        public byte EdadMeses { get; set; }

        public byte EdadAnios { get; set; }

        //public string Personalidades { get; set; }
        public List<string> Personalidades { get; set; } = new List<string>();

        //public List<byte> PersonalidadesId { get; set; } = new List<byte>();

        public string DescripcionCorta { get; set; }
        public string DescripcionLarga { get; set; }

        public bool Aprobado { get; set; }
        public bool Disponible { get; set; }
    }
}