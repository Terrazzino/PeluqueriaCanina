using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeluqueriaCanina.Migrations
{
    /// <inheritdoc />
    public partial class addAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdministradorId",
                table: "Auditorias");

            migrationBuilder.RenameColumn(
                name: "UsuarioModificadoNombre",
                table: "Auditorias",
                newName: "RolUsuario");

            migrationBuilder.RenameColumn(
                name: "UsuarioModificadoId",
                table: "Auditorias",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "Auditorias",
                newName: "FechaHora");

            migrationBuilder.RenameColumn(
                name: "Detalle",
                table: "Auditorias",
                newName: "NombreUsuario");

            migrationBuilder.RenameColumn(
                name: "AdministradorNombre",
                table: "Auditorias",
                newName: "Detalles");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_UsuarioId",
                table: "Auditorias",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auditorias_Usuario_UsuarioId",
                table: "Auditorias",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auditorias_Usuario_UsuarioId",
                table: "Auditorias");

            migrationBuilder.DropIndex(
                name: "IX_Auditorias_UsuarioId",
                table: "Auditorias");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Auditorias",
                newName: "UsuarioModificadoId");

            migrationBuilder.RenameColumn(
                name: "RolUsuario",
                table: "Auditorias",
                newName: "UsuarioModificadoNombre");

            migrationBuilder.RenameColumn(
                name: "NombreUsuario",
                table: "Auditorias",
                newName: "Detalle");

            migrationBuilder.RenameColumn(
                name: "FechaHora",
                table: "Auditorias",
                newName: "Fecha");

            migrationBuilder.RenameColumn(
                name: "Detalles",
                table: "Auditorias",
                newName: "AdministradorNombre");

            migrationBuilder.AddColumn<int>(
                name: "AdministradorId",
                table: "Auditorias",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
