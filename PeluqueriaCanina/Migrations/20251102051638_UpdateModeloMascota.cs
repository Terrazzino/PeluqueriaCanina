using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeluqueriaCanina.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModeloMascota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Turnos_MascotaId",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "TurnoId",
                table: "Mascotas");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_MascotaId",
                table: "Turnos",
                column: "MascotaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Turnos_MascotaId",
                table: "Turnos");

            migrationBuilder.AddColumn<int>(
                name: "TurnoId",
                table: "Mascotas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_MascotaId",
                table: "Turnos",
                column: "MascotaId",
                unique: true);
        }
    }
}
