using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Intefaces
{
    public interface IAuthRepository
    {
        Task<bool> ExisteCorreoAsync(string correo);

        Task<Usuario> ObtenerPorCorreoAsync(string correo);

        Task AgregarAsync(Usuario usuario);

        Task GuardarCambiosAsync();

        // Refresh tokens
        Task<RefreshToken> CrearTokenRefrescoAsync(int usuarioId, string token, DateTime expiracion);

        Task<RefreshToken> ObtenerTokenRefrescoAsync(string token);

        Task ActualizarTokenRefrescoAsync(RefreshToken tokenRefresco);

        Task<Usuario> ObtenerPorIdAsync(int id);
    }
}