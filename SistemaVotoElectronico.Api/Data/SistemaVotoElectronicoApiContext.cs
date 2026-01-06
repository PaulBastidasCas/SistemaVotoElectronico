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

        public DbSet<SistemaVotoElectronico.Modelos.Administrador> Administrador { get; set; } = default!;

public DbSet<SistemaVotoElectronico.Modelos.Candidato> Candidato { get; set; } = default!;

public DbSet<SistemaVotoElectronico.Modelos.Eleccion> Eleccion { get; set; } = default!;

public DbSet<SistemaVotoElectronico.Modelos.ListaElectoral> ListaElectoral { get; set; } = default!;

public DbSet<SistemaVotoElectronico.Modelos.PadronElectoral> PadronElectoral { get; set; } = default!;

public DbSet<SistemaVotoElectronico.Modelos.Votante> Votante { get; set; } = default!;

public DbSet<SistemaVotoElectronico.Modelos.Voto> Voto { get; set; } = default!;
    }
