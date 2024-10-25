using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace in_houseLPIWeb.Migrations
{
    public partial class UpdatePoP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RFP_No",
                table: "rfpForms",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Ap_Voucher",
                table: "PoP",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Ap_Voucher_Posted_Date",
                table: "PoP",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CDJ_Num_Posted_Date",
                table: "PoP",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cdj_Number",
                table: "PoP",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ap_Voucher",
                table: "PoP");

            migrationBuilder.DropColumn(
                name: "Ap_Voucher_Posted_Date",
                table: "PoP");

            migrationBuilder.DropColumn(
                name: "CDJ_Num_Posted_Date",
                table: "PoP");

            migrationBuilder.DropColumn(
                name: "Cdj_Number",
                table: "PoP");

            migrationBuilder.AlterColumn<int>(
                name: "RFP_No",
                table: "rfpForms",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
