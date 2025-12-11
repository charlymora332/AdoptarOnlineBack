using Microsoft.AspNetCore.Http;

namespace Application.Archivos.Intefaces
{
    public interface IImagenService
    {
        Task<string> GuardarArchivoAsync(IFormFile archivo, string? subcarpeta = null);
    }
}