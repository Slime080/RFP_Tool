using Microsoft.EntityFrameworkCore.Migrations;

public partial class UpdateRFPNoColumn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Drop existing primary key if necessary
        migrationBuilder.DropPrimaryKey(name: "PK_rfpForms", table: "rfpForms");

        // Drop the existing column if it is not an identity column
        migrationBuilder.DropColumn(name: "RFP_No", table: "rfpForms");

        // Add new column with identity specification
        migrationBuilder.AddColumn<int>(
            name: "RFP_No",
            table: "rfpForms",
            type: "int",
            nullable: false,
            defaultValue: 0)
            .Annotation("SqlServer:Identity", "1, 1");

        // Add primary key constraint on the new identity column
        migrationBuilder.AddPrimaryKey(name: "PK_rfpForms", table: "rfpForms", column: "RFP_No");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Revert changes if necessary
        migrationBuilder.DropPrimaryKey(name: "PK_rfpForms", table: "rfpForms");
        migrationBuilder.DropColumn(name: "RFP_No", table: "rfpForms");
        migrationBuilder.AddColumn<int>(
            name: "RFP_No",
            table: "rfpForms",
            type: "int",
            nullable: true);

        migrationBuilder.AddPrimaryKey(name: "PK_rfpForms", table: "rfpForms", column: "RFP_No");
    }
}
