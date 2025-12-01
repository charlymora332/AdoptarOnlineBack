using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> ExisteCorreoAsync(string correo);

        Task<Usuario> ObtenerPorCorreoAsync(string correo);

        Task AgregarAsync(Usuario usuario);

        Task GuardarCambiosAsync();
    }
}