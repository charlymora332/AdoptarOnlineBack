using Domain.Entities;

namespace Application.Interfaces
{
    public interface IImagenRepository
    {
        Task GuardarAsync(Imagen imagen);
    }
}