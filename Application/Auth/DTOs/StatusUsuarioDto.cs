using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.DTOs
{
    // Un modelo simple para devolver los datos del usuario
    public class StatusUsuario
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public bool IsAuthenticated => !string.IsNullOrEmpty(Id);
    }
}