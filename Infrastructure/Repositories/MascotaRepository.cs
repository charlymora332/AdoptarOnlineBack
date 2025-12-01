using Application.DTOs.Mascota;
using Application.Interfaces;
using Domain.Entities.Mascotas;
using Microsoft.EntityFrameworkCore;

namespace AdopcionOnline.Infrastructure.Repositories
{
    public class MascotaRepository : IMascotaRepository
    {
        private readonly AppDbContext _context;

        public MascotaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MascotaPreviewDto>> GetMascotasUserAsync(
byte? generoId,
List<byte>? categoriasEdadId,
List<byte>? tiposMascotaId,
List<byte>? personalidadesMascotasId,
List<byte>? tamaniosMascotaId,
int? pagina,
int? cantidadConsulta)
        {
            var query = _context.Mascotas
                .Include(m => m.CategoriaTipoAnimal)
                .Include(m => m.CategoriaEdad)
                .Include(m => m.MascotaPersonalidades)
                    .ThenInclude(mp => mp.CategoriaPersonalidad)
                .Where(m => m.Disponible && m.Aprobado)
                .AsQueryable();

            if (generoId.HasValue)
                query = query.Where(m => m.CategoriaGeneroId == generoId.Value);

            if (categoriasEdadId != null && categoriasEdadId.Any())
                query = query.Where(m => categoriasEdadId.Contains(m.CategoriaEdadId));

            if (tiposMascotaId != null && tiposMascotaId.Any())
                query = query.Where(m => tiposMascotaId.Contains(m.TipoAnimalId));

            if (personalidadesMascotasId != null && personalidadesMascotasId.Any())
                query = query.Where(m => m.MascotaPersonalidades
                    .Any(p => personalidadesMascotasId.Contains(p.CategoriaPersonalidadId)));

            if (tamaniosMascotaId != null && tamaniosMascotaId.Any())
                query = query.Where(m => tamaniosMascotaId.Contains(m.TamanioId));

            if (pagina.HasValue && cantidadConsulta.HasValue)
                query = query
                    .Skip((pagina.Value - 1) * cantidadConsulta.Value)
                    .Take(cantidadConsulta.Value);

            var mascotas = await query
                .Select(m => new MascotaPreviewDto
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    Genero = m.CategoriaGeneroId,
                    EdadAnios = (byte)(m.EdadMeses / 12),
                    EdadMeses = (byte)(m.EdadMeses % 12),
                    CategoriaEdad = m.CategoriaEdad.Nombre,
                    Aprobado = m.Aprobado,
                    Disponible = m.Disponible,
                    Tamanio = m.TamanioId,
                    TipoAnimal = m.TipoAnimalId,

                    //TipoAnimalString = m.CategoriaTipoAnimal != null
                    //    ? m.CategoriaTipoAnimal.Nombre
                    //    : "Desconocido",

                    PersonalidadesId = m.MascotaPersonalidades
                        .Select(p => p.CategoriaPersonalidad.Id)
                        .ToList(),

                    Personalidades = m.MascotaPersonalidades
                        .Select(p => p.CategoriaPersonalidad.Nombre)
                        .ToList(),

                    DescripcionCorta = m.DescripcionCorta,
                    ImagenUrl = m.ImagenUrl
                })
                .ToListAsync();

            return mascotas;
        }

