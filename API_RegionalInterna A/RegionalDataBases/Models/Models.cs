using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegionalDataBases.Models
{
    [Table("SUCURSALES")]
    public class Sucursal
    {
        [Key]
        [Column("id_sucursal")]
        public int IdSucursal { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [Column("direccion")]
        public string Direccion { get; set; }

        [Required]
        [Column("telefono")]
        [StringLength(10)]
        public string Telefono { get; set; }

        [Required]
        [Column("activa")]
        public bool Activa { get; set; }

        // Navegación
        public ICollection<Inventario> Inventarios { get; set; }
        public ICollection<ReporteVenta> ReportesVenta { get; set; }
        public ICollection<Cliente> Clientes { get; set; }
    }

    [Table("CATEGORIAS")]
    public class Categoria
    {
        [Key]
        [Column("id_categoria")]
        public int IdCategoria { get; set; }

        [Required]
        [Column("categoria")]
        [StringLength(100)]
        public string NombreCategoria { get; set; }

        [Required]
        [Column("activo")]
        public bool Activo { get; set; }

        // Navegación
        public ICollection<Producto> Productos { get; set; }
    }

    [Table("PRODUCTOS")]
    public class Producto
    {
        [Key]
        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [Column("id_categoria")]
        public int IdCategoria { get; set; }

        [Required]
        [Column("precio", TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Required]
        [Column("activo")]
        public bool Activo { get; set; }

        // Navegación
        [ForeignKey("IdCategoria")]
        public Categoria Categoria { get; set; }
        public ICollection<Inventario> Inventarios { get; set; }
        public ICollection<DetalleVenta> DetallesVenta { get; set; }
    }

    [Table("INVENTARIO")]
    public class Inventario
    {
        [Key]
        [Column("id_inventario")]
        public int IdInventario { get; set; }

        [Required]
        [Column("id_sucursal")]
        public int IdSucursal { get; set; }

        [Required]
        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Required]
        [Column("existencia")]
        public int Existencia { get; set; }

        [Required]
        [Column("ultima_actualizacion")]
        public DateTime UltimaActualizacion { get; set; }

        // Navegación
        [ForeignKey("IdSucursal")]
        public Sucursal Sucursal { get; set; }

        [ForeignKey("IdProducto")]
        public Producto Producto { get; set; }
    }

    [Table("CLIENTES")]
    public class Cliente
    {
        [Key]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(200)]
        public string Nombre { get; set; }

        [Required]
        [Column("telefono")]
        [StringLength(10)]
        public string Telefono { get; set; }

        [Required]
        [Column("direccion")]
        [StringLength(100)]
        public string Direccion { get; set; }

        [Required]
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [Column("id_sucursal")]
        public int IdSucursal { get; set; }

        [Required]
        [Column("activo")]
        public bool Activo { get; set; }

        // Navegación
        [ForeignKey("IdSucursal")]
        public Sucursal Sucursal { get; set; }
        public ICollection<ReporteVenta> ReportesVenta { get; set; }
    }

    [Table("REPORTE_VENTA")]
    public class ReporteVenta
    {
        [Key]
        [Column("id_reporte")]
        public int IdReporte { get; set; }

        [Required]
        [Column("id_sucursal")]
        public int IdSucursal { get; set; }

        [Required]
        [Column("total_ventas", TypeName = "decimal(10,2)")]
        public decimal TotalVentas { get; set; }

        [Required]
        [Column("fecha_recepcion")]
        public DateTime FechaRecepcion { get; set; }

        [Required]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        // Navegación
        [ForeignKey("IdSucursal")]
        public Sucursal Sucursal { get; set; }

        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }
        public ICollection<DetalleVenta> DetallesVenta { get; set; }
    }

    [Table("DETALLE_VENTAS")]
    public class DetalleVenta
    {
        [Key]
        [Column("id_detalle")]
        public int IdDetalle { get; set; }

        [Required]
        [Column("id_reporte")]
        public int IdReporte { get; set; }

        [Required]
        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Required]
        [Column("precio_unitario", TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Required]
        [Column("precio_venta", TypeName = "decimal(10,2)")]
        public decimal PrecioVenta { get; set; }

        // Navegación
        [ForeignKey("IdReporte")]
        public ReporteVenta ReporteVenta { get; set; }

        [ForeignKey("IdProducto")]
        public Producto Producto { get; set; }
    }
}