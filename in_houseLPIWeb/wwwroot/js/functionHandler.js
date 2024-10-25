///*
//    =========================================== This function is for Purpose of Payment and Benefactor Form ============================================
// */

//document.addEventListener('DOMContentLoaded', function () {
//    const entitySelectInput = document.getElementById('popEntity');
//    const storeSelectInput = document.getElementById('storeSelect');
//    const chargeSelect = document.getElementById('ToCq');
//    const grossBox = document.getElementById('grossContainer');
//    const calcBox = document.getElementById('calcContainer');
//    const grossInput = document.getElementById('popAmount');
//    const chargeTo = storeSelectInput.value.toLowerCase();

//    const debounce = (func, wait) => {
//        let timeout;
//        return (...args) => {
//            clearTimeout(timeout);
//            timeout = setTimeout(() => func.apply(this, args), wait);
//        };
//    };

//    // ======================================================== Purpose of Payment Functions ============================================================
//    $(document).on('submit', '#popForm', function (e) {
//        e.preventDefault();
//        const entSelected = document.getElementById('popEntity');
//        const storeSelected = document.getElementById('storeSelect');
//        const chargeTo = storeSelected.value.toLowerCase();
//        const ent = entSelected.value;

//        var formData = $(this).serialize();

//        if (!chargeTo.includes("sdwan")) {
//            $.ajax({
//                url: '/RFP/rfpAdd?handler=AddPOP',
//                type: 'POST',
//                data: formData,
//                success: function (result) {
//                    if (result.success) {
//                        // Update specific elements on the main page
//                        $('#dynamicContent').html(result.html);
//                        $('#secAmt').html(result.additionalHtml);
//                        $('#signContent').html(result.signatoriesHtml);

//                        showNotification('PoPnotificationContainer', 'Purpose of Payment is added.');

//                        clearPartialViewFields();

//                        if ($('#popModal').length) {
//                            // If popModal exists, hide it and its backdrop with animation
//                            $('#popModal').hide();
//                            $('.modal-backdrop').remove(); // Remove the backdrop if it exists
//                            $('body').removeClass('modal-open'); // Remove modal-open class
//                            $('body').css('padding-right', ''); // Reset padding-right if added by Bootstrap
//                            // Optionally, re-enable scrolling on the body
//                            $('body').css('overflow', 'auto');
//                        } else if ($('#editPoPModal').length) {
//                            // If editPoPModal exists, hide it and its backdrop with animation
//                            $('#editPoPModal').hide();
//                            $('.modal-backdrop').remove(); // Remove the backdrop if it exists
//                            $('body').removeClass('modal-open'); // Remove modal-open class
//                            $('body').css('padding-right', ''); // Reset padding-right if added by Bootstrap
//                            // Optionally, re-enable scrolling on the body
//                            $('body').css('overflow', 'auto');
//                        }
//                        // Reattach event listener to the button after closing the modal
//                        $('#popModal, #editPoPModal').on('hidden.bs.modal', function () {
//                            $('button[data-target="#popModal"]').on('click', openModal); // Reattach event listener to the "Add" button
//                            $('button[data-target="#editPoPModal"]').on('click', openEditModal); // Reattach event listener to the "Edit" button
//                        });


//                        // Add more update logic as needed
//                    } else {
//                        // Handle errors if needed
//                        console.error('Error:', result.errors);
//                    }
//                },
//                error: function (error) {
//                    console.error('Error:', error);
//                }
//            });
//        }
//        else {
//            $.ajax({
//                url: '/RFP/rfpAdd?handler=AddPOP',
//                type: 'POST',
//                data: formData,
//                success: function (result) {
//                    if (result.success) {
//                        // Update specific elements on the main page
//                        $('#dynamicContent').html(result.html);
//                        $('#secAmt').html(result.additionalHtml);
//                        $('#signContent').html(result.signatoriesHtml);

//                        if (ent != "" || ent != null) {
//                            showNotification('PoPnotificationContainer', 'Stores with SD-WAN from Entity ' + ent + ' are added.');
//                        } else {
//                            showNotification('PoPnotificationContainer', 'Stores with SD-WAN are added.');
//                        }

