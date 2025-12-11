using Application.Auth.DTOs;
using Application.Auth.Intefaces;
using Application.Auth.Services;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdopcionAPI.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private const string JwtCookieNombre = "jwt";
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
            Response.Cookies.Append(JwtCookieNombre, usuario.Token, new CookieOptions
            {
                HttpOnly = true,           // No accesible desde JS
                Secure = true,             // Solo HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            });

            return Ok(new
            {
                usuario.Id,
                usuario.Nombre,
                usuario.Correo,
                usuario.Rol,
                //usuario.Token
            });
        }

        [HttpGet("status")]
        public IActionResult GetUserStatus()
        {
            string token = Request.Cookies[JwtCookieNombre];

            if (string.IsNullOrEmpty(token))
            {
                // No hay token en la cookie
                return Unauthorized(); // 401
            }

            try
            {
                // Llama al servicio, que lanzará una excepción si falla.
                var status = _authService.ValidateTokenAndGetUserStatus(token);

                // Si llegamos aquí, el token es válido.
                return Ok(status); // 200 OK
            }
            catch (SecurityTokenExpiredException)
            {
                // Token expirado: sigue siendo 401 Unauthorized.
                // Opcionalmente, se puede limpiar la cookie.
                Response.Cookies.Delete(JwtCookieNombre);
                return Unauthorized(); // 401
            }
            catch (SecurityTokenValidationException)
            {
                // Firma inválida, token corrupto, claims faltantes: 401 Unauthorized.
                Response.Cookies.Delete(JwtCookieNombre);
                return Unauthorized(); // 401
            }
            catch (Exception ex)
            {
                // Para cualquier otro error inesperado.
                return StatusCode(500, "Error interno del servidor.");
            }
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