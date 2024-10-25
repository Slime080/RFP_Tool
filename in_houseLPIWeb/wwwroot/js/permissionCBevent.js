document.addEventListener("DOMContentLoaded", function () {
    // INITIALIZATION FOR INPUTS
    const userLevel = document.getElementById("uLevel").value;
    const currentLevel = document.getElementById("currentLevel");

    // INITIALIZATION FOR CHECKBOXES
    const rfp_Dash = document.getElementById("rfp_dashCB");
    const rfp_Add = document.getElementById("rfp_addCB");
    const rfp_Archive = document.getElementById("rfp_archiveCB");
    const rfp_Print = document.getElementById("rfp_printCB");
    const rfp_View = document.getElementById("rfp_viewCB");

    const rfpUtil_Dash = document.getElementById("rfpUtil_dashCB");
    const rfpUtil_PayeeAdd = document.getElementById("payee_addCB");
    const rfpUtil_TOCAdd = document.getElementById("toc_addCB");


    const util_StoAdd = document.getElementById("util_stoAddCB");
    const util_StoClose = document.getElementById("util_stoCloseCB");
    const util_EntAdd = document.getElementById("util_entAddCB");
    const util_TypeAdd = document.getElementById("util_typeAddCB");
    const util_UserEdit = document.getElementById("util_userEditCB");
    const util_Permission = document.getElementById("util_permissionCB");
    const util_DepEdit = document.getElementById("util_depEditCB");

    const gen_Index = document.getElementById("indexCB");
    const gen_Archive = document.getElementById("archiveCB");
    const gen_IFCsummary = document.getElementById("ifc_indexCB");

    // Initially disable Edit and Delete checkboxes
    rfp_Dash.disabled = true;
    rfp_Archive.disabled = true;
    rfpUtil_Dash.disabled = true;
    util_StoClose.disabled = true;
    util_EntAdd.disabled = true;
    util_TypeAdd.disabled = true;
    gen_Index.disabled = true;
    gen_Index.checked = true;

    if (currentLevel.value === "0") {
        util_EntAdd.disabled = false;
        util_TypeAdd.disabled = false;
    }

    // Add event listener to Add checkbox
    rfp_Add.addEventListener("change", updateCheckBox);
    rfp_Archive.addEventListener("change", updateCheckBox);
    rfp_Print.addEventListener("change", updateCheckBox);
    rfp_View.addEventListener("change", updateCheckBox);
    rfpUtil_PayeeAdd.addEventListener("change", updateCheckBox);
    rfpUtil_TOCAdd.addEventListener("change", updateCheckBox);
    util_StoAdd.addEventListener("change", updateCheckBox);

    updateCheckBox();

    function updateCheckBox() {
        if (currentLevel.value !== "0") {
            gen_Archive.disabled = true;
            rfp_Archive.disabled = true;
            util_UserEdit.disabled = true;
            util_Permission.disabled = true;
            util_DepEdit.disabled = true;
        }
        // events for RFP Add button
        if (rfp_Add.checked) {
            rfp_Archive.disabled = false;
            rfp_View.checked = true;
            rfp_Print.checked = true;
        }
        else {
            rfp_Archive.disabled = true;
            rfp_Archive.checked = false;
        }

        if (rfp_Archive.checked) {
            gen_Archive.disabled = false;
            gen_Archive.checked = true;
        }
        else {
            gen_Archive.disabled = true;
            gen_Archive.checked = false;
        }

        // events for RFP buttons
        if (rfp_Add.checked && rfp_Archive.checked && rfp_Print.checked && rfp_View.checked) {
            rfp_Dash.checked = true;
        }
        else {
            rfp_Dash.checked = false;
        }

        // event for RFP Utility buttons
        if (rfpUtil_PayeeAdd.checked && rfpUtil_TOCAdd.checked) {
            rfpUtil_Dash.checked = true;
        }
        else {
            rfpUtil_Dash.checked = false;
        }

        // events for Store Add button
        if (util_StoAdd.checked) {
            // util_StoClose.checked = true;
            util_StoClose.disabled = false;
        }
        else {
            util_StoClose.checked = false;
            util_StoClose.disabled = true;
        }
    }
});