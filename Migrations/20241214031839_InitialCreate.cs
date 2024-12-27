using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace EPARCIAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Proyectos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "longtext", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FlagActivo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    Estado = table.Column<string>(type: "longtext", nullable: true, defaultValue: "Pendiente")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proyectos", x => x.ID);
                    table.CheckConstraint("CHK_Fecha", "fecha_inicio <= fecha_fin");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tareas",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    FechaLimite = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, defaultValue: "Pendiente"),
                    Prioridad = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    FlagActivo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    ProyectoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tareas", x => x.ID);
                    table.CheckConstraint("CHK_Estado_Tarea", "Estado IN ('Pendiente', 'En Progreso', 'Completada')");
                    table.CheckConstraint("CHK_Prioridad_Tarea", "Prioridad IN ('Baja', 'Media', 'Alta')");
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Proyectos");

            migrationBuilder.DropTable(
                name: "Tareas");
        }
    }
}
