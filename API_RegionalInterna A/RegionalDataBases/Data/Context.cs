using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders; // Agrega esta directiva using
using RegionalDataBases.Models;


namespace RegionalDataBases.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<ReporteVenta> ReportesVenta { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Sucursales
            modelBuilder.Entity<Sucursal>(entity =>
            {
                entity.HasKey(e => e.IdSucursal);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Direccion).IsRequired();
                entity.Property(e => e.Telefono).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Activa).IsRequired();
            });

            // Configuración de Categorías
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.IdCategoria);
                entity.Property(e => e.NombreCategoria).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Activo).IsRequired();
            });

            // Configuración de Productos
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.IdProducto);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Precio).HasPrecision(10, 2).IsRequired();
                entity.Property(e => e.Activo).IsRequired();

                // Relación con Categoría
                entity.HasOne(p => p.Categoria)
                    .WithMany(c => c.Productos)
                    .HasForeignKey(p => p.IdCategoria)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Inventario
            modelBuilder.Entity<Inventario>(entity =>
            {
                entity.HasKey(e => e.IdInventario);
                entity.Property(e => e.Existencia).IsRequired();
                entity.Property(e => e.UltimaActualizacion).IsRequired();

                // Relaciones
                entity.HasOne(i => i.Sucursal)
                    .WithMany(s => s.Inventarios)
                    .HasForeignKey(i => i.IdSucursal)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.Producto)
                    .WithMany(p => p.Inventarios)
                    .HasForeignKey(i => i.IdProducto)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Clientes
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.IdCliente);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Telefono).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Direccion).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Activo).IsRequired();

                // Relación con Sucursal
                entity.HasOne(c => c.Sucursal)
                    .WithMany(s => s.Clientes)
                    .HasForeignKey(c => c.IdSucursal)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Reporte de Ventas
            modelBuilder.Entity<ReporteVenta>(entity =>
            {
                entity.HasKey(e => e.IdReporte);
                entity.Property(e => e.TotalVentas).HasPrecision(10, 2).IsRequired();
                entity.Property(e => e.FechaRecepcion).IsRequired();

                // Relaciones
                entity.HasOne(r => r.Sucursal)
                    .WithMany(s => s.ReportesVenta)
                    .HasForeignKey(r => r.IdSucursal)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Cliente)
                    .WithMany(c => c.ReportesVenta)
                    .HasForeignKey(r => r.IdCliente)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Detalle de Ventas
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.HasKey(e => e.IdDetalle);
                entity.Property(e => e.Cantidad).IsRequired();
                entity.Property(e => e.PrecioUnitario).HasPrecision(10, 2).IsRequired();
                entity.Property(e => e.PrecioVenta).HasPrecision(10, 2).IsRequired();

                // Relaciones
                entity.HasOne(d => d.ReporteVenta)
                    .WithMany(r => r.DetallesVenta)
                    .HasForeignKey(d => d.IdReporte)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Producto)
                    .WithMany(p => p.DetallesVenta)
                    .HasForeignKey(d => d.IdProducto)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}