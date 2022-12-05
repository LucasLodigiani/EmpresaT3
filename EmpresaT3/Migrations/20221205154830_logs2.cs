using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpresaT3.Migrations
{
    public partial class logs2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Operacion",
                table: "Logs",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Operacion",
                table: "Logs");
        }
    }
}
