using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos.Entidades;
using SistemaVotoElectronico.MVC.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiBaseUrl"] ?? "http://localhost:5051/api";
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Perfil()
        {
            var model = new PerfilViewModel
            {
                Nombre = "Cargando...",
                Foto = null,
                Items = new List<dynamic>()
            };

            var rol = User.FindFirst(ClaimTypes.Role)?.Value;
            var correo = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(correo))
            {
                return RedirectToAction("Index");
            }

            model.Rol = rol;
            model.Correo = correo;

            bool datosEncontrados = false;

            try
            {
                if (User.IsInRole("Administrador"))
                {
                    model.Stat1Label = "Nivel"; model.Stat2Label = "Permisos"; model.Stat2Value = "Total";
                    var respuesta = await Crud<Administrador>.ReadAllAsync($"{_apiBaseUrl}/Administradores");
                    if (respuesta.Success)
                    {
                        var user = respuesta.Data.FirstOrDefault(x => x.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));
                        if (user != null)
                        {
                            model.Nombre = user.NombreCompleto;
                            model.Stat1Value = "SuperAdmin";
                            model.Foto = user.Fotografia;
                            datosEncontrados = true;
                        }
                    }
                }
                else if (User.IsInRole("JefeDeMesa")) 
                {
                    model.Stat1Label = "Mesa"; model.Stat2Label = "Recinto"; model.Stat2Value = "Principal";
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
                    model.Stat1Label = "Cedula"; model.Stat2Label = "Estado"; model.Stat2Value = "Habilitado";
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
                                    string nombre = asignacion.Eleccion != null ? asignacion.Eleccion.Nombre : $"Eleccion #{asignacion.EleccionId}";
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
                model.Nombre = "Error de Conexion";
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
                    TempData["Mensaje"] = "Sesion no valida.";
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
                }

                if (exito)
                {
                    TempData["Mensaje"] = "Foto actualizada correctamente!";
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
