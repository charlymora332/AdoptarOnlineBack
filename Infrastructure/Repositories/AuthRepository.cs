using AdopcionOnline.Infrastructure;
using Application.Auth.Intefaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteCorreoAsync(string correo)
        {
            return await _context.Usuarios.AnyAsync(u => u.Correo == correo);
        }

        public async Task<Usuario> ObtenerPorCorreoAsync(string correo)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
        }

        public async Task AgregarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> CrearTokenRefrescoAsync(int usuarioId, string token, DateTime expiracion)
        {
            var tokenRefresco = new RefreshToken
            {
                UsuarioId = usuarioId,
                Token = token,
                CreadoEl = DateTime.UtcNow,
                UltimoUso = DateTime.UtcNow,
                Expiracion = expiracion,
                Revocado = false
            };

            await _context.RefreshTokens.AddAsync(tokenRefresco);
            await _context.SaveChangesAsync();
            return tokenRefresco;
        }

        public async Task<RefreshToken> ObtenerTokenRefrescoAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task ActualizarTokenRefrescoAsync(RefreshToken tokenRefresco)
        {
            _context.RefreshTokens.Update(tokenRefresco);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario> ObtenerPorIdAsync(int id)
        {
            return await _context.Set<Usuario>().FindAsync(id);
        }
    }
}