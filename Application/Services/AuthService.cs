using Application.DTOs.Auth;
using Application.Interfaces;
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

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
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
        public async Task<Usuario> LoginAsync(LoginDto dto)
        {
            string password = "123456";
            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            Console.WriteLine("Hash: " + hash);
            bool valid2 = BCrypt.Net.BCrypt.Verify("123456", hash);
            Console.WriteLine("Valido? " + valid2); // debe imprimir true

            var usuario = await _authRepository.ObtenerPorCorreoAsync(dto.Correo.ToLower().Trim());

            if (usuario == null)
                return null;

            bool valid = BCrypt.Net.BCrypt.Verify(dto.Password.Trim(), usuario.Password);
            return valid ? usuario : null;
        }

        public string GenerarJwt(Usuario usuario)
        {
            var key = Encoding.ASCII.GetBytes("MiSuperClaveSecreta123!MasLargaYSegura1234"); // o usa Configuration
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Rol)
        }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}