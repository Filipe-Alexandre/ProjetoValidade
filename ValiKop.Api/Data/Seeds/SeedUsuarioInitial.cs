using Microsoft.EntityFrameworkCore.Migrations;

namespace ValiKop.Api.Data.Seeds
{
    public static class SeedUsuarioInitial
    {
        public static void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("SET IDENTITY_INSERT Usuarios ON");

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[]
                {
            "Id",
            "Nome",
            "TipoUsuario",
            "Ativo",
            "Login",
            "PasswordHash",
            "PasswordSalt",
            "PasswordTemp"
                },
                values: new object[]
                {
            1,
            "Administrador",
            1, // Administrador
            true,
            "admin",
            "vkTQV3xsSJFph50msJBkZufKMfzpXbocCrgzfOJIrEY=",
            "USDcCO/OKIYeMvO5L88l5w==",
            true
                }
            );

            migrationBuilder.Sql("SET IDENTITY_INSERT Usuarios OFF");
        }


        public static void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1
            );
        }
    }
}
