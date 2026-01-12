using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SistemaVotoElectronico.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string api = "http://localhost:5050/api";

            Crud<Administrador>.UrlBase = $"{api}/Administradores";
            Crud<Candidato>.UrlBase = $"{api}/Candidatos";
            Crud<Eleccion>.UrlBase = $"{api}/Elecciones";
            Crud<ListaElectoral>.UrlBase = $"{api}/ListaElectorales";
            Crud<PadronElectoral>.UrlBase = $"{api}/PadronElectorales";
            Crud<Votante>.UrlBase = $"{api}/Votantes";
            Crud<Voto>.UrlBase = $"{api}/Votos";

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); 
                    options.AccessDeniedPath = "/Account/AccessDenied"; 
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication(); 
            app.UseAuthorization();  

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}"); 

            app.Run();
        }
    }
}
