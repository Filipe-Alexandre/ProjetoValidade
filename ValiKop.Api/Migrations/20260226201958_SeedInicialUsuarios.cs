using Microsoft.EntityFrameworkCore.Migrations;
using ValiKop.Api.Data.Seeds;

#nullable disable

namespace ValiKop.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedInicialUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SeedUsuarioInitial.Up(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            SeedUsuarioInitial.Down(migrationBuilder);
        }
    }
}
