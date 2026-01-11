using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

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

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
