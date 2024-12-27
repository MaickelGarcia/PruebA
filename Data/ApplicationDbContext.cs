using EPARCIAL.Models;
using Microsoft.EntityFrameworkCore;

namespace EPARCIAL.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración para la tabla Proyecto
            modelBuilder.Entity<Proyecto>(entity =>
            {
                entity.HasKey(p => p.ID);
                entity.Property(p => p.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(p => p.Estado)
                      .HasConversion<string>()
                      .HasDefaultValue("Pendiente") // Valor predeterminado
                      .IsRequired(false); // Opcional
                entity.HasCheckConstraint("CHK_Fecha", "fecha_inicio <= fecha_fin");
                // Asegurarse de que el proyecto esté activo
                entity.Property(p => p.FlagActivo)
                      .IsRequired()
                      .HasDefaultValue(true);
            });

            // Configuración para la tabla Tarea
            modelBuilder.Entity<Tarea>(entity =>
            {
                entity.HasKey(t => t.ID);
                entity.Property(t => t.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(t => t.Descripcion).HasMaxLength(500).IsRequired();
                entity.Property(t => t.Estado)
                      .HasMaxLength(20)
                      .HasDefaultValue("Pendiente") // Valor predeterminado
                      .IsRequired(false); // Opcional
                entity.Property(t => t.Prioridad)
                      .HasMaxLength(10)
                      .IsRequired();

                // Restricciones
                entity.HasCheckConstraint("CHK_Estado_Tarea", "Estado IN ('Pendiente', 'En Progreso', 'Completada')");
                entity.HasCheckConstraint("CHK_Prioridad_Tarea", "Prioridad IN ('Baja', 'Media', 'Alta')");

                // Aquí ya no se especifica una relación con Proyecto, solo se almacena el ProyectoID
                entity.Property(t => t.ProyectoID)
                      .IsRequired();  // Asegura que el ProyectoID sea requerido

                // Asegurarse de que el FlagActivo de la tarea sea true
                entity.Property(t => t.FlagActivo)
                      .IsRequired()
                      .HasDefaultValue(true);
            });
        }
    }
}
