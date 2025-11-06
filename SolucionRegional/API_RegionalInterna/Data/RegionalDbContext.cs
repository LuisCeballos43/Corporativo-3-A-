using Microsoft.EntityFrameworkCore;
using API_RegionalInterna.Models;

namespace API_RegionalInterna.Data
{
    public class RegionalDbContext : DbContext
    {
        public RegionalDbContext(DbContextOptions<RegionalDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
    }
}
