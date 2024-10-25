document.addEventListener('DOMContentLoaded', function () {
    var checkboxes = document.querySelectorAll('input[type="checkbox"]');
    var tableHeaders = document.querySelectorAll('.table th, .second-table th');
    var tableCells = document.querySelectorAll('.table td, .second-table td');
    var printContainers = document.querySelectorAll('.print-container');
    var QckPrintBtn = document.getElementById('QckPrintBtn')
    var PrintBtn = document.getElementById('PrintBtn');
    //var ExportBtn = document.getElementById('ExportBtn');
    //var RFPNumber = document.getElementById('RFPNo');
    //var RFPname = RFPNumber.value;
    //console.log('PrintBtn:', PrintBtn);

    // To call the print function in buttons
    PrintBtn.addEventListener('click', printDocument);
    QckPrintBtn.addEventListener('click', printDocument);

    // To reset toggle when the modal is closed
    $('#printChoiceModal').on('hidden.bs.modal', function () {
        // Reset toggles here
        resetToggles();
    });

    checkboxes.forEach(function (checkbox) {
        checkbox.addEventListener('change', function () {
            updateVisibility();
        });
    });

    // Add an event listener for beforeprint
    window.addEventListener('beforeprint', function () {
        // Select and hide the modals
        var modals = document.querySelectorAll('.modal');
        modals.forEach(function (modal) {
            modal.style.display = 'none';
        });
        // Hide modal backdrops during printing
        var backdrops = document.querySelectorAll('.modal-backdrop');
        backdrops.forEach(function (backdrop) {
            backdrop.style.display = 'none';
        });

        // Remove containers not required before printing 
        document.getElementById('footX').style.display = 'none';
        document.getElementById('no-print').style.display = 'none';
    });

    // Add an event listener for afterprint
    window.addEventListener('afterprint', function () {
        setTimeout(function () {
            // Close the modal using jQuery UI method
            $('#printChoiceModal').modal('hide');

            //// Show the modals again after printing
            //var modals = document.querySelectorAll('.modal');
            //modals.forEach(function (modal) {
            //    modal.style.display = 'block'; // You might need to adjust this based on your modal's original display property
            //});
            //// Show modal backdrops again after printing
            //var backdrops = document.querySelectorAll('.modal-backdrop');
            //backdrops.forEach(function (backdrop) {
            //    backdrop.style.display = 'block'; // You might need to adjust this based on your backdrop's original display property
            //});

            // Show containers not required after printing
            document.getElementById('footX').style.display = 'block';
            document.getElementById('no-print').style.display = 'block';
        }, 100);
    });

    // Function to trigger the print dialog
    function printDocument() {

        // Trigger the browser's print functionality
        window.print();
    }


    function updateVisibility() {
        checkboxes.forEach(function (checkbox) {
            var isChecked = checkbox.checked;
            var classSuffix = '-column';
            var columnClass = checkbox.id + classSuffix;

            tableHeaders.forEach(function (header) {
                if (header.classList.contains(columnClass)) {
                    header.style.display = isChecked ? 'table-cell' : 'none';
                }
            });

            tableCells.forEach(function (cell) {
                if (cell.classList.contains(columnClass)) {
                    cell.style.display = isChecked ? 'table-cell' : 'none';
                }
            });
        });

        // Toggle the visibility for print containers
        printContainers.forEach(function (container) {
            container.style.display = 'block'; // Adjust as needed
        });
    }

    
    // Function to set the page number in printing
    function setPageCount() {
        var pageCountElement = document.getElementById('pageCount');
        var endOfPageRows = document.querySelectorAll('.page-num');
        var totalPages = endOfPageRows.length; // Add 1 for the first page

        pageCountElement.innerHTML = 'Page 1 of ' + totalPages;
    }

    setPageCount();
    updateVisibility();

    function resetToggles() {
        var checkboxes = document.querySelectorAll('.switch input[type="checkbox"]');
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = false;
            if (checkbox.id === "Net") {
                checkbox.checked = true;
            }
        });

        updateVisibility();
    }
});
