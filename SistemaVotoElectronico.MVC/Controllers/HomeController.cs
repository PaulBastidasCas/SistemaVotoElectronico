using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;
using SistemaVotoElectronico.Modelos.Entidades;
using SistemaVotoElectronico.MVC.Models;
using System.Diagnostics;
using System.Dynamic;
using System.Security.Claims;

namespace SistemaVotoElectronico.MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly string _apiBaseUrl;

        public HomeController(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5051/api";
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Perfil()
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;
            var correo = User.FindFirst(ClaimTypes.Email)?.Value;

            dynamic model = new ExpandoObject();

            model.Rol = rol ?? "Desconocido";
            model.Correo = correo ?? "Sin Correo";
            model.Nombre = "Cargando...";
            model.Foto = "";
            model.Items = new List<dynamic>();
            model.ListaId = 0;
            model.Stat1Label = "Dato 1"; model.Stat1Value = "-";
            model.Stat2Label = "Dato 2"; model.Stat2Value = "-";

            bool datosEncontrados = false;

            try
            {
                if (rol == "Administrador")
                {
                    model.Stat1Label = "ID Admin"; model.Stat2Label = "Sistema"; model.Stat2Value = "Online";
                    var respuesta = await Crud<Administrador>.ReadAllAsync($"{_apiBaseUrl}/Administradores");
                    if (respuesta.Success)
                    {
                        var user = respuesta.Data.FirstOrDefault(x => x.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));
                        if (user != null)
                        {
                            model.Nombre = user.NombreCompleto;
                            model.Foto = user.Fotografia;
                            model.Stat1Value = user.Id.ToString();
                            datosEncontrados = true;
                        }
                    }
                }
                else if (rol == "Candidato")
                {
                    model.Stat1Label = "Orden"; model.Stat2Label = "ID";
                    var respuesta = await Crud<Candidato>.ReadAllAsync($"{_apiBaseUrl}/Candidatos");
                    if (respuesta.Success)
                    {
                        var user = respuesta.Data.FirstOrDefault(x => x.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));
                        if (user != null)
                        {
                            model.Nombre = user.NombreCompleto;
                            model.Foto = user.Fotografia;
                            model.Stat1Value = user.OrdenEnLista.ToString();
                            model.Stat2Value = user.Id.ToString();
                            model.ListaId = user.ListaElectoralId ?? 0;
                            datosEncontrados = true;
                        }
                    }
                }
                else if (rol == "JefeDeMesa")
                {
                    model.Stat1Label = "Mesa"; model.Stat2Label = "Zona"; model.Stat2Value = "Norte";
                    var respuesta = await Crud<JefeDeMesa>.ReadAllAsync($"{_apiBaseUrl}/JefesDeMesa");
                    if (respuesta.Success)
                    {
                        var user = respuesta.Data.FirstOrDefault(x => x.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));
                        if (user != null)
                        {
                            model.Nombre = user.NombreCompleto;
                            model.Stat1Value = user.MesaAsignada != null ? user.MesaAsignada.Nombre : "Sin Asignar";
                            model.Foto = user.Fotografia;
                            datosEncontrados = true;
                        }
                    }
                }
                else // Votante
                {
                    model.Stat1Label = "Cédula"; model.Stat2Label = "Estado"; model.Stat2Value = "Habilitado";
                    var respuesta = await Crud<Votante>.ReadAllAsync($"{_apiBaseUrl}/Votantes");
                    if (respuesta.Success)
                    {
                        var user = respuesta.Data.FirstOrDefault(x => x.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));
                        if (user != null)
                        {
                            model.Nombre = user.NombreCompleto;
                            model.Foto = user.Fotografia;
                            model.Stat1Value = user.Cedula;

                            // Cargar historial
                            var padronRes = await Crud<PadronElectoral>.ReadAllAsync($"{_apiBaseUrl}/PadronElectorales");
                            if (padronRes.Success && padronRes.Data != null)
                            {
                                var misAsignaciones = padronRes.Data.Where(p => p.VotanteId == user.Id).ToList();
                                foreach (var asignacion in misAsignaciones)
                                {
                                    string nombre = asignacion.Eleccion != null ? asignacion.Eleccion.Nombre : $"Elección #{asignacion.EleccionId}";
                                    model.Items.Add(new { Titulo = nombre, HaVotado = asignacion.CodigoCanjeado });
                                }
                            }
                            datosEncontrados = true;
                        }
                    }
                }

                if (!datosEncontrados) model.Nombre = "Usuario no encontrado (Verifique correo)";
            }
            catch (Exception ex)
            {
                model.Nombre = "Error de Conexión";
                model.Stat1Value = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePhoto(IFormFile fotoUpload)
        {
            if (fotoUpload == null || fotoUpload.Length == 0)
            {
                TempData["Mensaje"] = "Por favor selecciona una imagen.";
                TempData["Tipo"] = "danger";
                return RedirectToAction("Perfil");
            }

            try
            {
                string base64Foto = "";
                using (var ms = new MemoryStream())
                {
                    await fotoUpload.CopyToAsync(ms);
                    var bytes = ms.ToArray();
                    var ext = Path.GetExtension(fotoUpload.FileName).Replace(".", "");
                    base64Foto = $"data:image/{ext};base64,{Convert.ToBase64String(bytes)}";
                }

                var rol = User.FindFirst(ClaimTypes.Role)?.Value;
                var correo = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(correo))
                {
                    TempData["Mensaje"] = "Sesión no válida.";
                    return RedirectToAction("Index");
                }

                bool exito = false;
                string errorApi = "";

                switch (rol)
                {
                    case "Administrador":
                        var resA = await Crud<Administrador>.ReadAllAsync($"{_apiBaseUrl}/Administradores");
                        var admin = resA.Data?.FirstOrDefault(x => x.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));
                        if (admin != null)
                        {
                            admin.Fotografia = base64Foto;
                            admin.Contrasena = null; 
                            var update = await Crud<Administrador>.UpdateAsync($"{_apiBaseUrl}/Administradores", admin.Id.ToString(), admin);
                            exito = update.Success; errorApi = update.Message;
                        }
                        break;

                    case "JefeDeMesa":
                        var resJ = await Crud<JefeDeMesa>.ReadAllAsync($"{_apiBaseUrl}/JefesDeMesa");
                        var jefe = resJ.Data?.FirstOrDefault(x => x.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));
                        if (jefe != null)
                        {
                            jefe.Fotografia = base64Foto;
                            jefe.Contrasena = null;
                            var update = await Crud<JefeDeMesa>.UpdateAsync($"{_apiBaseUrl}/JefesDeMesa", jefe.Id.ToString(), jefe);
                            exito = update.Success; errorApi = update.Message;
                        }
                        break;

                    case "Votante":
                        var resV = await Crud<Votante>.ReadAllAsync($"{_apiBaseUrl}/Votantes");
                        var votante = resV.Data?.FirstOrDefault(x => x.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));
                        if (votante != null)
                        {
                            votante.Fotografia = base64Foto;
                            votante.Contrasena = null;
                            var update = await Crud<Votante>.UpdateAsync($"{_apiBaseUrl}/Votantes", votante.Id.ToString(), votante);
                            exito = update.Success; errorApi = update.Message;
                        }
                        break;

                    case "Candidato":
                        var resC = await Crud<Candidato>.ReadAllAsync($"{_apiBaseUrl}/Candidatos");
                        var candidato = resC.Data?.FirstOrDefault(x => x.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));
                        if (candidato != null)
                        {
                            candidato.Fotografia = base64Foto;
                            candidato.Contrasena = null;
                            var update = await Crud<Candidato>.UpdateAsync($"{_apiBaseUrl}/Candidatos", candidato.Id.ToString(), candidato);
                            exito = update.Success; errorApi = update.Message;
                        }
                        break;
                }

                if (exito)
                {
                    TempData["Mensaje"] = "¡Foto actualizada correctamente!";
                    TempData["Tipo"] = "success";
                }
                else
                {
                    TempData["Mensaje"] = $"Error al actualizar en API: {errorApi}";
                    TempData["Tipo"] = "warning";
                }

            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error interno: {ex.Message}";
                TempData["Tipo"] = "danger";
            }

            return RedirectToAction("Perfil");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}