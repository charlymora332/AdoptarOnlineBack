namespace Application.IA.Intefaces
{
    public interface IIaDescripcionService
    {
        Task<(string DescripcionCorta, string DescripcionLarga)> GenerarDescripcionesAsync(string descripcionBase);
    }
}