using Microsoft.EntityFrameworkCore.Migrations;

namespace in_houseLPIWeb.Migrations
{
    public partial class UpdateIdentityConfigurationFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing primary key constraint if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes 
                           WHERE object_id = OBJECT_ID('rfpForms') 
                             AND name = 'PK_rfpForms')
                BEGIN
                    ALTER TABLE rfpForms DROP CONSTRAINT PK_rfpForms;
                END");

            // Drop the existing RFP_No column if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns 
                           WHERE object_id = OBJECT_ID('rfpForms') 
                             AND name = 'RFP_No')
                BEGIN
                    ALTER TABLE rfpForms DROP COLUMN RFP_No;
                END");

            // Add the RFP_No column with IDENTITY property and make it the primary key
            migrationBuilder.Sql(@"
                ALTER TABLE rfpForms ADD RFP_No INT IDENTITY(1,1) PRIMARY KEY NOT NULL;");

            // Check if columns already exist before adding
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns 
                               WHERE object_id = OBJECT_ID('PoP') 
                                 AND name = 'ApVoucher')
                BEGIN
                    ALTER TABLE PoP ADD ApVoucher nvarchar(max) NULL;
                END");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns 
                               WHERE object_id = OBJECT_ID('PoP') 
                                 AND name = 'CdjNumber')
                BEGIN
                    ALTER TABLE PoP ADD CdjNumber nvarchar(max) NULL;
                END");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns 
                               WHERE object_id = OBJECT_ID('PoP') 
                                 AND name = 'ServiceInvoice')
                BEGIN
                    ALTER TABLE PoP ADD ServiceInvoice nvarchar(max) NULL;
                END");
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

            // Revert the RFP_No column to its original state
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns 
                           WHERE object_id = OBJECT_ID('rfpForms') 
                             AND name = 'RFP_No')
                BEGIN
                    ALTER TABLE rfpForms DROP COLUMN RFP_No;
                END
                ALTER TABLE rfpForms ADD RFP_No INT NOT NULL IDENTITY(1,1);");
        }
    }
}
