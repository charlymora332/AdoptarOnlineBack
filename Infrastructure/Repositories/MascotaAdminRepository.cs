//PARA ELIMINAR AHORA MISMO SI FUNCIONA Y COMPLILA EL CODIGO

//using Application.DTOs.Mascota;
//using Application.Interfaces;
//using Domain.Entities.Mascotas;
//using Microsoft.EntityFrameworkCore;

//namespace AdopcionOnline.Infrastructure.Repositories
//{
//    public class MascotaAdminRepository : IMascotaAdminRepository
//    {
//        private readonly AppDbContext _context;

//        public MascotaAdminRepository(AppDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<List<MascotaPreviewAdminDto>> ObtenerTodasAsync()
//        {
//            return await _context.Mascotas
//               .Select(m => new MascotaPreviewAdminDto
//               {
//                   Id = m.Id,
//                   Nombre = m.Nombre,
//                   EdadMeses = m.EdadMeses,
//                   TipoAnimal = m.TipoAnimalId,
//                   DescripcionCorta = m.DescripcionCorta,
//                   ImagenUrl = m.ImagenUrl,
//                   Aprobado = m.Aprobado,
//                   Disponible = m.Disponible
//               })
//                .ToListAsync();
//        }

//        /// Método para consultar la información detallada de una mascota por su ID
//        public async Task<MascotaDetallesAdminDto?> ObtenerInfoMascotaAsync(int id)
//        {
//            return await _context.Mascotas
//                .Where(m => m.Id == id)
//                .Select(m => new MascotaDetallesAdminDto
//                {
//                    Id = m.Id,
//                    Nombre = m.Nombre,
//                    EdadAnios = (byte)(m.EdadMeses / 12),
//                    EdadMeses = (byte)(m.EdadMeses % 12),
//                    Aprobado = m.Aprobado,
//                    Disponible = m.Disponible,
//                    Genero = m.CategoriaGenero.Nombre,

//                    DescripcionLarga = m.DescripcionLarga,
//                    DescripcionCorta = m.DescripcionCorta,
//                    ImagenUrl = m.ImagenUrl,
//                    //Personalidades = m.PersonalidadesId
//                    //    .Select(p => p.CategoriaPersonalidad.Nombre)
//                    //    .ToList()
//                    Personalidades = m.MascotaPersonalidades
//                        .Select(p => p.CategoriaPersonalidad.Nombre)
//                        .ToList()
//                })
//                .FirstOrDefaultAsync();
//        }

//        public async Task ActualizarAsync(int id, bool aprobado, bool disponible)
//        {
//            //_context.Mascotas.Update(mascota);
//            //await _context.SaveChangesAsync();

//            var mascota = new Mascota { Id = id };

//            _context.Mascotas.Attach(mascota);

//            mascota.Aprobado = aprobado;
//            mascota.Disponible = disponible;

//            _context.Entry(mascota).Property(m => m.Aprobado).IsModified = true;
//            _context.Entry(mascota).Property(m => m.Disponible).IsModified = true;

//            await _context.SaveChangesAsync();
//        }

//        public async Task EliminarAsync(int id)
//        {
//            var mascota = await _context.Mascotas.FindAsync(id);
//            if (mascota is not null)
//            {
//                _context.Mascotas.Remove(mascota);
//                await _context.SaveChangesAsync();
//            }
//        }

//        public async Task<Mascota?> GetByIdAsync(int id)
//        {
//            return await _context.Mascotas
//                .Include(m => m.CategoriaEdad)
//                //.Include(m => m.CategoriaTipoAnimal)
//                //.Include(m => m.MascotaPersonalidades)
//                //.ThenInclude(mp => mp.CategoriaPersonalidad)
//                .FirstOrDefaultAsync(m => m.Id == id); // ✅ sin incluir ImagenUrl
//        }

//        public async Task RemoveRelacionPersonalidad(int mascotaId, int personalidadId)
//        {
//            var relacion = await _context.MascotaPersonalidades
//                .FirstOrDefaultAsync(mp =>
//                    mp.MascotaId == mascotaId &&
//                    mp.CategoriaPersonalidadId == personalidadId);

//            if (relacion is not null)
//            {
//                _context.MascotaPersonalidades.Remove(relacion);
//                await _context.SaveChangesAsync();
//            }
//        }
//    }
//}