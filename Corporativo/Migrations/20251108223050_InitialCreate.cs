using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corporativo.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "REGIONES",
                columns: table => new
                {
                    id_region = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    api_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REGIONES", x => x.id_region);
                });

            migrationBuilder.CreateTable(
                name: "REPORTES_MENSUALES",
                columns: table => new
                {
                    id_reporte = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_region = table.Column<int>(type: "int", nullable: false),
                    año = table.Column<int>(type: "int", nullable: false),
                    mes = table.Column<int>(type: "int", nullable: false),
                    total_ventas = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    fecha_recepcion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REPORTES_MENSUALES", x => x.id_reporte);
                    table.ForeignKey(
                        name: "FK_REPORTES_MENSUALES_REGIONES_id_region",
                        column: x => x.id_region,
                        principalTable: "REGIONES",
                        principalColumn: "id_region",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DETALLE_REPORTE",
                columns: table => new
                {
                    id_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_reporte = table.Column<int>(type: "int", nullable: false),
                    id_sucursal = table.Column<int>(type: "int", nullable: false),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DETALLE_REPORTE", x => x.id_detalle);
                    table.ForeignKey(
                        name: "FK_DETALLE_REPORTE_REPORTES_MENSUALES_id_reporte",
                        column: x => x.id_reporte,
                        principalTable: "REPORTES_MENSUALES",
                        principalColumn: "id_reporte",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DETALLE_REPORTE_id_reporte",
                table: "DETALLE_REPORTE",
                column: "id_reporte");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesMensuales_Region_Año_Mes",
                table: "REPORTES_MENSUALES",
                columns: new[] { "id_region", "año", "mes" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DETALLE_REPORTE");

            migrationBuilder.DropTable(
                name: "REPORTES_MENSUALES");

            migrationBuilder.DropTable(
                name: "REGIONES");
        }
    }
}
