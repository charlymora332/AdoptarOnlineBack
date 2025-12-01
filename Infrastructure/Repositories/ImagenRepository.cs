using Application.Interfaces;
using Domain.Entities;

// es un ejemplo por que esto nunca va a memoria

namespace Infrastructure.Repositories

{
    public class ImagenRepository : IImagenRepository
    {
        public Task GuardarAsync(Imagen imagen)
        {
            // 🚨 No hace nada, solo cumple con la interfaz
            return Task.CompletedTask;
        }
    }
}