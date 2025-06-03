using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using _200_OK_SALUD___Backend.Models;

namespace _200_OK_SALUD___Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<MedicoEspecialidad> MedicoEspecialidades { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<HistorialMedico> HistorialMedicos { get; set; }
        public DbSet<ReglaPrioridad> ReglaPrioridades { get; set; }
        public DbSet<CitaPrioridad> CitaPrioridades { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Claves compuestas
            builder.Entity<MedicoEspecialidad>()
                .HasKey(me => new { me.MedicoId, me.EspecialidadId });

            builder.Entity<CitaPrioridad>()
                .HasKey(cp => new { cp.CitaId, cp.ReglaId });

            // Relación Cita → Medico: indicamos la colección inversa y FK
            builder.Entity<Cita>()
                .HasOne(c => c.Medico)
                .WithMany(m => m.Citas)
                .HasForeignKey(c => c.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Cita → Paciente: indicamos la colección inversa y FK
            builder.Entity<Cita>()
                .HasOne(c => c.Paciente)
                .WithMany(p => p.Citas)
                .HasForeignKey(c => c.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Especificar precisión para los decimal:
            builder.Entity<Cita>()
                .Property(c => c.PuntajePrioridad)
                .HasPrecision(5, 2);

            builder.Entity<ReglaPrioridad>()
                .Property(r => r.Peso)
                .HasPrecision(5, 2);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var ahora = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries()
                .Where(e =>
                    e.Entity is Especialidad ||
                    e.Entity is Paciente ||
                    e.Entity is Medico ||
                    e.Entity is Cita ||
                    e.Entity is HistorialMedico ||
                    e.Entity is ReglaPrioridad ||
                    e.Entity is CitaPrioridad ||
                    e.Entity is Notificacion))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Property("CreatedAt").CurrentValue = ahora;
                        entry.Property("UpdatedAt").CurrentValue = ahora;
                        break;
                    case EntityState.Modified:
                        entry.Property("UpdatedAt").CurrentValue = ahora;
                        entry.Property("CreatedAt").IsModified = false;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
