using Application.Archivos.Intefaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

public class ImagenService : IImagenService
{
    private readonly string _uploadRootFolder; // carpeta base

    public ImagenService(IConfiguration config)
    {
        // Ruta base absoluta
        var relativePath = config["UploadFolder"]; // Ej: "wwwroot/Uploads"
        _uploadRootFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            relativePath!
        );
    }

    public async Task<string> GuardarArchivoAsync(IFormFile archivo, string? subcarpeta = null)
    {
        // Validación de extensión
        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(archivo.FileName).ToLower();

        if (!extensionesPermitidas.Contains(extension))
            throw new Exception("Formato de archivo no permitido.");

        // Construimos la ruta final con subcarpetas si las hay
        string carpetaDestino = _uploadRootFolder;

        if (!string.IsNullOrEmpty(subcarpeta))
        {
            carpetaDestino = Path.Combine(_uploadRootFolder, subcarpeta);

            // Crear subcarpeta si no existe
            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);
        }

        // Nombre único
        var nombreArchivo = $"{Guid.NewGuid()}{extension}";

        // Ruta física completa
        var rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

        // Guardar archivo correctamente
        using (var stream = new FileStream(rutaCompleta, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await archivo.CopyToAsync(stream);
        }

        // Construir URL pública
        string url = string.IsNullOrEmpty(subcarpeta)
            ? $"/Uploads/{nombreArchivo}"
            : $"/Uploads/{subcarpeta}/{nombreArchivo}";

        return url;
    }
}

//using Application.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;

//public class ImagenService : IImagenService
//{
//    private readonly string _uploadFolder;

//    public ImagenService(IConfiguration config)
//    {
//        var relativePath = config["UploadFolder"];

//        _uploadFolder = Path.Combine(
//            Directory.GetCurrentDirectory(),
//            relativePath!
//        );
//    }

//    public async Task<string> GuardarArchivoAsync(IFormFile archivo)
//    {
//        // Validación extensión
//        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
//        var extension = Path.GetExtension(archivo.FileName).ToLower();

//        if (!extensionesPermitidas.Contains(extension))
//            throw new Exception("Formato de archivo no permitido.");

//        // Asegura carpeta
//        if (!Directory.Exists(_uploadFolder))
//            Directory.CreateDirectory(_uploadFolder);

//        // Genera nombre único
//        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
//        var rutaCompleta = Path.Combine(_uploadFolder, nombreArchivo);

//        // Guarda el archivo
//        using var stream = new FileStream(rutaCompleta, FileMode.Create);
//        await archivo.CopyToAsync(stream);

//        // URL pública (el middleware la sirve)
//        return $"/Uploads/{nombreArchivo}";
//    }
//}