//                        clearPartialViewFields();

//                        if ($('#popModal').length) {
//                            // If popModal exists, hide it and its backdrop with animation
//                            $('#popModal').hide();
//                            $('.modal-backdrop').remove(); // Remove the backdrop if it exists
//                            $('body').removeClass('modal-open'); // Remove modal-open class
//                            $('body').css('padding-right', ''); // Reset padding-right if added by Bootstrap
//                            $('body').css('overflow', 'auto'); // Optionally, re-enable scrolling on the body
//                        } else if ($('#editPoPModal').length) {
//                            // If editPoPModal exists, hide it and its backdrop with animation
//                            $('#editPoPModal').hide();
//                            $('.modal-backdrop').remove(); // Remove the backdrop if it exists
//                            $('body').removeClass('modal-open'); // Remove modal-open class
//                            $('body').css('padding-right', ''); // Reset padding-right if added by Bootstrap
//                            $('body').css('overflow', 'auto'); // Optionally, re-enable scrolling on the body
//                        }
//                        // Reattach event listener to the button after closing the modal
//                        $('#popModal, #editPoPModal').on('hidden.bs.modal', function () {
//                            $('button[data-target="#popModal"]').on('click', openModal); // Reattach event listener to the "Add" button
//                            $('button[data-target="#editPoPModal"]').on('click', openEditModal); // Reattach event listener to the "Edit" button
//                        });


//                        // Add more update logic as needed
//                    } else {
//                        // Handle errors if needed
//                        console.error('Error:', result.errors);
//                    }
//                },
//                error: function (error) {
//                    console.error('Error:', error);
//                }
//            });

//        }

//    });

//    // ==================================== Event Listener for Purpose of Payment ================================

//    storeSelectInput.addEventListener('change', function () {
//        debounce(hideGrossInput, 250)();
//    });
//    entitySelectInput.addEventListener('change', function () {
//        debounce(hideGrossInput, 250)();
//    });
//    chargeSelect.addEventListener('change', function () {
//        debounce(hideGrossInput, 250)();
//    });
//    if (chargeTo.includes("sdwan")) {
//        hideGrossInput();
//    }


//    function hideGrossInput() {
//        var storeSelect = document.getElementById('storeSelect');
//        var input = storeSelect.value.toLowerCase();

//        if (input.includes("sdwan") || input.includes("sd-wan")) {
//            grossBox.style.display = 'none';
//            calcBox.style.display = 'none';
//            grossInput.value = '0';
//        }
//        else {
//            grossBox.style.display = 'block';
//            calcBox.style.display = 'block';
//        }
//    }

//});

//function clearPartialViewFields() {
//    var toCqSelect = document.getElementById('ToCq');
//    var popEntitySelect = document.getElementById('popEntity');
//    var storeSelect = document.getElementById('storeSelect');
//    var ifcContainer = document.getElementById('ifcBox');
//    var lpiContainer = document.getElementById('lpiBox');
//    var ticketContainer = document.getElementById('ticket');
//    toCqSelect.value = '';
//    popEntitySelect.value = '';
//    storeSelect.value = '';
//    ifcContainer.value = '';
//    lpiContainer.value = '';


//    document.getElementById('dueDate').value = null;
//    document.getElementById('csd').value = null;
//    document.getElementById('ced').value = null;

//    document.getElementById('ORq').value = '';
//    document.getElementById('SIq').value = '';
//    document.getElementById('DRq').value = '';
//    document.getElementById('POq').value = '';

//    document.getElementById('basicAmt').value = '0.00';
//    document.getElementById('vatAmt').value = '0.00';
//    document.getElementById('whtAmt').value = '0.00';
//    document.getElementById('netAmt').value = '0.00';

//    document.getElementById('popAmount').value = '';

//    ifcContainer.style.display = 'none';
//    lpiContainer.style.display = 'none';
//    ticketContainer.style.display = 'none';

//    // Corrected checkbox handling
//    document.getElementById('ticketCheck').checked = false;
//    document.getElementById('vatPerc').value = 0;
//    document.getElementById('whtPerc').value = 0;

