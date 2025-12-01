namespace Domain.Common.Utils
{
    /// <summary>
    /// Define las categorías de edad de una mascota y su lógica de asignación.
    /// </summary>
    public static class CategoriaEdadHelper
    {
        // Enum con las categorías posibles (opcional, útil para mantener legibilidad interna)
        private enum CategoriaEdadEnum : byte
        {
            Cachorro = 1,
            Joven = 2,
            Adulto = 3,
            //Senior = 4
        }

        // Diccionario con los rangos en meses
        private static readonly Dictionary<CategoriaEdadEnum, (byte Min, byte Max)> RangosEdad =
            new()
            {
                { CategoriaEdadEnum.Cachorro, (0, 11) },
                { CategoriaEdadEnum.Joven, (12, 35) },
                { CategoriaEdadEnum.Adulto, (35, byte.MaxValue) }
                //{ CategoriaEdadEnum.Senior, (84, byte.MaxValue) }
            };

        /// <summary>
        /// Devuelve el ID de la categoría de edad correspondiente según los meses de edad.
        /// </summary>
        public static byte ObtenerCategoriaPorEdad(byte edad)
        {
            foreach (var rango in RangosEdad)
            {
                if (edad >= rango.Value.Min && edad <= rango.Value.Max)
                    return (byte)rango.Key;
            }

            return (byte)CategoriaEdadEnum.Adulto; // valor por defecto
        }
    }
}