namespace Application.Archivos.Helpers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;

    public static class UrlImgCompleta
    {
        private static IHttpContextAccessor _httpContextAccessor;
        private static string _subRuta;

        public static void Configure(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            // Leer la carpeta de uploads del JSON
            _subRuta = configuration["BaseImagenes"] ?? "/Storege";
            // Asegurarnos que empieza con "/"
            if (!_subRuta.StartsWith("/")) _subRuta = "/" + _subRuta;
        }

        public static string? GetImagenUrlCompleta(string? imagenUrl)
        {
            if (string.IsNullOrEmpty(imagenUrl) || _httpContextAccessor?.HttpContext == null)
                return null;

            var request = _httpContextAccessor.HttpContext.Request;

            // Normalizar para que no se dupliquen los slashes ni carpetas
            var cleanImagenUrl = imagenUrl.Replace("\\", "/").TrimStart('/');

            return $"{request.Scheme}://{request.Host}{_subRuta}/{cleanImagenUrl}";
        }
    }
}