//    document.getElementById('Descq').value = '';
//}

//function showNotification(container, message) {
//    var notificationContainer = document.getElementById(container);
//    var notificationMessage = document.getElementById('notificationMessage');

//    // Set the message content
//    notificationMessage.textContent = message;

//    // Show the container
//    notificationContainer.style.display = 'block';

//    // Trigger a reflow to enable the fade-in transition
//    void notificationContainer.offsetWidth;

//    // Fade in
//    notificationContainer.style.opacity = '1';

//    // Sets the notification time span
//    setTimeout(function () {
//        // Fade out
//        notificationContainer.style.opacity = '0';

//        // Hide the container after fade out
//        setTimeout(function () {
//            notificationContainer.style.display = 'none';
//        }, 500);
//    }, 3000);
//}
//function resetDropdowns() {
//    const popEntityDropdown = document.getElementById('popEntity');
//    const ToCqDropdown = document.getElementById('ToCq');
//    const storeSelectDropdown = document.getElementById('storeSelect');

//    if (popEntityDropdown && Choices.instancesById[popEntityDropdown.id]) {
//        Choices.instancesById[popEntityDropdown.id].clearStore(); // Clear choices
//        Choices.instancesById[popEntityDropdown.id].setChoiceByValue(''); // Reset to placeholder
//    }

//    if (ToCqDropdown && Choices.instancesById[ToCqDropdown.id]) {
//        Choices.instancesById[ToCqDropdown.id].clearStore(); // Clear choices
//        Choices.instancesById[ToCqDropdown.id].setChoiceByValue(''); // Reset to placeholder
//    }

//    if (storeSelectDropdown && Choices.instancesById[storeSelectDropdown.id]) {
//        Choices.instancesById[storeSelectDropdown.id].clearStore(); // Clear choices
//        Choices.instancesById[storeSelectDropdown.id].setChoiceByValue(''); // Reset to placeholder
//    }
//}

//function openModal() {
//    $('#popModal').modal('show'); // Open the modal
//    $('#editPoPModal').modal('show'); // Open the modal

//    // Reset the dropdowns when the modal opens
//    resetDropdowns();
//}



//// =================================== Function section for Purpose of Payment Form ===================================






/*
    =========================================== This function is for Purpose of Payment and Benefactor Form ============================================
*/

