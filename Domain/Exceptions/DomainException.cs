using Domain.Entities.Mascotas;

namespace AdoptionApp.Domain.Exceptions
{
    /// <summary>
    /// Contrato del repositorio para manejar operaciones de Mascota.
    /// Define qué se puede hacer, sin importar cómo se implemente.
    /// </summary>
    public interface IMascotaRepositoryyyyyy
    {
        Task<Mascota?> GetByIdAsync(int id);

        Task<IEnumerable<Mascota>> GetAllAsync();

        Task AddAsync(Mascota mascota);

        Task UpdateAsync(Mascota mascota);

        Task DeleteAsync(int id);
    }
}