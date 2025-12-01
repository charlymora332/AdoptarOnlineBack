using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Application.DTOs.Mascota;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MascotasUsuarioController : ControllerBase
    {
        private readonly IMascotaService _mascotaService;
        private readonly IIaDescripcionService _iaDescripcionService;

        public MascotasUsuarioController(IMascotaService mascotaService, IIaDescripcionService iaDescripcionService)
        {
            _mascotaService = mascotaService;
            _iaDescripcionService = iaDescripcionService;
        }

        // GET: api/mascotas/test
        [HttpGet]
        public async Task<IActionResult> Test([FromQuery] byte? genero,
    [FromQuery] List<byte>? categoriaEdad,
    [FromQuery] List<byte>? tipoMascota,
    [FromQuery] List<byte>? personalidadesMascotasId,
    [FromQuery] List<byte>? tamanioMascota,
    [FromQuery] int? pagina,
    [FromQuery] int? cantidadConsulta)
        {
            var mascotas = await _mascotaService.ObtenerPreviewTodasMascotasAsync(genero, categoriaEdad, tipoMascota, personalidadesMascotasId, tamanioMascota, pagina, cantidadConsulta);

            if (mascotas == null) return NotFound();
            return Ok(mascotas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MascotaInfoDto>> GetInfo(int id)
        {
            var mascota = await _mascotaService.ObtenerMascotaMasInfoAsync(id);

            if (mascota == null) return NotFound();

            return Ok(mascota);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
    [FromForm] MascotaRequestDto mascotaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = "Datos incompletos o con formato incorrecto.",
                    errores = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var created = await _mascotaService.CrearAsync(mascotaDto);

            return Ok(new
            {
                exito = true,
                mensaje = "Mascota registrada correctamente."
            });
        }

        [HttpGet("gpt")]
        public async Task<IActionResult> Ask(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Ingresa una descripción.");

            var resultado = await _iaDescripcionService.GenerarDescripcionesAsync(q);

            return Ok(new
            {
                descripcionCorta = resultado.DescripcionCorta,
                descripcionLarga = resultado.DescripcionLarga
            });
        }
    }
}