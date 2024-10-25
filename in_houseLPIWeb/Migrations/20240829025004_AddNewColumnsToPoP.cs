using Microsoft.EntityFrameworkCore.Migrations;

namespace in_houseLPIWeb.Migrations
{
    public partial class AddNewColumnsToPoP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop and recreate the RFP_No column with the IDENTITY property
            migrationBuilder.DropColumn(
                name: "RFP_No",
                table: "rfpForms");

            // Add new columns to the PoP table
            migrationBuilder.AddColumn<string>(
                name: "ApVoucher",
                table: "PoP",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CdjNumber",
                table: "PoP",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceInvoice",
                table: "PoP",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the new columns in the Down method
            migrationBuilder.DropColumn(
                name: "ApVoucher",
                table: "PoP");

            migrationBuilder.DropColumn(
                name: "CdjNumber",
                table: "PoP");

            migrationBuilder.DropColumn(
                name: "ServiceInvoice",
                table: "PoP");

            // Recreate the RFP_No column to its original state
            migrationBuilder.DropColumn(
                name: "RFP_No",
                table: "rfpForms");
        }
    }
}
