using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegionalDataBases.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CATEGORIAS",
                columns: table => new
                {
                    id_categoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CATEGORIAS", x => x.id_categoria);
                });

            migrationBuilder.CreateTable(
                name: "SUCURSALES",
                columns: table => new
                {
                    id_sucursal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUCURSALES", x => x.id_sucursal);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCTOS",
                columns: table => new
                {
                    id_producto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    id_categoria = table.Column<int>(type: "int", nullable: false),
                    precio = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCTOS", x => x.id_producto);
                    table.ForeignKey(
                        name: "FK_PRODUCTOS_CATEGORIAS_id_categoria",
                        column: x => x.id_categoria,
                        principalTable: "CATEGORIAS",
                        principalColumn: "id_categoria",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CLIENTES",
                columns: table => new
                {
                    id_cliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    id_sucursal = table.Column<int>(type: "int", nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLIENTES", x => x.id_cliente);
                    table.ForeignKey(
                        name: "FK_CLIENTES_SUCURSALES_id_sucursal",
                        column: x => x.id_sucursal,
                        principalTable: "SUCURSALES",
                        principalColumn: "id_sucursal",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "INVENTARIO",
                columns: table => new
                {
                    id_inventario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_sucursal = table.Column<int>(type: "int", nullable: false),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    existencia = table.Column<int>(type: "int", nullable: false),
                    ultima_actualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INVENTARIO", x => x.id_inventario);
                    table.ForeignKey(
                        name: "FK_INVENTARIO_PRODUCTOS_id_producto",
                        column: x => x.id_producto,
                        principalTable: "PRODUCTOS",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_INVENTARIO_SUCURSALES_id_sucursal",
                        column: x => x.id_sucursal,
                        principalTable: "SUCURSALES",
                        principalColumn: "id_sucursal",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "REPORTE_VENTA",
                columns: table => new
                {
                    id_reporte = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_sucursal = table.Column<int>(type: "int", nullable: false),
                    total_ventas = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    fecha_recepcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_cliente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REPORTE_VENTA", x => x.id_reporte);
                    table.ForeignKey(
                        name: "FK_REPORTE_VENTA_CLIENTES_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "CLIENTES",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_REPORTE_VENTA_SUCURSALES_id_sucursal",
                        column: x => x.id_sucursal,
                        principalTable: "SUCURSALES",
                        principalColumn: "id_sucursal",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DETALLE_VENTAS",
                columns: table => new
                {
                    id_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_reporte = table.Column<int>(type: "int", nullable: false),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    precio_venta = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DETALLE_VENTAS", x => x.id_detalle);
                    table.ForeignKey(
                        name: "FK_DETALLE_VENTAS_PRODUCTOS_id_producto",
                        column: x => x.id_producto,
                        principalTable: "PRODUCTOS",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DETALLE_VENTAS_REPORTE_VENTA_id_reporte",
                        column: x => x.id_reporte,
                        principalTable: "REPORTE_VENTA",
                        principalColumn: "id_reporte",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CLIENTES_id_sucursal",
                table: "CLIENTES",
                column: "id_sucursal");

            migrationBuilder.CreateIndex(
                name: "IX_DETALLE_VENTAS_id_producto",
                table: "DETALLE_VENTAS",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_DETALLE_VENTAS_id_reporte",
                table: "DETALLE_VENTAS",
                column: "id_reporte");

            migrationBuilder.CreateIndex(
                name: "IX_INVENTARIO_id_producto",
                table: "INVENTARIO",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_INVENTARIO_id_sucursal",
                table: "INVENTARIO",
                column: "id_sucursal");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTOS_id_categoria",
                table: "PRODUCTOS",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_REPORTE_VENTA_id_cliente",
                table: "REPORTE_VENTA",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_REPORTE_VENTA_id_sucursal",
                table: "REPORTE_VENTA",
                column: "id_sucursal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DETALLE_VENTAS");

            migrationBuilder.DropTable(
                name: "INVENTARIO");

            migrationBuilder.DropTable(
                name: "REPORTE_VENTA");

            migrationBuilder.DropTable(
                name: "PRODUCTOS");

            migrationBuilder.DropTable(
                name: "CLIENTES");

            migrationBuilder.DropTable(
                name: "CATEGORIAS");

            migrationBuilder.DropTable(
                name: "SUCURSALES");
        }
    }
}
