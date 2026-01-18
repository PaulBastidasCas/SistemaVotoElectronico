using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;
using SistemaVotoElectronico.MVC.Models;
using System.Diagnostics;
using System.Dynamic;
using System.Security.Claims;

namespace SistemaVotoElectronico.MVC.Controllers
{
    [Authorize] 
    public class HomeController : Controller
    {
        private readonly string _apiBaseUrl = "http://localhost:5050/api";

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
            model.Nombre = "Usuario (Datos no encontrados)"; 
            model.Foto = "";
            model.Items = new List<dynamic>();

            model.Stat1Label = "Dato 1"; model.Stat1Value = "-";
            model.Stat2Label = "Dato 2"; model.Stat2Value = "-";

            bool datosEncontrados = false;

            try
            {
                if (rol == "Administrador")
                {
                    model.Stat1Label = "ID Admin";
                    model.Stat2Label = "Sistema"; model.Stat2Value = "Online";

                    Crud<Administrador>.UrlBase = $"{_apiBaseUrl}/Administradores";
                    var respuesta = await Crud<Administrador>.ReadAllAsync();

                    if (respuesta.Success)
                    {
                        var admin = respuesta.Data.FirstOrDefault(x => x.Correo == correo);
                        if (admin != null)
                        {
                            model.Nombre = admin.NombreCompleto; 
                            model.Stat1Value = admin.Id.ToString();
                            datosEncontrados = true;
                        }
                    }
                }
                else if (rol == "Candidato")
                {
                    model.Stat1Label = "Orden";
                    model.Stat2Label = "ID";

                    Crud<Candidato>.UrlBase = $"{_apiBaseUrl}/Candidatos";
                    var respuesta = await Crud<Candidato>.ReadAllAsync();

                    if (respuesta.Success)
                    {
                        var candidato = respuesta.Data.FirstOrDefault(x => x.Correo == correo);
                        if (candidato != null)
                        {
                            model.Nombre = candidato.NombreCompleto;
                            model.Foto = candidato.Fotografia;
                            model.Stat1Value = candidato.OrdenEnLista.ToString();
                            model.Stat2Value = candidato.Id.ToString();
                            datosEncontrados = true;
                        }
                    }
                }
                else if (rol == "JefeDeMesa")
                {
                    model.Stat1Label = "Mesa";
                    model.Stat2Label = "Zona"; model.Stat2Value = "Norte";

                    Crud<JefeDeMesa>.UrlBase = $"{_apiBaseUrl}/JefesDeMesa";
                    var respuesta = await Crud<JefeDeMesa>.ReadAllAsync();

                    if (respuesta.Success)
                    {
                        var jefe = respuesta.Data.FirstOrDefault(x => x.Correo == correo);
                        if (jefe != null)
                        {
                            model.Nombre = jefe.NombreCompleto;
                            model.Stat1Value = jefe.MesaAsignada != null ? jefe.MesaAsignada.Id.ToString() : "Sin Asignar";
                            datosEncontrados = true;
                        }
                    }
                }
                else 
                {
                    model.Stat1Label = "Cédula";
                    model.Stat2Label = "Estado"; model.Stat2Value = "Habilitado";

                    Crud<Votante>.UrlBase = $"{_apiBaseUrl}/Votantes";
                    var respuesta = await Crud<Votante>.ReadAllAsync();

                    if (respuesta.Success)
                    {
                        var votante = respuesta.Data.FirstOrDefault(x => x.Correo == correo);
                        if (votante != null)
                        {
                            model.Nombre = votante.NombreCompleto;
                            model.Foto = votante.Fotografia;
                            model.Stat1Value = votante.Cedula;

                            if (votante.HistorialVotos != null)
                            {
                                foreach (var voto in votante.HistorialVotos)
                                {
                                    model.Items.Add(new { Titulo = "Elección ID: " + voto.EleccionId, Detalle = "Voto realizado" });
                                }
                            }
                            datosEncontrados = true;
                        }
                    }
                }

                if (!datosEncontrados)
                {
                    model.Nombre = "Usuario no encontrado en BD";
                }
            }
            catch (Exception ex)
            {
                model.Nombre = "Error de Conexión";
                model.Stat1Label = "Error";
                model.Stat1Value = ex.Message;
            }

            return View(model);
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
