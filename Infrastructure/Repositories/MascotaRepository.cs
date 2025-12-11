using Application.Mascotas.Intefaces;
using Application.Mascotas.Models.Requests;
using Application.Mascotas.Models.Responses;
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

        //public async Task<List<MascotaPreviewDto>> GetMascotasUserAsync(FiltrosRequestDTO filtros)
        //{
        //    var query = _context.Mascotas
        //        .Include(m => m.CategoriaTipoAnimal)
        //        .Include(m => m.CategoriaEdad)
        //        .Include(m => m.MascotaPersonalidades)
        //            .ThenInclude(mp => mp.CategoriaPersonalidad)
        //        .Where(m => m.Disponible && m.Aprobado)
        //        .AsQueryable();

        //    if (filtros.GeneroId.HasValue)
        //        query = query.Where(m => m.CategoriaGeneroId == filtros.GeneroId.Value);

        //    if (filtros.CategoriaEdadId != null && (filtros.CategoriaEdadId.Any()))
        //        query = query.Where(m => filtros.CategoriaEdadId.Contains(m.CategoriaEdadId));

        //    if (filtros.TipoMascotaId != null && filtros.TipoMascotaId.Any())
        //        query = query.Where(m => filtros.TipoMascotaId.Contains(m.TipoAnimalId));

        //    if (filtros.PersonalidadesMascotasId != null && filtros.PersonalidadesMascotasId.Any())
        //        query = query.Where(m => m.MascotaPersonalidades
        //            .Any(p => filtros.PersonalidadesMascotasId.Contains(p.CategoriaPersonalidadId)));

        //    if (filtros.TamanioMascotaId != null && filtros.TamanioMascotaId.Any())
        //        query = query.Where(m => filtros.TamanioMascotaId.Contains(m.TamanioId));

        //    if (filtros.Pagina.HasValue && filtros.CantidadConsulta.HasValue)
        //        query = query
        //            .Skip((filtros.Pagina.Value - 1) * filtros.CantidadConsulta.Value)
        //            .Take(filtros.CantidadConsulta.Value);

        //    var mascotas = await query
        //        .Select(m => new MascotaPreviewDto
        //        {
        //            Id = m.Id,
        //            Nombre = m.Nombre,
        //            Genero = m.CategoriaGeneroId,
        //            EdadAnios = (byte)(m.EdadMeses / 12),
        //            EdadMeses = (byte)(m.EdadMeses % 12),
        //            CategoriaEdad = m.CategoriaEdad.Nombre,
        //            Aprobado = m.Aprobado,
        //            Disponible = m.Disponible,
        //            Tamanio = m.TamanioId,
        //            TipoAnimal = m.TipoAnimalId,

        //            //TipoAnimalString = m.CategoriaTipoAnimal != null
        //            //    ? m.CategoriaTipoAnimal.Nombre
        //            //    : "Desconocido",

        //            PersonalidadesId = m.MascotaPersonalidades
        //                .Select(p => p.CategoriaPersonalidad.Id)
        //                .ToList(),

        //            Personalidades = m.MascotaPersonalidades
        //                .Select(p => p.CategoriaPersonalidad.Nombre)
        //                .ToList(),

        //            DescripcionCorta = m.DescripcionCorta,
        //            ImagenUrl = m.ImagenUrl
        //        })
        //        .ToListAsync();

        //    return mascotas;
        //}

        public async Task<List<MascotaPreviewDto>> GetMascotasUserAsync(FiltrosRequestDTO filtros)
        {
            // 1. Inicios de Query: SOLO Includes NECESARIOS o AsQueryable
            // Se eliminan includes que solo se proyectan en el DTO (como MascotaPersonalidades)
            var query = _context.Mascotas
                // Dejar Include si el DTO lo usa como propiedad navegacional (ej: m.CategoriaEdad.Nombre)
                .Include(m => m.CategoriaEdad)
                .Where(m => m.Disponible && m.Aprobado)
                .AsQueryable();

            // 2. Filtros Condicionales (Limpieza con ?.Any())
            if (filtros.GeneroId.HasValue)
                query = query.Where(m => m.CategoriaGeneroId == filtros.GeneroId.Value);

            if (filtros.CategoriaEdadId?.Any() == true)
                query = query.Where(m => filtros.CategoriaEdadId.Contains(m.CategoriaEdadId));

            if (filtros.TipoMascotaId?.Any() == true)
                query = query.Where(m => filtros.TipoMascotaId.Contains(m.TipoAnimalId));

            if (filtros.PersonalidadesMascotasId?.Any() == true)
                query = query.Where(m => m.MascotaPersonalidades
                    .Any(p => filtros.PersonalidadesMascotasId.Contains(p.CategoriaPersonalidadId)));

            if (filtros.TamanioMascotaId?.Any() == true)
                query = query.Where(m => filtros.TamanioMascotaId.Contains(m.TamanioId));

            // 3. Paginación
            if (filtros.Pagina.HasValue && filtros.CantidadConsulta.HasValue)
                query = query
                    .Skip((filtros.Pagina.Value - 1) * filtros.CantidadConsulta.Value)
                    .Take(filtros.CantidadConsulta.Value);

            // 4. Proyección (Select) y Ejecución
            var mascotas = await query
                .Select(m => new MascotaPreviewDto
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    // ... otras propiedades
                    CategoriaEdad = m.CategoriaEdad.Nombre, // Usa el include de CategoriaEdad

                    // Proyección de colecciones sin includes previos
                    PersonalidadesId = m.MascotaPersonalidades
                        .Select(p => p.CategoriaPersonalidad.Id)
                        .ToList(),

                    Personalidades = m.MascotaPersonalidades
                        .Select(p => p.CategoriaPersonalidad.Nombre)
                        .ToList(),

                    // ... otras propiedades
                    DescripcionCorta = m.DescripcionCorta,
                    ImagenUrl = m.ImagenUrl
                })
                .ToListAsync();

            return mascotas;
        }

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

        public async Task EliminarAsync(int id)
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

        //public async Task EliminarAsync(int id)
        //{
        //    var mascota = await _context.Mascotas.FindAsync(id);
        //    if (mascota is not null)
        //    {
        //        _context.Mascotas.Remove(mascota);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        public async Task<Mascota?> GetByIdAsync(int id)
        {
            return await _context.Mascotas.AsNoTracking()
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