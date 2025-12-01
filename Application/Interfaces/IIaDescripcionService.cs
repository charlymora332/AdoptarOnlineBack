namespace Application.Interfaces
{
    public interface IIaDescripcionService
    {
        Task<(string DescripcionCorta, string DescripcionLarga)> GenerarDescripcionesAsync(string descripcionBase);
    }
}