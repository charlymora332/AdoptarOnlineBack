using AdopcionAPI.Controllers.Mascotas.Models;
using Application.IA.Intefaces;
using Application.Mascotas.Intefaces;
using Application.Mascotas.Models.Requests;

//using Application.Mascotas.Models.Requests;
//using Application.Mascotas.Models.Responses;

using Application.Mascotas.Services;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdopcionAPI.Controllers.Mascotas
{
    [ApiController]
    [Route("api/[controller]")]
    public class MascotasController : ControllerBase
    {
        private readonly IMascotaService _mascotaService;
        private readonly IIaDescripcionService _iaDescripcionService;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public MascotasController(IMascotaService mascotaService, IIaDescripcionService iaDescripcionService, IMapper mapper, IMediator mediator)
        {
            _mascotaService = mascotaService;
            _iaDescripcionService = iaDescripcionService;
            _mapper = mapper;
            _mediator = mediator;
        }

        // GET: api/mascotas/test
        [HttpGet]
        public async Task<IActionResult> Test([FromQuery] FiltrosRequestDTO filtros)
        {
            var mascota = _mapper.Map<FiltrosRequestDTO>(filtros);
            var mascotas = await _mascotaService.ObtenerTodasUserAsync(mascota);
            if (mascotas == null) return NotFound();
            return Ok(mascotas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetInfoUser(int id)
        {
            var mascota = await _mascotaService.ObtenerPorIdUserAsync(id);
            if (mascota == null) return NotFound();
            return Ok(mascota);
        }

        [HttpGet("Admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerPreviewAdminMascotas()
        {
            var mascotas = await _mascotaService.ObtenerTodasAdminAsync();
            return Ok(mascotas);
        }

        [HttpGet("Admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetInfoAdmin(int id)
        {
            var mascota = await _mascotaService.ObtenerPorIdAdminAsync(id);
            if (mascota == null) return NotFound();
            return Ok(mascota);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(
    [FromForm] Models.MascotaDto data)
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

                var mascota = _mapper.Map<Application.Mascotas.Models.Requests.MascotaRequestDto>(data);

                var created = await _mascotaService.CrearAsync(mascota);

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
        [HttpPut("Admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, bool aprobado, bool disponible)
        {
            //if (id != mascotaDto.Id) return BadRequest();
            await _mascotaService.ActualizarAsync(id, aprobado, disponible);
            return Ok(new { message = "Mascota actualizada correctamente" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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