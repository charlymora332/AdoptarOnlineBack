using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Services.IA
{
    public class IaDescripcionService : IIaDescripcionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public IaDescripcionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:ApiKey"];
        }

        public async Task<(string DescripcionCorta, string DescripcionLarga)> GenerarDescripcionesAsync(string descripcionBase)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_apiKey))
                {
                    return ("ERROR: API KEY ausente o vacía.", "Verifica OpenAI:ApiKey en appsettings.json.");
                }

                string url = "https://api.openai.com/v1/chat/completions";

                var payload = new
                {
                    model = "gpt-4.1-mini",
                    messages = new[]
                    {
                        new {
                            role = "system",
                            content = "Eres un generador de descripciones profesionales."
                        },
                        new {
                            role = "user",
                            content =
$@"Genera una descripción corta y una larga en JSON.

Reglas:
- Corta: máximo 20 palabras
- Larga: entre 100 y 150 palabras
- Responde SOLO JSON

Información base:
{descripcionBase}

Formato:
{{
  ""descripcionCorta"": """",
  ""descripcionLarga"": """"
}}"
                        }
                    }
                };

                var requestJson = JsonSerializer.Serialize(payload);
                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiKey);

                HttpResponseMessage response;

                try
                {
                    response = await _httpClient.PostAsync(url, content);
                }
                catch (HttpRequestException ex)
                {
                    return ($"ERROR HTTP al conectar: {ex.Message}", ex.StackTrace);
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return (
                        $"ERROR OpenAI ({response.StatusCode}): {jsonResponse}",
                        "La IA no generó respuesta. Revisa tu API KEY o el modelo usado."
                    );
                }

                // ------------------
                //  PARSEO DE LA IA
                // ------------------
                try
                {
                    using var doc = JsonDocument.Parse(jsonResponse);

                    var message = doc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        return ("ERROR: mensaje vacío en respuesta GPT.", jsonResponse);
                    }

                    var result = JsonSerializer.Deserialize<DescripcionModel>(message);

                    if (result == null)
                    {
                        return ("ERROR parseando JSON interno del modelo.", message);
                    }

                    return (result.DescripcionCorta, result.DescripcionLarga);
                }
                catch (Exception ex)
                {
                    return ($"ERROR parseando la respuesta: {ex.Message}", jsonResponse);
                }
            }
            catch (Exception ex)
            {
                return (
                    $"ERROR inesperado: {ex.Message}",
                    ex.StackTrace ?? "Sin stacktrace"
                );
            }
        }

        private class DescripcionModel
        {
            [JsonPropertyName("descripcionCorta")]
            public string DescripcionCorta { get; set; }

            [JsonPropertyName("descripcionLarga")]
            public string DescripcionLarga { get; set; }
        }
    }
}

//using Application.Interfaces;
//using Microsoft.Extensions.Configuration;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.Json;

//namespace Application.Services.IA
//{
//    public class IaDescripcionService : IIaDescripcionService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _apiKey;

//        public IaDescripcionService(HttpClient httpClient, IConfiguration configuration)
//        {
//            _httpClient = httpClient;
//            _apiKey = configuration["OpenAI:ApiKey"];
//        }

//        public async Task<(string DescripcionCorta, string DescripcionLarga)> GenerarDescripcionesAsync(string descripcionBase)
//        {
//            // Endpoint oficial de OpenAI
//            string url = "https://api.openai.com/v1/chat/completions";

//            // Construcción del prompt
//            var payload = new
//            {
//                model = "gpt-4.1-mini",
//                messages = new[]
//                {
//                    new {
//                        role = "system",
//                        content = "Eres un generador de descripciones profesionales."
//                    },
//                    new {
//                        role = "user",
//                        content =
//$@"Genera una descripción corta y una larga en JSON.

//Reglas:
//- Corta: máximo 20 palabras
//- Larga: entre 100 y 150 palabras
//- Responde SOLO JSON

//Información base:
//{descripcionBase}

//Formato:
//{{
//  ""descripcionCorta"": """",
//  ""descripcionLarga"": """"
//}}"
//                    }
//                }
//            };

//            var requestJson = JsonSerializer.Serialize(payload);
//            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

//            // Encabezados
//            _httpClient.DefaultRequestHeaders.Authorization =
//                new AuthenticationHeaderValue("Bearer", _apiKey);

//            // Llamada a OpenAI
//            var response = await _httpClient.PostAsync(url, content);
//            var jsonResponse = await response.Content.ReadAsStringAsync();

//            if (!response.IsSuccessStatusCode)
//                throw new Exception($"Error en OpenAI: {jsonResponse}");

//            // Parsear respuesta
//            using var doc = JsonDocument.Parse(jsonResponse);
//            var message = doc.RootElement
//                .GetProperty("choices")[0]
//                .GetProperty("message")
//                .GetProperty("content")
//                .GetString();

//            // Convertir el JSON devuelto por GPT
//            var result = JsonSerializer.Deserialize<DescripcionModel>(message);

//            return (result.DescripcionCorta, result.DescripcionLarga);
//        }

//        // Clase interna para mapear JSON devuelto
//        private class DescripcionModel
//        {
//            public string DescripcionCorta { get; set; }
//            public string DescripcionLarga { get; set; }
//        }
//    }
//}