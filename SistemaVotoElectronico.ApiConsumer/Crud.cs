using Newtonsoft.Json;
using SistemaVotoElectronico.Modelos;
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
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseJson = await response.Content.ReadAsStringAsync();

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
                var response = await _httpClient.GetAsync(endpoint);
                var responseJson = await response.Content.ReadAsStringAsync();

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
                string url;
                if (string.Equals(field, "Id", StringComparison.OrdinalIgnoreCase))
                {
                    url = $"{endpointBase}/{value}";
                }
                else
                {
                    url = $"{endpointBase}/{field}/{value}";
                }

                var response = await _httpClient.GetAsync(url);
                var responseJson = await response.Content.ReadAsStringAsync();

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
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{endpoint}/{id}", content);

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
                var response = await _httpClient.DeleteAsync($"{endpoint}/{id}");

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
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiResult<TResult>>(responseJson);
                }
                else
                {
                    try
                    {
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