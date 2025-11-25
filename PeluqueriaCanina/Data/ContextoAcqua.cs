using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Models.ClasesDeAdministrador;
using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Models.ClasesDePago;
using PeluqueriaCanina.Models.ClasesDePeluquero;
using PeluqueriaCanina.Models.ClasesDeTurno;
using PeluqueriaCanina.Models.REPORTES;
using PeluqueriaCanina.Models.Users;

namespace PeluqueriaCanina.Data
{
    public class ContextoAcqua : DbContext
    {
        public ContextoAcqua(DbContextOptions<ContextoAcqua> options) : base(options) { }

        public DbSet<Persona> Personas {  get; set; } 
        public DbSet<Peluquero> Peluqueros { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Jornada> Jornadas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Valoracion> Valoraciones { get; set; }


        //REPORTES
        public DbSet<ReporteServiciosTotales> vw_ReporteServiciosTotales { get; set; }
        public DbSet<ReportePeluquerosPorServicio> vw_ReportePeluquerosPorServicio { get; set; }
        public DbSet<ReporteDetallePeluqueroView> vw_ReporteDetallePeluquero { get; set; }



        //___________________________________________________________________

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Vaslidaciones por email
            modelBuilder.Entity<Peluquero>()
                .HasIndex(p => p.Mail)
                .IsUnique();

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Mail)
                .IsUnique();

            modelBuilder.Entity<Administrador>()
                .HasIndex(a => a.Mail)
                .IsUnique();

            //
            modelBuilder.Entity<Peluquero>()
                .HasMany(p => p.Jornadas)
                .WithOne(j => j.Peluquero)
                .HasForeignKey(j=>j.PeluqueroId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Peluquero>()
                .HasMany(p => p.Turnos)
                .WithOne(t => t.Peluquero)
                .HasForeignKey(t => t.PeluqueroId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mascota>()
                .HasOne(c => c.Cliente)
                .WithMany(m => m.Mascotas)
                .HasForeignKey(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Mascota)
                .WithMany(m => m.Turnos)
                .HasForeignKey(t => t.MascotaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Servicio)
                .WithMany()
                .HasForeignKey(t => t.ServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Peluquero)
                .WithMany(p => p.Turnos)
                .HasForeignKey(t => t.PeluqueroId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Servicio>()
                .Property(s => s.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Turno>()
                .Property(t => t.Precio)
                .HasPrecision(18, 2);

            // ---- RELACIÓN Valoracion -> Peluquero ----
            modelBuilder.Entity<Valoracion>()
                .HasOne(v => v.Peluquero)
                .WithMany(p => p.Valoraciones)
                .HasForeignKey(v => v.PeluqueroId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- RELACIÓN Valoracion -> Cliente ----
            modelBuilder.Entity<Valoracion>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Valoraciones)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- RELACIÓN Valoracion -> Turno ----
            modelBuilder.Entity<Valoracion>()
                .HasOne(v => v.Turno)
                .WithOne(t => t.Valoracion)
                .HasForeignKey<Valoracion>(v => v.TurnoId)
                .OnDelete(DeleteBehavior.Cascade);



            //--------------------------------------------------------------------------------------------------------
            //REPORTES
            // --- REPORTES: mapear vistas (sin clave)
            modelBuilder.Entity<ReporteServiciosTotales>(b =>
            {
                b.HasNoKey();
                b.ToView("vw_ReporteServiciosTotales");
                // Opcional: si la vista devuelve columnas con otros nombres, mapéalas aquí
            });

            modelBuilder.Entity<ReportePeluquerosPorServicio>(b =>
            {
                b.HasNoKey();
                b.ToView("vw_ReportePeluquerosPorServicio");
            });

            modelBuilder.Entity<ReporteDetallePeluqueroView>(b =>
            {
                b.HasNoKey();
                b.ToView("vw_ReporteDetallePeluquero");
            });


        }
    }
}
