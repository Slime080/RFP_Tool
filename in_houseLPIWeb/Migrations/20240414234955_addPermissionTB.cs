using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace in_houseLPIWeb.Migrations
{
    public partial class addPermissionTB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Index = table.Column<bool>(type: "bit", nullable: false),
                    Archive = table.Column<bool>(type: "bit", nullable: false),
                    IFC_Index = table.Column<bool>(type: "bit", nullable: false),
                    RFP_Dash = table.Column<bool>(type: "bit", nullable: false),
                    RFP_Index = table.Column<bool>(type: "bit", nullable: false),
                    RFP_Add = table.Column<bool>(type: "bit", nullable: false),
                    RFP_Edit = table.Column<bool>(type: "bit", nullable: false),
                    RFP_Archive = table.Column<bool>(type: "bit", nullable: false),
                    RFP_Activate = table.Column<bool>(type: "bit", nullable: false),
                    RFP_PoP_Edit = table.Column<bool>(type: "bit", nullable: false),
                    RFP_PoP_Delete = table.Column<bool>(type: "bit", nullable: false),
                    RFP_Print = table.Column<bool>(type: "bit", nullable: false),
                    RFP_View = table.Column<bool>(type: "bit", nullable: false),
                    RFPutil_Dash = table.Column<bool>(type: "bit", nullable: false),
                    RFPutil_Payee_Index = table.Column<bool>(type: "bit", nullable: false),
                    RFPutil_Payee_Edit = table.Column<bool>(type: "bit", nullable: false),
                    RFPutil_TOC_Index = table.Column<bool>(type: "bit", nullable: false),
                    RFPutil_TOC_Edit = table.Column<bool>(type: "bit", nullable: false),
                    Util_Dash = table.Column<bool>(type: "bit", nullable: false),
                    Util_Permission = table.Column<bool>(type: "bit", nullable: false),
                    Util_Store_Index = table.Column<bool>(type: "bit", nullable: false),
                    Util_Store_Edit = table.Column<bool>(type: "bit", nullable: false),
                    Util_Store_Archive = table.Column<bool>(type: "bit", nullable: false),
                    Util_Store_Actv = table.Column<bool>(type: "bit", nullable: false),
                    Util_StoreEnt_Index = table.Column<bool>(type: "bit", nullable: false),
                    Util_StoreEnt_Edit = table.Column<bool>(type: "bit", nullable: false),
                    Util_StoreType_Index = table.Column<bool>(type: "bit", nullable: false),
                    Util_StoreType_Edit = table.Column<bool>(type: "bit", nullable: false),
                    Util_User_Index = table.Column<bool>(type: "bit", nullable: false),
                    Util_User_Edit = table.Column<bool>(type: "bit", nullable: false),
                    Util_UserDep_Index = table.Column<bool>(type: "bit", nullable: false),
                    Util_UserDep_Edit = table.Column<bool>(type: "bit", nullable: false),
                    Util_UserDep_Actv = table.Column<bool>(type: "bit", nullable: false),
                    Util_UserDep_Archive = table.Column<bool>(type: "bit", nullable: false),
                    AddedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissions");
        }
    }
}
