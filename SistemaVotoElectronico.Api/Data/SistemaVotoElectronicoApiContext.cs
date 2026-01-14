using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVotoElectronico.Modelos;

    public class SistemaVotoElectronicoApiContext : DbContext
    {
        public SistemaVotoElectronicoApiContext (DbContextOptions<SistemaVotoElectronicoApiContext> options)
            : base(options)
        {
        }

        public DbSet<SistemaVotoElectronico.Modelos.Administrador> Administradores { get; set; } = default!;

public DbSet<SistemaVotoElectronico.Modelos.Candidato> Candidatos { get; set; } = default!;

public DbSet<SistemaVotoElectronico.Modelos.Eleccion> Elecciones { get; set; } = default!;

public DbSet<SistemaVotoElectronico.Modelos.ListaElectoral> ListaElectorales { get; set; } = default!;

public DbSet<SistemaVotoElectronico.Modelos.PadronElectoral> PadronElectorales { get; set; } = default!;

public DbSet<SistemaVotoElectronico.Modelos.Votante> Votantes { get; set; } = default!;

public DbSet<SistemaVotoElectronico.Modelos.Voto> Votos { get; set; } = default!;

public DbSet<SistemaVotoElectronico.Modelos.JefeDeMesa> JefesDeMesa { get; set; } = default!;

public DbSet<SistemaVotoElectronico.Modelos.Mesa> Mesas { get; set; } = default!;
    }
