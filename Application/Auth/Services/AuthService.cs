using Domain.Entities;

//using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Application.Auth.DTOs;
using Application.Auth.Intefaces;

namespace Application.Auth.Services
{
    public class AuthService : IAuthService

    {
        private readonly IAuthRepository _authRepository;

        // La clave secreta debe obtenerse de forma segura (ej: IConfiguration)
        private const string SecretString = "MiSuperClaveSecreta123!MasLargaYSegura1234";

        // Almacena la clave convertida a bytes una sola vez
        private readonly byte[] _secretKeyBytes;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
            _secretKeyBytes = Encoding.UTF8.GetBytes(SecretString);
        }

        // Registro
        public async Task<bool> RegistrarAsync(RegisterDto dto)
        {
            if (await _authRepository.ExisteCorreoAsync(dto.Correo))
                return false; // ya existe usuario

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo.ToLower().Trim(),
                Telefono = dto.Telefono,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _authRepository.AgregarAsync(usuario);
            await _authRepository.GuardarCambiosAsync();

            return true;
        }

        // Login
        public async Task<LoginResultDto> LoginAsync(LoginDto dto)
        {
            var usuario = await _authRepository.ObtenerPorCorreoAsync(dto.Correo.ToLower().Trim());
            if (usuario == null)
                return null;

            bool valid = BCrypt.Net.BCrypt.Verify(dto.Password.Trim(), usuario.Password);
            if (!valid)
                return null;

            var token = GenerarJwt(usuario);

            var respuesta = new LoginResultDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Rol = usuario.Rol,
                Token = token
            };

            return respuesta;
        }

        /// <summary>
        /// Valida el JWT y devuelve los datos del usuario o null si no es válido.
        /// </summary>
        public StatusUsuario? ValidateTokenAndGetUserStatus(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new StatusUsuario(); // No hay token
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // Debe validar la Signature
                IssuerSigningKey = new SymmetricSecurityKey(_secretKeyBytes),
                ValidateIssuer = false, // Asume que el Issuer no es estrictamente necesario validar
                ValidateAudience = false, // Asume que la Audience no es estrictamente necesario validar
                ClockSkew = TimeSpan.Zero, // Tolerancia de tiempo para expiración es cero
                ValidateLifetime = true // **CRUCIAL**: Valida la claim 'exp' (Expiración)
            };

            try
            {
                // 1. Validar el token. Si falla la firma o expira, lanzará una excepción.
                ClaimsPrincipal principal = tokenHandler.ValidateToken(
                    token,
                    validationParameters,
                    out SecurityToken validatedToken
                );

                // 2. Extraer las Claims del Payload
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = principal.FindFirst(ClaimTypes.Name)?.Value;
                var userRole = principal.FindFirst(ClaimTypes.Role)?.Value;

                // 3. Devolver los datos del usuario si las Claims clave existen
                if (string.IsNullOrEmpty(userId))
                {
                    // El token es técnicamente válido, pero faltan claims esenciales
                    return new StatusUsuario();
                }

                return new StatusUsuario
                {
                    Id = userId,
                    Name = userName,
                    Role = userRole
                };
            }
            catch (SecurityTokenExpiredException)
            {
                // El token ha expirado (exp)
                throw new SecurityTokenExpiredException("El token de acceso ha expirado.");
            }
            catch (Exception)
            {
                // Error de validación de firma o cualquier otro
                throw new SecurityTokenValidationException("El token es inválido o no está firmado correctamente.");
            }
        }

        public string GenerarJwt(Usuario usuario)
        {
            var key = Encoding.ASCII.GetBytes("MiSuperClaveSecreta123!MasLargaYSegura1234");
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                     //new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                     new Claim(ClaimTypes.Name, usuario.Nombre),
                     new Claim(ClaimTypes.Role, usuario.Rol)
        }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Refresh token
        public async Task<RefreshToken> GenerarTokenRefrescoAsync(int usuarioId)
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var expiracion = DateTime.UtcNow.AddDays(1);

            return await _authRepository.CrearTokenRefrescoAsync(usuarioId, token, expiracion);
        }

        public async Task<(bool Valido, RefreshToken TokenRefresco)> ValidarTokenRefrescoAsync(string token)
        {
            var tokenRefresco = await _authRepository.ObtenerTokenRefrescoAsync(token);
            bool valido = tokenRefresco != null && tokenRefresco.Expiracion > DateTime.UtcNow;
            return (valido, tokenRefresco);
        }

        public async Task<Usuario> ObtenerUsuarioPorIdAsync(int usuarioId)
        {
            return await _authRepository.ObtenerPorIdAsync(usuarioId);
        }

        public async Task RevocarTokenRefrescoAsync(string token)
        {
            var tokenRefresco = await _authRepository.ObtenerTokenRefrescoAsync(token);
            if (tokenRefresco != null)
            {
                tokenRefresco.Revocado = true;
                await _authRepository.ActualizarTokenRefrescoAsync(tokenRefresco);
            }
        }
    }
}