using Application.DTOs.Mascota;
using Application.Helpers;
using Application.Interfaces;
using Domain.Common.Enum;
using Domain.Common.Utils;
using Domain.Entities.Mascotas;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Mascotas
{
    /// <summary>
    /// Implementación de la lógica de aplicación para manejar mascotas.
    /// Usa el repositorio del dominio como dependencia.
    /// </summary>
    public class MascotaService : IMascotaService
    {
        private readonly IMascotaRepository _mascotaRepository;
        private readonly IIaDescripcionService _iaDescripcionService; // ✅ usa la interfaz
        private readonly IImagenService _imagenService; // ✅ usa la interfaz
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MascotaService(IMascotaRepository mascotaRepository, IIaDescripcionService iaDescripcion, IImagenService imagenService, IHttpContextAccessor httpContextAccessor)
        {
            _mascotaRepository = mascotaRepository;
            _iaDescripcionService = iaDescripcion;
            _imagenService = imagenService;
            _httpContextAccessor = httpContextAccessor;
        }

        //Generico

        public async Task<MascotaRequestDto?> ObtenerPorIdAsync(int id)
        {
            var mascota = await _mascotaRepository.GetByIdAsync(id);
            return mascota == null ? null : MapToDto(mascota);
        }

        //public async Task

        // Método para obtener la información detallada de una mascota
        public async Task<MascotaInfoDto?> ObtenerMascotaMasInfoAsync(int id)
        {
            var consulta = await _mascotaRepository.GetMascotaDetallesUserAsync(id);
            consulta.ImagenUrl = UrlImgCompleta.GetImagenUrlCompleta(consulta.ImagenUrl);

            if (consulta == null) return null;

            return consulta;

            //return new MascotaInfoDto
            //{
            //    Nombre = mascota.Nombre,
            //    EdadAnios = (byte)(mascota.EdadMeses / 12),
            //    EdadMeses = (byte)(mascota.EdadMeses % 12),

            //    Personalidades = mascota.Personalidades,
            //    //.Select(p => p.CategoriaPersonalidad.Nombre)
            //    //.ToList(),
            //    DescripcionLarga = mascota.DescripcionLarga,
            //    ImagenUrl = mascota.ImagenUrl
            //};
        }

        //Metodo para obtener todas las mascotas en formato preview para mostrar en el front(TiendaAdoptar)
        public async Task<IEnumerable<MascotaPreviewDto>> ObtenerPreviewTodasMascotasAsync(
    byte? generoId,
    List<byte>? categoriasEdadId,
    List<byte>? tiposMascotaId,
    List<byte>? personalidadesMascotasId,
    List<byte>? tamaniosMascotaId,
    int? pagina,
    int? cantidadConsulta)
        {
            try
            {
                var consulta = await _mascotaRepository.GetMascotasUserAsync(
                    generoId,
                    categoriasEdadId,
                    tiposMascotaId,
                    personalidadesMascotasId,
                    tamaniosMascotaId,
                    pagina,
                    cantidadConsulta
                );
                // Solo reemplazas ImagenUrl por la completa
                foreach (var m in consulta)
                {
                    if (!string.IsNullOrEmpty(m.ImagenUrl))
                    {
                        m.ImagenUrl = UrlImgCompleta.GetImagenUrlCompleta(m.ImagenUrl);
                    }
                }

                return consulta;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error al intentar obtener las mascotas", ex);
            }
        }

        //public async Task<IEnumerable<MascotaPreviewDto>> ObtenerPreviewTodasMascotasAsync()
        //{
        //    try
        //    {
        //        var mascotas = await _mascotaRepository.GetMascotasUserAsync();
        //        //return mascotas.Select(MapToDto);
        //        return mascotas.Select(m => new MascotaPreviewDto
        //        {
        //            Id = m.Id,
        //            Nombre = m.Nombre,
        //            EdadAnios = (byte)(m.EdadMeses / 12),
        //            EdadMeses = (byte)(m.EdadMeses % 12),

        //            Personalidades = m.Personalidades,
        //            DescripcionCorta = m.DescripcionCorta,
        //            TipoAnimalString = m.TipoAnimalString,
        //            TipoAnimal = m.TipoAnimal,
        //            Tamanio = m.Tamanio,
        //            ImagenUrl = m.ImagenUrl,
        //            Aprobado = m.Aprobado,
        //            Disponible = m.Disponible
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("Ocurrió un error al intentar obtener las mascotas", ex);
        //    }
        //}

        // Método para crear una nueva mascota con validaciones
        public async Task<int> CrearAsync(MascotaRequestDto mascotaDto)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(mascotaDto.Nombre))
                    throw new ArgumentException("El nombre no puede estar vacío.");

                if (mascotaDto.EdadMeses > 240) // máximo 20 años
                    throw new ArgumentException("Edad inválida.");
                if (mascotaDto.PersonalidadesId == null || !mascotaDto.PersonalidadesId.Any())
                    throw new ArgumentException("Debe seleccionar al menos una personalidad.");
                if (string.IsNullOrWhiteSpace(mascotaDto.Genero))
                    throw new ArgumentException("El género no puede estar vacío.");
                if (string.IsNullOrWhiteSpace(mascotaDto.TipoAnimalId))
                    throw new ArgumentException("El tipo animal no puede estar vacío.");
                if (string.IsNullOrWhiteSpace(mascotaDto.Descripcion))
                    throw new ArgumentException("La descripción no puede estar vacía.");

                if (!Enum.TryParse<GeneroE>(mascotaDto.Genero, true, out var generoEnum))
                    throw new ArgumentException("Género inválido.");
                if (!Enum.TryParse<TipoAnimalE>(mascotaDto.TipoAnimalId, true, out var AnimalIdEnum))
                    throw new ArgumentException("Género inválido.");

                byte genero = (byte)generoEnum;
                byte animalId = (byte)AnimalIdEnum;

                var ImagenUrl = await _imagenService.GuardarArchivoAsync(mascotaDto.Imagen, "mascotas");

                // Mapeo a entidad
                var mascota = MapToEntity(mascotaDto, genero, animalId, ImagenUrl);

                // Asignar categoría según la edad
                mascota.CategoriaEdadId = CategoriaEdadHelper.ObtenerCategoriaPorEdad(mascotaDto.EdadMeses);

                // Generar descripciones automáticas con IA
                (mascota.DescripcionCorta, mascota.DescripcionLarga) =
                    await _iaDescripcionService.GenerarDescripcionesAsync(mascotaDto.Descripcion);

                //var (corta, larga) = await _iaDescripcionService.GenerarDescripcionesAsync("Perro golden amigable, juguetón y vacunado.");

                Console.WriteLine(mascota.DescripcionCorta);
                Console.WriteLine(mascota.DescripcionLarga);

                // Guardar en repositorio
                await _mascotaRepository.AddAsync(mascota);

                return mascota.Id;
            }
            catch (ArgumentException argEx)
            {
                throw new InvalidOperationException($"Error de validación: {argEx.Message}", argEx);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error al crear la mascota.", ex);
            }
        }

        //Generico

        //public async Task<List<MascotaPreviewDto>> ObtenerTodasAsync()
        //{
        //    var mascotas = await _mascotaRepository.GetMascotasUserAsync();
        //    return mascotas;
        //}

        public async Task ActualizarAsync(int id, bool aprobado, bool disponible)
        {
            var mascota = await _mascotaRepository.GetByIdAsync(id);
            if (mascota == null)
            {
                throw new NotFoundException("Mascota no encontrada");
            }

            mascota.Aprobado = aprobado;
            mascota.Disponible = disponible;

            //await _mascotaRepository.UpdateAsync(mascota);
        }

        //Generico

        private async Task ActualizarPersonalidadesAsync(Mascota mascota, List<byte> personalidadIds)
        {
            var personalidadesExistentes = mascota.MascotaPersonalidades?
                .Select(p => p.CategoriaPersonalidadId).ToList() ?? new List<byte>();

            // Eliminar relaciones antiguas
            var relacionesAEliminar = mascota.MascotaPersonalidades?
                .Where(p => !personalidadIds.Contains(p.CategoriaPersonalidadId))
                .ToList();

            if (relacionesAEliminar != null && relacionesAEliminar.Any())
            {
                foreach (var relacion in relacionesAEliminar)
                {
                    //await _mascotaRepository.RemoveRelacionPersonalidad(mascota.Id, relacion.CategoriaPersonalidadId);
                }
            }

            // Agregar relaciones nuevas
            foreach (var personalidadId in personalidadIds)
            {
                if (!personalidadesExistentes.Contains(personalidadId))
                {
                    mascota.MascotaPersonalidades.Add(new MascotaPersonalidad { CategoriaPersonalidadId = personalidadId });
                }
            }
        }

        //Generico

        public async Task EliminarAsync(int id)
        {
            //await _mascotaRepository.DeleteAsync(id);
        }

        //mapeos
        private string GetImagenUrlCompleta(string rutaRelativa)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (string.IsNullOrEmpty(rutaRelativa) || request == null)
                return null;

            return $"{request.Scheme}://{request.Host}{rutaRelativa}";
        }

        private MascotaRequestDto MapToDto(Mascota mascota)
        {
            string tipoAnimalString = null;

            // 👇 Validamos o convertimos el enum ANTES de crear el objeto
            if (Enum.TryParse<TipoAnimalE>(mascota.TipoAnimalId.ToString(), out var tipo))
            {
                tipoAnimalString = tipo.ToString();
            }

            // Aquí construyes la URL completa si quieres
            //var imagenUrlCompleta = !string.IsNullOrEmpty(mascota.ImagenUrl)
            //    ? $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{mascota.ImagenUrl}"
            //    : null;

            var imagenUrlCompleta = _httpContextAccessor.HttpContext != null
              ? $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{mascota.ImagenUrl}"
              : mascota.ImagenUrl;

            // ✅ Luego inicializamos el DTO
            return new MascotaRequestDto
            {
                Nombre = mascota.Nombre,
                EdadMeses = mascota.EdadMeses,
                TamanioId = mascota.TamanioId,
                TipoAnimalId = tipoAnimalString,
                PersonalidadesId = mascota.MascotaPersonalidades?
                    .Select(p => p.CategoriaPersonalidadId)
                    .ToList() ?? new List<byte>(),
                ImagenUrl = imagenUrlCompleta,
                CorreoPublicador = mascota.Usuario.Correo,
                Ciudad = mascota.Ciudad
            };
        }

        private Mascota MapToEntity(MascotaRequestDto dto, byte genero, byte animalId, string ImagenUrl) => new Mascota
        {
            //Id = dto.Id,
            Nombre = dto.Nombre,
            TamanioId = dto.TamanioId,
            EdadMeses = dto.EdadMeses,
            CategoriaGeneroId = genero,
            TipoAnimalId = animalId,
            MascotaPersonalidades = dto.PersonalidadesId?
                .Select(id => new MascotaPersonalidad { CategoriaPersonalidadId = id })
                .ToList() ?? new List<MascotaPersonalidad>(),
            ImagenUrl = ImagenUrl,
            Aprobado = true,
            Disponible = true,
            UsuarioId = 1, // Temporal hasta tener autenticación
            //CorreoPublicador = dto.CorreoPublicador,
            Ciudad = dto.Ciudad
        };
    }
}