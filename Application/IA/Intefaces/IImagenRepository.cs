using Domain.Entities;

namespace Application.IA.Intefaces
{
    public interface IImagenRepository
    {
        Task GuardarAsync(Imagen imagen);
    }
}