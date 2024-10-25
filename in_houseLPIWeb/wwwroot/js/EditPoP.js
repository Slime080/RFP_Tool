document.addEventListener('DOMContentLoaded', function () {
    // Get modal elements
    var modal = document.getElementById('editModal');
    var closeBtn = document.querySelector('.modal .close');
    var cancelBtn = document.getElementById('cancelBtn');
    var submitBtn = document.getElementById('submitBtn');

    // Function to open the modal
    function openModal(data) {
        // Populate form fields with data
        document.getElementById('uID').value = data.uID || '';
        document.getElementById('popId').value = data.popId || '';
        document.getElementById('createdBy').value = data.createdBy || '';
        document.getElementById('createDate').value = data.createDate || '';
        document.getElementById('ToCq').value = data.ToC || '';
        document.getElementById('dueDate').value = data.DueDate || '';
        document.getElementById('csd').value = data.CoverStartDate || '';
        document.getElementById('ced').value = data.CoverEndDate || '';
        document.getElementById('bdate').value = data.BillingDate || '';
        document.getElementById('popEntity').value = data.Entity || '';
        document.getElementById('storeSelect').value = data.ChargeTo || '';
        document.getElementById('ORq').value = data.OR_Number || '';
        document.getElementById('SIq').value = data.SI_Number || '';
        document.getElementById('DRq').value = data.DR_Number || '';
        document.getElementById('POq').value = data.PO_Number || '';
        document.getElementById('popAmount').value = data.Amount || '';
        document.getElementById('currency').value = data.Currency || '';
        document.getElementById('vatPerc').value = data.VATPercent || '';
        document.getElementById('whtPerc').value = data.WHTPercent || '';
        document.getElementById('basicAmt').value = data.BasicAmount || '';
        document.getElementById('vatAmt').value = data.VATAmount || '';
        document.getElementById('whtAmt').value = data.WHTAmount || '';
        document.getElementById('totalAmt').value = data.TotalAmount || '';
        document.getElementById('remarks').value = data.AdditionalInfo || '';

        modal.style.display = 'block';
    }

    // Function to close the modal
    function closeModal() {
        modal.style.display = 'none';
    }

    // Event listener for close button
    closeBtn.addEventListener('click', closeModal);
    cancelBtn.addEventListener('click', closeModal);

    // Event listener for form submission
    submitBtn.addEventListener('click', function (event) {
        event.preventDefault();
        var formData = new FormData(document.getElementById('editForm'));
        // Send data to the server or handle it as needed
        console.log('Form data:', Array.from(formData.entries()));
        closeModal();
    });

    // Example of opening the modal with data
    document.querySelectorAll('.edit-btn').forEach(function (button) {
        button.addEventListener('click', function () {
            var data = {
                uID: button.dataset.uid,
                popId: button.dataset.popid,
                createdBy: button.dataset.createdby,
                createDate: button.dataset.createddate,
                ToC: button.dataset.toc,
                DueDate: button.dataset.duedate,
                CoverStartDate: button.dataset.csd,
                CoverEndDate: button.dataset.ced,
                BillingDate: button.dataset.bdate,
                Entity: button.dataset.entity,
                ChargeTo: button.dataset.chargeto,
                OR_Number: button.dataset.orq,
                SI_Number: button.dataset.siq,
                DR_Number: button.dataset.drq,
                PO_Number: button.dataset.poq,
                Amount: button.dataset.amount,
                Currency: button.dataset.currency,
                VATPercent: button.dataset.vatperc,
                WHTPercent: button.dataset.whtperc,
                BasicAmount: button.dataset.basicamt,
                VATAmount: button.dataset.vatamt,
                WHTAmount: button.dataset.whtamt,
                TotalAmount: button.dataset.totalamt,
                AdditionalInfo: button.dataset.remarks
            };
            openModal(data);
        });
    });
});
