/*Edited by Kurt*/

/* Filter dropdown types of charges */


function validateForm() {
    let isValid = true;

    // Clear previous error messages
    const errorMessages = {
        'errormsg': '',
        'errorPopEntity': '',
        'errorChargeTo': '',
        'errorCoverStartDate': '',
        'errorCoverEndDate': '',
        'errorGrossAmount': '',
        'errorDueDate': ''
    };

    Object.keys(errorMessages).forEach(id => {
        document.getElementById(id).textContent = errorMessages[id];
    });

    // Type of Charges
    const toCq = document.getElementById('ToCq');
    if (toCq && toCq.value.trim() === '') {
        isValid = false;
        errorMessages['errormsg'] = 'Type of Charges is required.';
        scrollToElement('popModalLabel');
    }

    // Charge To
    const popEntity = document.getElementById('popEntity');
    const chargeTo = document.getElementById('storeSelect');
    if (popEntity && popEntity.value.trim() === '') {
        isValid = false;
        errorMessages['errorPopEntity'] = '(Entity) is required.';
        scrollToElement('popModalLabel');
    }
    if (chargeTo && chargeTo.value.trim() === '') {
        isValid = false;
        errorMessages['errorChargeTo'] = 'Charge To (Store) is required.';
        scrollToElement('popModalLabel');
    }

    // Cover Dates
    const coverStartDate = document.getElementById('csd');
    const coverEndDate = document.getElementById('ced');
    if (coverStartDate && coverStartDate.value.trim() === '') {
        isValid = false;
        errorMessages['errorCoverStartDate'] = 'Cover Start Date is required.';
        scrollToElement('popModalLabel');
    }
    if (coverEndDate && coverEndDate.value.trim() === '') {
        isValid = false;
        errorMessages['errorCoverEndDate'] = 'Cover End Date is required.';
        scrollToElement('popModalLabel');
    }

    // Gross Amount
    const grossAmount = document.getElementById('popAmount');
    if (grossAmount && grossAmount.value.trim() === '') {
        isValid = false;
        errorMessages['errorGrossAmount'] = 'Amount is required.';
        scrollToElement('popModalLabel');
    }

    // Due Date
    const dueDate = document.getElementById('dueDate');
    if (dueDate && dueDate.value.trim() === '') {
        isValid = false;
        errorMessages['errorDueDate'] = 'Due Date is required.';
        scrollToElement('popModalLabel');
    }

    // Update error messages
    Object.keys(errorMessages).forEach(id => {
        document.getElementById(id).textContent = errorMessages[id];
    });

    return isValid; // Return false to prevent form submission if validation fails
}

function scrollToElement(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
}

/* End Filter dropdown types of charges */