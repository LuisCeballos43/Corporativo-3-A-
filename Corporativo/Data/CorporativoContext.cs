using Microsoft.EntityFrameworkCore;
using Corporativo.Models;
namespace Corporativo.Data
{
    public class CorporativoContext : DbContext
    {
        public CorporativoContext(DbContextOptions<CorporativoContext> options)
            : base(options)
        {
        }

        public DbSet<Region> Regiones { get; set; }
        public DbSet<ReporteMensual> ReportesMensuales { get; set; }
        public DbSet<DetalleReporte> DetallesReporte { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para Region
            modelBuilder.Entity<Region>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ApiUrl).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Activa).IsRequired();
            });

            // Configuración para ReporteMensual
            modelBuilder.Entity<ReporteMensual>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Índice único compuesto para evitar duplicados de región/año/mes
                entity.HasIndex(e => new { e.IdRegion, e.Año, e.Mes })
                      .IsUnique()
                      .HasDatabaseName("IX_ReportesMensuales_Region_Año_Mes");

                entity.Property(e => e.TotalVentas)
                      .HasColumnType("decimal(12, 2)");

                // Relación con Region
                entity.HasOne(e => e.Region)
                      .WithMany(r => r.ReportesMensuales)
                      .HasForeignKey(e => e.IdRegion)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración para DetalleReporte
            modelBuilder.Entity<DetalleReporte>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Subtotal)
                      .HasColumnType("decimal(10, 2)");

                // Relación con ReporteMensual
                entity.HasOne(e => e.ReporteMensual)
                      .WithMany(r => r.DetallesReporte)
                      .HasForeignKey(e => e.IdReporte)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
