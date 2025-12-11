using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
        public DateTime CreadoEl { get; set; }
        public DateTime UltimoUso { get; set; }
        public bool Revocado { get; set; }
    }
}