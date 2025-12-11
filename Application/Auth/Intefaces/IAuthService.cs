using Application.Auth.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Intefaces
{
    public interface IAuthService
    {
        StatusUsuario? ValidateTokenAndGetUserStatus(string token);

        Task<bool> RegistrarAsync(RegisterDto dto);

        Task<LoginResultDto> LoginAsync(LoginDto dto);

        public string GenerarJwt(Usuario usuario);

        // Refresh tokens
        Task<RefreshToken> GenerarTokenRefrescoAsync(int usuarioId);

        Task<(bool Valido, RefreshToken TokenRefresco)> ValidarTokenRefrescoAsync(string token);

        //Task<bool> ValidarTokenRefrescoAsync(string token, out RefreshToken tokenRefresco);
        Task RevocarTokenRefrescoAsync(string token);

        Task<Usuario> ObtenerUsuarioPorIdAsync(int usuarioId);
    }
}