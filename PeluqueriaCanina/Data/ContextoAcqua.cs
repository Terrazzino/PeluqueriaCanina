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

        //REPORTES
        // REPORTES
        public DbSet<ReporteServicios> ReporteServicios { get; set; }
        public DbSet<ReportePeluquerosPorServicio> ReportePeluqueroPorServicio { get; set; }
        public DbSet<ReporteDetallePeluquero> ReporteDetallePeluquero { get; set; }



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

            //--------------------------------------------------------------------------------------------------------
            //REPORTES
            //DE PRUEBA PARA
            //BASE DE DATOS APLICADA

            modelBuilder.Entity<ReporteServicios>(b =>
            {
                b.ToTable("ReporteServicios");
                b.HasKey(x => x.Id);
                b.Property(x => x.NombreServicio).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<ReportePeluquerosPorServicio>(b =>
            {
                b.ToTable("ReportePeluqueroPorServicio"); // el nombre que usaste en el SQL
                b.HasKey(x => x.Id);
                b.Property(x => x.NombrePeluquero).HasMaxLength(100).IsRequired();

                b.HasOne(r => r.ReporteServicio)
                 .WithMany()
                 .HasForeignKey(r => r.ReporteServicioId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ReporteDetallePeluquero>(b =>
            {
                b.ToTable("ReporteDetallePeluquero");
                b.HasKey(x => x.Id);
                b.Property(x => x.Recaudado).HasColumnType("decimal(18,2)");

                b.HasOne(d => d.ReportePeluqueroPorServicio)
                 .WithMany(p => p.Detalles)
                 .HasForeignKey(d => d.ReportePeluqueroPorServicioId)
                 .OnDelete(DeleteBehavior.Cascade);
            });


        }
    }
}
