using Microsoft.EntityFrameworkCore.Migrations;

namespace in_houseLPIWeb.Migrations
{
    public partial class AddCDJExtraction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CDJ_Extraction",
                columns: table => new
                {
                    CDJ_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RFP_No = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Voucher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSettleVoucher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Closed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TOCNAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PoPCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CDJ_Extraction", x => x.CDJ_Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CDJ_Extraction");
        }
    }
}
