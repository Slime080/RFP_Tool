using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace in_houseLPIWeb.Migrations
{
    public partial class addPoPFormTB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PoP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PoP_Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChargeTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "date", nullable: true),
                    CoverStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    CoverEndDate = table.Column<DateTime>(type: "date", nullable: false),
                    ToC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OR_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SI_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DR_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PO_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    VATPercent = table.Column<int>(type: "int", nullable: false),
                    WHTPercent = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoP", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoP");
        }
    }
}
