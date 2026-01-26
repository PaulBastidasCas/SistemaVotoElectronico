global using iText.Kernel.Pdf;
global using iText.Layout;
global using iText.Layout.Element;
global using iText.Layout.Properties;
global using iText.IO.Font.Constants;
global using iText.Kernel.Font;
global using iText.Kernel.Colors;
global using System.Net;
global using System.Net.Mail;
global using System.IO;
global using SistemaVotoElectronico.Api.Data;
global using SistemaVotoElectronico.Modelos;
global using Microsoft.EntityFrameworkCore;
using Serilog;

namespace SistemaVotoElectronico.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //==============================================================
            // Configurar Serilog leyendo desde appsettings.json
            //==============================================================
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            Log.Information("Iniciado el proceso de LOGGER");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            builder.Services.AddDbContext<SistemaVotoElectronicoApiContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("SistemaVotacionAPIContext.postgresql")
            ?? throw new InvalidOperationException("Connection string not found.")));


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services
               .AddControllers()
               .AddNewtonsoftJson(
                   options =>
                   options.SerializerSettings.ReferenceLoopHandling
                   = Newtonsoft.Json.ReferenceLoopHandling.Ignore
               );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseResponseCompression();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
