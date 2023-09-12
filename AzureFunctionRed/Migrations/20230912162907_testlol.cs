using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureFunctionRed.Migrations
{
    /// <inheritdoc />
    public partial class testlol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceConfigs");

            migrationBuilder.CreateTable(
                name: "ParseDatas",
                columns: table => new
                {
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParseDatas");

            migrationBuilder.CreateTable(
                name: "DeviceConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HostName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SharedAccessKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceConfigs", x => x.Id);
                });
        }
    }
}
