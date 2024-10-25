function validateForm() {
    let isValid = true;

    // Clear previous error messages
    document.getElementById('errormsg').textContent = '';
    document.getElementById('errorPopEntity').textContent = '';
    document.getElementById('errorChargeTo').textContent = '';
    document.getElementById('errorCoverStartDate').textContent = '';
    document.getElementById('errorCoverEndDate').textContent = '';
    document.getElementById('errorGrossAmount').textContent = '';
    document.getElementById('errorDueDate').textContent = '';

    // Type of Charges
    const toCq = document.getElementById('ToCq');
    if (toCq && toCq.value.trim() === '') {
        isValid = false;
        document.getElementById('errormsg').textContent = 'Type of Charges is required.';
        scrollToElement('popModalLabel');
    }

    // Charge To
    const popEntity = document.getElementById('popEntity');
    const chargeTo = document.getElementById('storeSelect');
    if (popEntity && popEntity.value.trim() === '') {
        isValid = false;
        document.getElementById('errorPopEntity').textContent = '(Entity) is required.';
        scrollToElement('popModalLabel');
    }
    if (chargeTo && chargeTo.value.trim() === '') {
        isValid = false;
        document.getElementById('errorChargeTo').textContent = 'Charge To (Store) is required.';
        scrollToElement('popModalLabel');
    }

    // Cover Dates
    const coverStartDate = document.getElementById('csd');
    const coverEndDate = document.getElementById('ced');
    if (coverStartDate && coverStartDate.value.trim() === '') {
        isValid = false;
        document.getElementById('errorCoverStartDate').textContent = 'Cover Start Date is required.';
        scrollToElement('popModalLabel');
    }
    if (coverEndDate && coverEndDate.value.trim() === '') {
        isValid = false;
        document.getElementById('errorCoverEndDate').textContent = 'Cover End Date is required.';
        scrollToElement('popModalLabel');
    }

    // Gross Amount
    const grossAmount = document.getElementById('popAmount');
    if (grossAmount && grossAmount.value.trim() === '') {
        isValid = false;
        document.getElementById('errorGrossAmount').textContent = 'Amount is required.';
        scrollToElement('popModalLabel');
    }

    // Due Date
    const dueDate = document.getElementById('dueDate');
    if (dueDate && dueDate.value.trim() === '') {
        isValid = false;
        document.getElementById('errorDueDate').textContent = 'Due Date is required.';
        scrollToElement('popModalLabel');
    }

    if (isValid) {
        setTimeout(function () {
            window.location.reload();
        }, 1000); // Adjust the timeout as needed
    }

    return isValid;
}

function scrollToElement(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
}
