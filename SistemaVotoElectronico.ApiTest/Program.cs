using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.ApiTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Crud<Eleccion>.UrlBase = "http://localhost:5050/api/Elecciones";
            Crud<ListaElectoral>.UrlBase = "http://localhost:5050/api/ListaElectorales";
            Crud<Candidato>.UrlBase = "http://localhost:5050/api/Candidatos";

            Console.WriteLine("--- INICIANDO TEST DE VOTO ELECTRONICO ---");

            var nuevaEleccion = new Eleccion
            {
                Nombre = "Elecciones Generales 2026",
                TipoEleccion = "Nominal",
                FechaInicio = DateTime.UtcNow,
                FechaFinalizacion = DateTime.UtcNow.AddDays(1),
                Activa = true,
                Listas = new List<ListaElectoral>()
            };

            Console.WriteLine("\n[POST] Creando Elección...");
            var resultadoEleccion = Crud<Eleccion>.Create(nuevaEleccion);

            if (resultadoEleccion.Success)
            {
                var eleccionCreada = resultadoEleccion.Data;
                Console.WriteLine($"SUCCESS: Elección creada con ID: {eleccionCreada.Id}");

                var nuevaLista = new ListaElectoral
                {
                    Nombre = "Movimiento Innovación",
                    NumeroLista = "Lista 10",
                    Siglas = "MI",
                    Color = "#0000FF",
                    Logotipo = "logo.png",
                    EleccionId = eleccionCreada.Id 
                };

                Console.WriteLine("\n[POST] Creando Lista Electoral...");
                var resultadoLista = Crud<ListaElectoral>.Create(nuevaLista);

                if (resultadoLista.Success)
                {
                    var listaCreada = resultadoLista.Data;
                    Console.WriteLine($"SUCCESS: Lista creada con ID: {listaCreada.Id}");

                    var nuevoCandidato = new Candidato
                    {
                        NombreCompleto = "Roberto Gómez",
                        Cedula = "0900000001",
                        Correo = "roberto@voto.com",
                        Contrasena = "12345",
                        Fotografia = "logo.png",
                        Rol = "Candidato", 
                        OrdenEnLista = 1,
                        ListaElectoralId = listaCreada.Id 
                    };

                    Console.WriteLine("\n[POST] Creando Candidato...");
                    var resultadoCandidato = Crud<Candidato>.Create(nuevoCandidato);

                    if (resultadoCandidato.Success)
                    {
                        var candidatoCreado = resultadoCandidato.Data;
                        Console.WriteLine($"SUCCESS: Candidato creado: {candidatoCreado.NombreCompleto}");
                        Console.WriteLine("\n[PUT] Actualizando nombre del Candidato...");
                        candidatoCreado.NombreCompleto = "Roberto Gómez Bolaños";

                        var resultadoUpdate = Crud<Candidato>.Update(candidatoCreado.Id.ToString(), candidatoCreado);
                        Console.WriteLine($"Resultado Update: {resultadoUpdate.Success}");

                        Console.WriteLine("\n[GET] Leyendo todos los candidatos...");
                        var listaCandidatos = Crud<Candidato>.ReadAll();

                        if (listaCandidatos.Success)
                        {
                            foreach (var c in listaCandidatos.Data)
                            {
                                Console.WriteLine($" - ID: {c.Id} | Nombre: {c.NombreCompleto} | ListaID: {c.ListaElectoralId}");
                            }
                        }

                        Console.WriteLine("\n[DELETE] Eliminando candidato...");
                        Crud<Candidato>.Delete(candidatoCreado.Id.ToString());
                    }
                    else
                    {
                        Console.WriteLine($"ERROR Candidato: {resultadoCandidato.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"ERROR Lista: {resultadoLista.Message}");
                }
            }
            else
            {
                Console.WriteLine($"ERROR Elección: {resultadoEleccion.Message}");
            }

            Console.WriteLine("\nPresiona ENTER para salir...");
            Console.ReadLine();
        }
    }
}
