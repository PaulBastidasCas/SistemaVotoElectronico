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
        public static string UrlBase = "";

        // consumir una API y ejecutar el vervo POST
        public static ApiResult<T> Create(T data)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // invocar al servicio web
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var response = httpClient.PostAsync(UrlBase, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        json = response.Content.ReadAsStringAsync().Result;
                        // deserializar la respuesta
                        var newData = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<T>>(json);
                        return newData;
                    }
                    else
                    {
                        var errorResponse = response.Content.ReadAsStringAsync().Result;
                        return ApiResult<T>.Fail($"Error: {response.StatusCode}. Detalles: {errorResponse}");
                    }
                }
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Fail(ex.Message);
            }
        }

        public static ApiResult<List<T>> ReadAll()
        {
            // consumir una API y ejecutar el verbo GET
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // invocar al servicio web
                    var response = httpClient.GetAsync(UrlBase).Result;
                    var json = response.Content.ReadAsStringAsync().Result;
                    // deserializar la respuesta
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<List<T>>>(json);
                    return data;
                }
            }
            catch (Exception ex)
            {
                return ApiResult<List<T>>.Fail(ex.Message);
            }
        }

        public static ApiResult<T> ReadBy(string field, string value)
        {
            // consumir una API y ejecutar el verbo GET con parámetros
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // invocar al servicio web
                    var response = httpClient.GetAsync($"{UrlBase}/{field}/{value}").Result;
                    var json = response.Content.ReadAsStringAsync().Result;
                    // deserializar la respuesta
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<T>>(json);
                    return data;
                }
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Fail(ex.Message);
            }
        }

        public static ApiResult<bool> Update(string id, T data)
        {
            // consumir una API y ejecutar el verbo PUT
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // invocar al servicio web
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var response = httpClient.PutAsync($"{UrlBase}/{id}", content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return ApiResult<bool>.Ok(true);
                    }
                    else
                    {
                        return ApiResult<bool>.Fail($"Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }

        public static ApiResult<bool> Delete(string id)
        {
            // consumir una API y ejecutar el verbo DELETE
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // invocar al servicio web
                    var response = httpClient.DeleteAsync($"{UrlBase}/{id}").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return ApiResult<bool>.Ok(true);
                    }
                    else
                    {
                        return ApiResult<bool>.Fail($"Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }
    }
}
