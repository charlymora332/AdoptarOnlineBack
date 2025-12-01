using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IImagenService
    {
        Task<string> GuardarArchivoAsync(IFormFile archivo, string? subcarpeta = null);
    }
}