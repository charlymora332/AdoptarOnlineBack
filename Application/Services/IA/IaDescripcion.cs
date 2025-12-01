using Application.Interfaces;
using System.Threading.Tasks;

namespace Application.Services.IA
{
    public class IaDescripcionService : IIaDescripcionService
    {
        public async Task<(string DescripcionCorta, string DescripcionLarga)> GenerarDescripcionesAsync(string descripcionBase)
        {
            // Simulación temporal (demo)
            await Task.Delay(500); // simula tiempo de respuesta

            // Por ahora generamos texto genérico para probar el flujo
            string corta = $"Descripción corta generada para: {descripcionBase}";
            string larga = $"Esta es una descripción larga generada automáticamente con base en: {descripcionBase}. Aquí se agregan más detalles y contexto.";

            return (corta, larga);
        }
    }
}


//using Application.Interfaces;
//using System.Net.Http;
//using System.Text;
//using System.Text.Json;

//namespace Application.Services.IA
//{


//    public class IADescripcion : IIADescripcion
//    {
//        private readonly HttpClient _httpClient;

//        public IADescripcion(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        public async Task<(string, string)> GenerarDescripcionesAsync(string descripcionOriginal)
//        {
//            var prompt = $"Genera dos versiones de esta descripción de mascota: una corta (máx 100 caracteres) y una larga, bien redactada:\n\n{descripcionOriginal}";

//            var response = await _httpClient.PostAsync(
//                "https://api.openai.com/v1/chat/completions",
//                new StringContent(JsonSerializer.Serialize(new
//                {
//                    model = "gpt-3.5-turbo",
//                    messages = new[] { new { role = "user", content = prompt } },
//                    temperature = 0.7
//                }), Encoding.UTF8, "application/json")
//            );

//            var json = await response.Content.ReadAsStringAsync();
//            // parseas aquí las 2 versiones según la respuesta del modelo
//            // por simplicidad, devuelvo el texto completo
//            return (descripcionOriginal[..Math.Min(100, descripcionOriginal.Length)], json);
//        }
//    }

//}

