using Newtonsoft.Json;
using SistemaVotoElectronico.Modelos.Responses;
using System.Text;

namespace SistemaVotoElectronico.ApiConsumer
{
    public static class Crud<T>
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<ApiResult<T>> CreateAsync(string endpoint, T data)
        {
            try
            {
                // 1. Serializar el objeto T a string JSON y configurar Content-Type
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // 2. Ejecutar petición POST
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseJson = await response.Content.ReadAsStringAsync();

                // 3. Deserializar respuesta exitosa o encapsular error
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiResult<T>>(responseJson);
                }
                else
                {
                    return ApiResult<T>.Fail($"Error {response.StatusCode}: {responseJson}");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Fail($"Error de conexión: {ex.Message}");
            }
        }

        public static async Task<ApiResult<List<T>>> ReadAllAsync(string endpoint)
        {
            try
            {
                // 1. Ejecutar petición GET directa a la URL listado
                var response = await _httpClient.GetAsync(endpoint);
                var responseJson = await response.Content.ReadAsStringAsync();

                // 2. Deserializar lista
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiResult<List<T>>>(responseJson);
                }
                else
                {
                    return ApiResult<List<T>>.Fail($"Error {response.StatusCode}: {responseJson}");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<List<T>>.Fail($"Error de conexión: {ex.Message}");
            }
        }

        public static async Task<ApiResult<T>> ReadByAsync(string endpointBase, string field, string value)
        {
            try
            {
                // 1. Construir ruteo lógico: Si es por Id, url directa; caso contrario subruteo de campo
                string url;
                if (string.Equals(field, "Id", StringComparison.OrdinalIgnoreCase))
                {
                    url = $"{endpointBase}/{value}";
                }
                else
                {
                    url = $"{endpointBase}/{field}/{value}";
                }

                // 2. Ejecutar petición GET
                var response = await _httpClient.GetAsync(url);
                var responseJson = await response.Content.ReadAsStringAsync();

                // 3. Retornar resultado
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiResult<T>>(responseJson);
                }
                else
                {
                    return ApiResult<T>.Fail($"Error {response.StatusCode}. No encontrado.");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Fail($"Error de conexión: {ex.Message}");
            }
        }

        public static async Task<ApiResult<bool>> UpdateAsync(string endpoint, string id, T data)
        {
            try
            {
                // 1. Serializar el cuerpo para la actualización
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // 2. Enviar petición PUT adjuntando el id a la URL base
                var response = await _httpClient.PutAsync($"{endpoint}/{id}", content);

                // 3. Devolver status de exito/fracaso de acuerdo al Status Code HTTP
                if (response.IsSuccessStatusCode)
                {
                    return ApiResult<bool>.Ok(true);
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    return ApiResult<bool>.Fail($"Error {response.StatusCode}: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail($"Error de conexión: {ex.Message}");
            }
        }

        public static async Task<ApiResult<bool>> DeleteAsync(string endpoint, string id)
        {
            try
            {
                // 1. Invocar el método DELETE anexando el Id en la URL
                var response = await _httpClient.DeleteAsync($"{endpoint}/{id}");

                // 2. Procesar respuesta
                if (response.IsSuccessStatusCode)
                {
                    return ApiResult<bool>.Ok(true);
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    return ApiResult<bool>.Fail($"Error {response.StatusCode}: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail($"Error de conexión: {ex.Message}");
            }
        }

        public static async Task<ApiResult<TResult>> PostAndGetResultAsync<TInput, TResult>(string endpoint, TInput data)
        {
            try
            {
                // 1. Serializar el objeto de tipo TInput enviado por el consumidor
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // 2. Realizar POST
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseJson = await response.Content.ReadAsStringAsync();

                // 3. Retornar el DTO esperado del tipo TResult
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiResult<TResult>>(responseJson);
                }
                else
                {
                    try
                    {
                        // 4. Intentar desempaquetar el error provisto por el servidor
                        var errorResult = JsonConvert.DeserializeObject<ApiResult<TResult>>(responseJson);
                        return ApiResult<TResult>.Fail(errorResult?.Message ?? $"Error {response.StatusCode}");
                    }
                    catch
                    {
                        return ApiResult<TResult>.Fail($"Error {response.StatusCode}: {responseJson}");
                    }
                }
            }
            catch (Exception ex)
            {
                return ApiResult<TResult>.Fail($"Error de conexión: {ex.Message}");
            }
        }
    }
}