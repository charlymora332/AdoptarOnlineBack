using Application.DTOs.Mascota;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MascotasAdminController : ControllerBase
    {
        private readonly IMascotaAdminService _mascotaAdminService;

        public MascotasAdminController(IMascotaAdminService mascotaAdminService)
        {
            _mascotaAdminService = mascotaAdminService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPreviewMascotas()
        {
            var mascotas = await _mascotaAdminService.ObtenerTodasAsync();

            return Ok(mascotas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MascotaInfoDto>> GetInfo(int id)
        {
            var mascota = await _mascotaAdminService.ObtenerPorIdAsync(id);

            if (mascota == null) return NotFound();

            return Ok(mascota);
        }

        // PUT: api/mascotas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, bool aprobado, bool disponible)
        {
            //if (id != mascotaDto.Id) return BadRequest();
            await _mascotaAdminService.ActualizarAsync(id, aprobado, disponible);
            return Ok(new { message = "Mascota actualizada correctamente" });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _mascotaAdminService.EliminarAsync(id);
            return NoContent();
        }
    }
}