document.addEventListener('DOMContentLoaded', function () {
    const entitySelectInput = document.getElementById('popEntity');
    const storeSelectInput = document.getElementById('storeSelect');
    const chargeSelect = document.getElementById('ToCq');
    const grossBox = document.getElementById('grossContainer');
    const calcBox = document.getElementById('calcContainer');
    const grossInput = document.getElementById('popAmount');
    const chargeTo = storeSelectInput.value.toLowerCase();

    const debounce = (func, wait) => {
        let timeout;
        return (...args) => {
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(this, args), wait);
        };
    };

    // ======================================================== Purpose of Payment Functions ============================================================
    $(document).on('submit', '#popForm', function (e) {
        e.preventDefault();
        const entSelected = document.getElementById('popEntity');
        const storeSelected = document.getElementById('storeSelect');
        const chargeTo = storeSelected.value.toLowerCase();
        const ent = entSelected.value;

        var formData = $(this).serialize();

        const hideModal = (modalId) => {
            $(modalId).modal('hide');
            $('.modal-backdrop').remove(); // Remove backdrop
            $('body').removeClass('modal-open'); // Remove modal-open class
            $('body').css('padding-right', ''); // Reset padding-right if added by Bootstrap
            $('body').css('overflow', 'auto'); // Re-enable body scrolling
        };

        if (!chargeTo.includes("sdwan")) {
            $.ajax({
                url: '/RFP/rfpAdd?handler=AddPOP',
                type: 'POST',
                data: formData,
                success: function (result) {
                    if (result.success) {
                        $('#dynamicContent').html(result.html);
                        $('#secAmt').html(result.additionalHtml);
                        $('#signContent').html(result.signatoriesHtml);

                        showNotification('PoPnotificationContainer', 'Purpose of Payment is added.');
                        clearPartialViewFields();
                      
                        hideModal('#popModal');
                        hideModal('#editPoPModal');
                    } else {
                        console.error('Error:', result.errors);
                    }
                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });
        } else {
            $.ajax({
                url: '/RFP/rfpAdd?handler=AddPOP',
                type: 'POST',
                data: formData,
                success: function (result) {
                    if (result.success) {
                        $('#dynamicContent').html(result.html);
                        $('#secAmt').html(result.additionalHtml);
                        $('#signContent').html(result.signatoriesHtml);
                   
                        showNotification('PoPnotificationContainer', 'Stores with SD-WAN are added.');
                        clearPartialViewFields();
                    
                        hideModal('#popModal');
                        hideModal('#editPoPModal');
                    } else {
                        console.error('Error:', result.errors);
                    }
                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });
        }
    });

    // ==================================== Event Listener for Purpose of Payment ================================

    storeSelectInput.addEventListener('change', function () {
        debounce(hideGrossInput, 250)();
    });
    entitySelectInput.addEventListener('change', function () {
        debounce(hideGrossInput, 250)();
    });
    chargeSelect.addEventListener('change', function () {
        debounce(hideGrossInput, 250)();
    });

    if (chargeTo.includes("sdwan")) {
        hideGrossInput();
    }

    function hideGrossInput() {
        var storeSelect = document.getElementById('storeSelect');
        var input = storeSelect.value.toLowerCase();

        if (input.includes("sdwan") || input.includes("sd-wan")) {
            grossBox.style.display = 'none';
            calcBox.style.display = 'none';
            grossInput.value = '0';
        } else {
            grossBox.style.display = 'block';
            calcBox.style.display = 'block';
        }
    }
});

// Reset fields in modal
function clearPartialViewFields() {
    const resetValue = (id) => document.getElementById(id).value = '';
    const resetDisplay = (id, display = 'none') => document.getElementById(id).style.display = display;

    resetValue('popEntity');
    resetValue('storeSelect');
    resetValue('ToCq');
    resetDisplay('ifcBox');
    resetDisplay('lpiBox');
    resetDisplay('ticket');
    resetValue('popAmount');

    // Reset other form fields and containers
    resetValue('dueDate');
    resetValue('csd');
    resetValue('ced');
    resetValue('ORq');
    resetValue('SIq');
    resetValue('DRq');
    resetValue('POq');
    document.getElementById('basicAmt').value = '0.00';
    document.getElementById('vatAmt').value = '0.00';
    document.getElementById('whtAmt').value = '0.00';
    document.getElementById('netAmt').value = '0.00';

    // Checkbox and percentage reset
    document.getElementById('ticketCheck').checked = false;
    document.getElementById('vatPerc').value = 0;
    document.getElementById('whtPerc').value = 0;

    document.getElementById('Descq').value = '';
}

function showNotification(container, message) {
    var notificationContainer = document.getElementById(container);
    var notificationMessage = document.getElementById('notificationMessage');

    notificationMessage.textContent = message;
    notificationContainer.style.display = 'block';
    void notificationContainer.offsetWidth; // Trigger reflow
    notificationContainer.style.opacity = '1';

    setTimeout(function () {
        notificationContainer.style.opacity = '0';
        setTimeout(function () {
            notificationContainer.style.display = 'none';
        }, 500);
    }, 3000);
}

function resetDropdowns() {
    //if (Choices.instancesById['ToCq']) {
    //    Choices.instancesById['ToCq'].clearStore();
    //    Choices.instancesById['ToCq'].setChoiceByValue('');
    //}
    const popEnt = document.getElementById('popEntity')
    console.log(popEnt.value);

    //if (Choices.instancesById['storeSelect']) {
    //    Choices.instancesById['storeSelect'].clearStore();
    //    Choices.instancesById['storeSelect'].setChoiceByValue('');
    //}
}


function openModal() {
    $('#popModal').modal('show');
}

function openEditModal() {
    $('#editPoPModal').modal('show');
}
