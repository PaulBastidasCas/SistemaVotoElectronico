using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;
using System.Security.Claims;
using System.Text;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _apiBaseUrl = "http://localhost:5050/api";
        private readonly HttpClient _httpClient;

        public AccountController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto modelo)
        {
            try
            {
                var json = JsonConvert.SerializeObject(modelo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Login", content);
                var responseString = await response.Content.ReadAsStringAsync();

                var resultado = JsonConvert.DeserializeObject<ApiResult<dynamic>>(responseString);

                if (resultado != null && resultado.Success)
                {
                    string rol = (string)resultado.Data.Rol;
                    string nombre = (string)resultado.Data.Nombre;
                    string correo = (string)resultado.Data.Correo;
                    string id = (string)resultado.Data.Id;

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, nombre),
                        new Claim(ClaimTypes.Email, correo),
                        new Claim(ClaimTypes.Role, rol),
                        new Claim(ClaimTypes.NameIdentifier, id)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                    if (rol == "Administrador")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (rol == "Candidato")
                    {
                        return RedirectToAction("Index", "Home"); 
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); 
                    }
                }

                ViewData["Error"] = resultado?.Message ?? "Credenciales inválidas";
                return View(modelo);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = "Error: " + ex.Message;
                return View(modelo);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistroDto modelo)
        {
            if (!ModelState.IsValid) return View(modelo);

            try
            {
                Crud<Votante>.UrlBase = $"{_apiBaseUrl}/Votantes";

                var nuevoVotante = new Votante
                {
                    Cedula = modelo.Cedula,
                    NombreCompleto = modelo.NombreCompleto,
                    Correo = modelo.Correo,
                    Contrasena = modelo.Contrasena, 
                    Rol = "Votante",
                    Fotografia = ""  
                };

                var resultado = await Crud<Votante>.CreateAsync(nuevoVotante);

                if (resultado.Success)
                {
                    TempData["SuccessMessage"] = "Registro exitoso. Inicie sesión para votar.";
                    return RedirectToAction("Login");
                }

                ViewData["Error"] = resultado.Message;
                return View(modelo);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = $"Error: {ex.Message}";
                return View(modelo);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
