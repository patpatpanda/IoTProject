using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureFunctionRed.Migrations
{
    /// <inheritdoc />
    public partial class had : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ParseDatas",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParseDatas",
                table: "ParseDatas",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ParseDatas",
                table: "ParseDatas");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ParseDatas");
        }
    }
}
