using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdopcionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegistrarAsync(dto);
            if (!result) return BadRequest("Correo ya registrado");
            return Ok("Usuario registrado correctamente");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var usuario = await _authService.LoginAsync(dto);
            if (usuario == null)
                return Unauthorized("Correo o contraseña incorrectos");

            // 🔹 Mandar JWT en cookie
            Response.Cookies.Append("jwt", usuario.Token, new CookieOptions
            {
                HttpOnly = true,           // No accesible desde JS
                Secure = true,             // Solo HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            });

            // 🔹 Devolver usuario + token en JSON (para pruebas)
            return Ok(new
            {
                usuario.Id,
                usuario.Nombre,
                usuario.Correo,
                usuario.Rol,
                Token = usuario.Token
            });
        }

        // ---------------------------
        // Refrescar token
        // ---------------------------
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out string token))
                return Unauthorized("No hay refresh token");

            // Validamos token refresco
            var (valido, tokenRefresco) = await _authService.ValidarTokenRefrescoAsync(token);

            if (!valido)
                return Unauthorized("Refresh token inválido o expirado");

            // Obtenemos el usuario mediante el Service
            var usuario = await _authService.ObtenerUsuarioPorIdAsync(tokenRefresco.UsuarioId);

            var nuevoAccessToken = _authService.GenerarJwt(usuario);

            return Ok(new { accessToken = nuevoAccessToken });
        }

        // ---------------------------
        // Logout / cerrar sesión
        // ---------------------------
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue("refreshToken", out string token))
            {
                await _authService.RevocarTokenRefrescoAsync(token);

                // Eliminar cookie del cliente
                Response.Cookies.Delete("refreshToken");
            }

            return Ok("Sesión cerrada correctamente");
        }
    }
}