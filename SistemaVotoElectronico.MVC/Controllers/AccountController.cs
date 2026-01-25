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
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;

        public AccountController(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5051/api";
            _httpClient = new HttpClient();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Perfil", "Home");
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

                var resultado = JsonConvert.DeserializeObject<ApiResult<LoginResponseDto>>(responseString);

                if (resultado != null && resultado.Success && resultado.Data != null)
                {
                    string rol = resultado.Data.Rol;
                    string nombre = resultado.Data.Nombre;
                    string correo = resultado.Data.Correo;
                    string id = resultado.Data.Id.ToString();

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, nombre),
                        new Claim(ClaimTypes.Email, correo),
                        new Claim(ClaimTypes.Role, rol),
                        new Claim(ClaimTypes.NameIdentifier, id)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Perfil", "Home");
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
                string endpoint = $"{_apiBaseUrl}/Votantes";

                var nuevoVotante = new Votante
                {
                    Cedula = modelo.Cedula,
                    NombreCompleto = modelo.NombreCompleto,
                    Correo = modelo.Correo,
                    Contrasena = modelo.Contrasena,
                    Rol = "Votante",
                    Fotografia = ""
                };

                var resultado = await Crud<Votante>.CreateAsync(endpoint, nuevoVotante);

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


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(RecuperarDto modelo)
        {
            if (!ModelState.IsValid) return View(modelo);

            var resultado = await Crud<bool>.PostAndGetResultAsync<RecuperarDto, bool>($"{_apiBaseUrl}/Auth/Recuperar", modelo);

            if (resultado.Success)
            {
                TempData["Mensaje"] = "Si el correo existe, se ha enviado un código.";
                return RedirectToAction("ResetPassword");
            }

            ViewData["Error"] = resultado.Message;
            return View(modelo);
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto modelo)
        {
            if (!ModelState.IsValid) return View(modelo);

            var resultado = await Crud<bool>.PostAndGetResultAsync<ResetPasswordDto, bool>($"{_apiBaseUrl}/Auth/Reset", modelo);

            if (resultado.Success)
            {
                TempData["SuccessMessage"] = "Contraseña actualizada. Inicia sesión.";
                return RedirectToAction("Login");
            }

            ViewData["Error"] = resultado.Message;
            return View(modelo);
        }
    }

    public class LoginResponseDto
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
        public int Id { get; set; }
    }
}
