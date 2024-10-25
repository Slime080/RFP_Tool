using Microsoft.EntityFrameworkCore.Migrations;

namespace in_houseLPIWeb.Migrations
{
    public partial class updatePayeeToCTB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TOCDepartment",
                table: "TOCs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayeeDepartment",
                table: "Payees",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TOCDepartment",
                table: "TOCs");

            migrationBuilder.DropColumn(
                name: "PayeeDepartment",
                table: "Payees");
        }
    }
}
