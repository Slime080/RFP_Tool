using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace in_houseLPIWeb.Migrations
{
    public partial class addRFPFormTB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rfpForms",
                columns: table => new
                {
                    RFP_No = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Payee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "date", nullable: false),
                    PoPCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Checked = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Noted = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Approved = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rfpForms", x => x.RFP_No);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rfpForms");
        }
    }
}
