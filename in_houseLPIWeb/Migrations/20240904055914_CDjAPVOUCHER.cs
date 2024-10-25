using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace in_houseLPIWeb.Migrations
{
    public partial class CDjAPVOUCHER : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApVoucher",
                table: "PoP");

            migrationBuilder.DropColumn(
                name: "CdjNumber",
                table: "PoP");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "RFP_No",
                table: "rfpForms",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Ap_Voucher",
                table: "rfpForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Ap_Voucher_Posted_Date",
                table: "rfpForms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CDJ_Num_Posted_Date",
                table: "rfpForms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cdj_Number",
                table: "rfpForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "rfpForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "PoP",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Ap_Voucher",
                table: "rfpForms");

            migrationBuilder.DropColumn(
                name: "Ap_Voucher_Posted_Date",
                table: "rfpForms");

            migrationBuilder.DropColumn(
                name: "CDJ_Num_Posted_Date",
                table: "rfpForms");

            migrationBuilder.DropColumn(
                name: "Cdj_Number",
                table: "rfpForms");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "rfpForms");

            migrationBuilder.AlterColumn<int>(
                name: "RFP_No",
                table: "rfpForms",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "PoP",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

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
        }
    }
}
