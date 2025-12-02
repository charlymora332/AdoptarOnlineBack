using Application.DTOs.Mascota;
using Application.Interfaces;
using Application.Services.Mascotas;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MascotasController : ControllerBase
    {
        private readonly IMascotaService _mascotaService;
        private readonly IIaDescripcionService _iaDescripcionService;

        public MascotasController(IMascotaService mascotaService, IIaDescripcionService iaDescripcionService)
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
            var mascotas = await _mascotaService.ObtenerTodasUserAsync(genero, categoriaEdad, tipoMascota, personalidadesMascotasId, tamanioMascota, pagina, cantidadConsulta);

            if (mascotas == null) return NotFound();
            return Ok(mascotas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MascotaInfoDto>> GetInfo(int id)
        {
            var mascota = await _mascotaService.ObtenerPorIdUserAsync(id);

            if (mascota == null) return NotFound();

            return Ok(mascota);
        }

        [HttpGet("Admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerPreviewMascotas()
        {
            var mascotas = await _mascotaService.ObtenerTodasAdminAsync();

            return Ok(mascotas);
        }

        [HttpGet("Admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MascotaInfoDto>> GetInfoAdmin(int id)
        {
            var mascota = await _mascotaService.ObtenerPorIdAdminAsync(id);

            if (mascota == null) return NotFound();

            return Ok(mascota);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(
    [FromForm] MascotaRequestDto mascotaDto)
        {
            try
            {
                // Código del POST

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
            catch (Exception ex)
            {
                Console.WriteLine(ex); // o usar logger
                throw; // opcional si quieres que el error suba
            }
        }

        // PUT: api/mascotas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, bool aprobado, bool disponible)
        {
            //if (id != mascotaDto.Id) return BadRequest();
            await _mascotaService.ActualizarAsync(id, aprobado, disponible);
            return Ok(new { message = "Mascota actualizada correctamente" });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _mascotaService.EliminarAsync(id);
            return NoContent();
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