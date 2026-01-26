using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVotoElectronico.Modelos.Entidades;

namespace SistemaVotoElectronico.Api.Data
{
    public class SistemaVotoElectronicoApiContext : DbContext
    {
        public SistemaVotoElectronicoApiContext(DbContextOptions<SistemaVotoElectronicoApiContext> options)
            : base(options)
        {
        }

        public DbSet<Administrador> Administradores { get; set; } = default!;
        public DbSet<Candidato> Candidatos { get; set; } = default!;
        public DbSet<Eleccion> Elecciones { get; set; } = default!;
        public DbSet<ListaElectoral> ListaElectorales { get; set; } = default!;
        public DbSet<PadronElectoral> PadronElectorales { get; set; } = default!;
        public DbSet<Votante> Votantes { get; set; } = default!;
        public DbSet<Voto> Votos { get; set; } = default!;
        public DbSet<JefeDeMesa> JefesDeMesa { get; set; } = default!;
        public DbSet<Mesa> Mesas { get; set; } = default!;
        public DbSet<SolicitudRecuperacion> SolicitudesRecuperacion { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // 1. Índices para LOGIN (Búsqueda rápida por Correo)
            modelBuilder.Entity<Administrador>().HasIndex(x => x.Correo).IsUnique();
            modelBuilder.Entity<JefeDeMesa>().HasIndex(x => x.Correo).IsUnique();
            modelBuilder.Entity<Candidato>().HasIndex(x => x.Correo).IsUnique();
            modelBuilder.Entity<Votante>().HasIndex(x => x.Correo).IsUnique();
            modelBuilder.Entity<Votante>().HasIndex(x => x.Cedula).IsUnique();
            // Ayuda a validar códigos de enlace rápido
            modelBuilder.Entity<PadronElectoral>().HasIndex(x => x.CodigoEnlace);
        }
    }
}