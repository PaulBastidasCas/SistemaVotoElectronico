using Newtonsoft.Json;
using SistemaVotoElectronico.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotoElectronico.ApiConsumer
{
    public static class Crud<T>
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public static string UrlBase = "";

        public static async Task<ApiResult<T>> CreateAsync(T data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(UrlBase, content);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiResult<T>>(responseJson);
                }
                else
                {
                    return ApiResult<T>.Fail($"Error: {response.StatusCode}. Detalles: {responseJson}");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Fail(ex.Message);
            }
        }

        public static async Task<ApiResult<List<T>>> ReadAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(UrlBase);
                var responseJson = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ApiResult<List<T>>>(responseJson);
            }
            catch (Exception ex)
            {
                return ApiResult<List<T>>.Fail(ex.Message);
            }
        }

        public static async Task<ApiResult<T>> ReadByAsync(string field, string value)
        {
            try
            {
                string url;

                if (field.Equals("Id", StringComparison.OrdinalIgnoreCase))
                {
                    url = $"{UrlBase}/{value}";
                }
                else
                {
                    url = $"{UrlBase}/{field}/{value}";
                }

                var response = await _httpClient.GetAsync(url);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiResult<T>>(responseJson);
                }
                else
                {
                    return ApiResult<T>.Fail($"Error: {response.StatusCode}. No se encontró el recurso.");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Fail(ex.Message);
            }
        }

        public static async Task<ApiResult<bool>> UpdateAsync(string id, T data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{UrlBase}/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return ApiResult<bool>.Ok(true);
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    return ApiResult<bool>.Fail($"Error: {response.StatusCode}. Detalles: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }

        public static async Task<ApiResult<bool>> DeleteAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{UrlBase}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return ApiResult<bool>.Ok(true);
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    return ApiResult<bool>.Fail($"Error: {response.StatusCode}. Detalles: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }
    }
}
