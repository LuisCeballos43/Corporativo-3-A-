using Microsoft.EntityFrameworkCore;
using API_RegionalInterna.Models;

namespace API_RegionalInterna.Data
{
    public class RegionalDbContext : DbContext
    {
        public RegionalDbContext(DbContextOptions<RegionalDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<ReporteSemanalDetalle> ReporteSemanalDetalles { get; set; }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }

        public DbSet<ReportesVentas> ReportesVentas { get; set; }


        public DbSet<Region> Regiones { get; set; } = null!;

    }
}
