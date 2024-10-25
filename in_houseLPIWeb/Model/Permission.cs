using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace in_houseLPIWeb.Model
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public bool Index { get; set; }                 // Homepage
        public bool Archive { get; set; }               // Archive Page
        public bool IFC_Index { get; set; }             // IFC Summary
        // RFP Pages
        public bool RFP_Dash { get; set; }
        public bool RFP_Index { get; set; }
        public bool RFP_Add { get; set; }
        public bool RFP_Edit { get; set; }
        public bool RFP_Archive { get; set; }
        public bool RFP_Activate { get; set; }
        public bool RFP_PoP_Edit { get; set; }
        public bool RFP_PoP_Delete { get; set; }
        public bool RFP_Print { get; set; }
        public bool RFP_View { get; set; }
        // Utility Page for RFP
        public bool RFPutil_Dash { get; set; }
        public bool RFPutil_Payee_Index { get; set; }
        public bool RFPutil_Payee_Edit { get; set; }
        public bool RFPutil_TOC_Index { get; set; }
        public bool RFPutil_TOC_Edit { get; set; }
        // Utility Page for User and Store details
        public bool Util_Dash { get; set; }
        public bool Util_Permission { get; set; }       // Updating User Page Permission
        public bool Util_Store_Index { get; set; }      // Page for List and adding new stores
        public bool Util_Store_Edit { get; set; }       // Page for Editing details
        public bool Util_Store_Archive { get; set; }    // Page for closing a store
        public bool Util_Store_Actv { get; set; }       // Page for activating or opening
        public bool Util_StoreEnt_Index { get; set; }   // Page for List of Store Entity and adding new
        public bool Util_StoreEnt_Edit { get; set; }    // Page for updating existing store entity
        public bool Util_StoreType_Index { get; set; }  // Page for List of Store Type and adding new
        public bool Util_StoreType_Edit { get; set; }   // Page for updating existing store type
        public bool Util_User_Index { get; set; }       // Page for List of Users
        public bool Util_User_Edit { get; set; }        // Page for updating user details
        public bool Util_UserDep_Index { get; set; }    // Page for List of departments
        public bool Util_UserDep_Edit { get; set; }     // Page for updating department details
        public bool Util_UserDep_Actv { get; set; }     // Page for activating department details
        public bool Util_UserDep_Archive { get; set; }  // Page for deactivating department details
        
        public string AddedBy { get; set; }           // To determine the user who add the permission
        public DateTime AddedDate { get; set; }       // To determine when the user added the permission
        public string ModifiedBy { get; set; }          // To determine the user who modified the permission
        public DateTime? ModifiedDate { get; set; }      // To determine when the user modified the permission

        // Add additional pages necessary
    }
}
