using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureFunctionRed.Migrations
{
    /// <inheritdoc />
    public partial class hejsar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "DeviceActions");

            migrationBuilder.AddColumn<bool>(
                name: "IsOn",
                table: "DeviceActions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOn",
                table: "DeviceActions");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "DeviceActions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