        /// Método para consultar la información detallada de una mascota por su ID
        public async Task<MascotaInfoDto?> GetMascotaDetallesUserAsync(int id)
        {
            return await _context.Mascotas
                .Where(m => m.Id == id && m.Disponible && m.Aprobado)
                .Select(m => new MascotaInfoDto
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    EdadMeses = m.EdadMeses,
                    //EdadAnios = m.EdadAnios,
                    DescripcionLarga = m.DescripcionLarga,
                    ImagenUrl = m.ImagenUrl,
                    //Personalidades = m.PersonalidadesId
                    //    .Select(p => p.CategoriaPersonalidad.Nombre)
                    //    .ToList()
                    Personalidades = m.MascotaPersonalidades
                        .Select(p => p.CategoriaPersonalidad.Nombre)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        //public async Task<Mascota?> GetByIdAsync(int id)
        //{
        //    return await _context.Mascotas
        //        .Include(m => m.CategoriaEdad)
        //        .Include(m => m.CategoriaTipoAnimal)
        //        .Include(m => m.MascotaPersonalidades)
        //            .ThenInclude(mp => mp.CategoriaPersonalidad)
        //        .FirstOrDefaultAsync(m => m.Id == id); // ✅ sin incluir ImagenUrl
        //}

        //    public async Task<List<MascotaPreviewDto>> GetAllAsync()
        //    {
        //        var mascotas = await _context.Mascotas
        //          //.Where(m => m.Disponible != true && m.Aprobado != true)

        //          .Select(m => new MascotaPreviewDto
        //          {
        //              Id = m.Id,
        //              Nombre = m.Nombre,
        //              EdadMeses = m.EdadMeses,
        //              // Solo la primera personalidad
        //              //PersonalidadPrincipal = m.PersonalidadesId
        //              //    .Select(p => p.CategoriaPersonalidad.Nombre)
        //              //    .FirstOrDefault() ?? string.Empty,
        //              Personalidades = m.MascotaPersonalidades
        //.Select(p => p.CategoriaPersonalidad.Nombre)
        //.ToList(),
        //              DescripcionCorta = m.DescripcionCorta,
        //              ImagenUrl = m.ImagenUrl,
        //              Aprobado = m.Aprobado,
        //              Disponible = m.Disponible
        //          })
        //          .ToListAsync();
        //        return mascotas;
        //    }

        public async Task AddAsync(Mascota mascota)
        {
            if (mascota == null)
                throw new ArgumentNullException(nameof(mascota), "La mascota no puede ser null.");

            try
            {
                await _context.Mascotas.AddAsync(mascota);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Solo capturamos errores de BD que realmente pueden pasar
                throw new ApplicationException("Error al guardar la mascota en la base de datos.", dbEx);
            }
        }

        public async Task UpdateAsync(Mascota mascota)
        {
            _context.Mascotas.Update(mascota);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota is not null)
            {
                _context.Mascotas.Remove(mascota);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveRelacionPersonalidad(int mascotaId, int personalidadId)
        {
            var relacion = await _context.MascotaPersonalidades
                .FirstOrDefaultAsync(mp =>
                    mp.MascotaId == mascotaId &&
                    mp.CategoriaPersonalidadId == personalidadId);

            if (relacion is not null)
            {
                _context.MascotaPersonalidades.Remove(relacion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<MascotaPreviewAdminDto>> GetMascotasAdminAsync()
        {
            return await _context.Mascotas
               .Select(m => new MascotaPreviewAdminDto
               {
                   Id = m.Id,
                   Nombre = m.Nombre,
                   EdadMeses = m.EdadMeses,
                   TipoAnimal = m.TipoAnimalId,
                   DescripcionCorta = m.DescripcionCorta,
                   ImagenUrl = m.ImagenUrl,
                   Aprobado = m.Aprobado,
                   Disponible = m.Disponible
               })
                .ToListAsync();
        }

        /// Método para consultar la información detallada de una mascota por su ID
        public async Task<MascotaDetallesAdminDto?> GetMascotaAdminByIdAsync(int id)
        {
            return await _context.Mascotas
                .Where(m => m.Id == id)
                .Select(m => new MascotaDetallesAdminDto
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    EdadAnios = (byte)(m.EdadMeses / 12),
                    EdadMeses = (byte)(m.EdadMeses % 12),
                    Aprobado = m.Aprobado,
                    Disponible = m.Disponible,
                    Genero = m.CategoriaGenero.Nombre,

                    DescripcionLarga = m.DescripcionLarga,
                    DescripcionCorta = m.DescripcionCorta,
                    ImagenUrl = m.ImagenUrl,
                    //Personalidades = m.PersonalidadesId
                    //    .Select(p => p.CategoriaPersonalidad.Nombre)
                    //    .ToList()
                    Personalidades = m.MascotaPersonalidades
                        .Select(p => p.CategoriaPersonalidad.Nombre)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task ActualizarAsync(int id, bool aprobado, bool disponible)
        {
            //_context.Mascotas.Update(mascota);
            //await _context.SaveChangesAsync();

            var mascota = new Mascota { Id = id };

            _context.Mascotas.Attach(mascota);

            mascota.Aprobado = aprobado;
            mascota.Disponible = disponible;

            _context.Entry(mascota).Property(m => m.Aprobado).IsModified = true;
            _context.Entry(mascota).Property(m => m.Disponible).IsModified = true;

            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota is not null)
            {
                _context.Mascotas.Remove(mascota);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Mascota?> GetByIdAsync(int id)
        {
            return await _context.Mascotas
                .Include(m => m.CategoriaEdad)
                //.Include(m => m.CategoriaTipoAnimal)
                //.Include(m => m.MascotaPersonalidades)
                //.ThenInclude(mp => mp.CategoriaPersonalidad)
                .FirstOrDefaultAsync(m => m.Id == id); // ✅ sin incluir ImagenUrl
        }

        //public async Task RemoveRelacionPersonalidad(int mascotaId, int personalidadId)
        //{
        //    var relacion = await _context.MascotaPersonalidades
        //        .FirstOrDefaultAsync(mp =>
        //            mp.MascotaId == mascotaId &&
        //            mp.CategoriaPersonalidadId == personalidadId);

        //    if (relacion is not null)
        //    {
        //        _context.MascotaPersonalidades.Remove(relacion);
        //        await _context.SaveChangesAsync();
        //    }
        //}
    }
}