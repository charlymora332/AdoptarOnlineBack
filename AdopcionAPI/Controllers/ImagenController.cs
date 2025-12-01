using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore;

namespace AdopcionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagenController : ControllerBase
    {
        private readonly IImagenService _imagenService;
        private readonly IImagenRepository _imagenRepository;

        public ImagenController(IImagenService imagenService, IImagenRepository imagenRepository)
        {
            _imagenService = imagenService;
            _imagenRepository = imagenRepository;
        }

        [HttpPost("subir")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Subir(IFormFile archivo)
        {
            try
            {
                if (archivo == null || archivo.Length == 0)
                    return BadRequest("Archivo inválido");

                var url = await _imagenService.GuardarArchivoAsync(archivo);

                var imagen = new Imagen
                {
                    Id = Guid.NewGuid(),
                    Url = url
                };

                await _imagenRepository.GuardarAsync(imagen);

                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}