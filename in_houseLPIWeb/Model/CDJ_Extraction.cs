using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CDJ_Extraction
{
    [Key] // Specifies CDJ_Id as the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Specifies that CDJ_Id is an auto-incrementing identity column
    public int CDJ_Id { get; set; }

    public string RFP_No { get; set; }
    public string Voucher { get; set; }
    public string LastSettleVoucher { get; set; }
    public string ApprovedDate { get; set; }
    public string Closed { get; set; }
    public string StoreName { get; set; }
    public string TOCNAME { get; set; }
    public string PoPCode { get; set; }
}
