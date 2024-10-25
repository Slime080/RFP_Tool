document.addEventListener('DOMContentLoaded', function () {
    // DOM elements
    const ticketCheckbox = document.getElementById('ticketCheck');
    const dateCheckbox = document.getElementById('dateCheck');
    const ticketContainer = document.getElementById('ticket');
    const billPeriodContainer = document.getElementById('billPeriod');
    const billDateContainer = document.getElementById('billDate');
    const ifcContainer = document.getElementById('ifcBox');
    const lpiContainer = document.getElementById('lpiBox');
    ticketContainer.style.display = 'none';
    billDateContainer.style.display = 'none';
    ifcContainer.style.display = 'none';
    lpiContainer.style.display = 'none';
    const ORnum = document.getElementById('ORq');
    const SInum = document.getElementById('SIq');
    const DRnum = document.getElementById('DRq');
    const POnum = document.getElementById('POq');

    // Initialization code for calculation
    const charge = document.getElementById('ToCq');
    const inputAmount = document.getElementById('popAmount');
    const vatPercent = document.getElementById('vatPerc');
    const whtPercent = document.getElementById('whtPerc');
    const basicAmount = document.getElementById('basicAmt');
    const vatAmount = document.getElementById('vatAmt');
    const whtAmount = document.getElementById('whtAmt');
    const netAmount = document.getElementById('netAmt');
    const ifcAmount = document.getElementById('ifcAmt');
    const lpiAmount = document.getElementById('lpiAmt');

    // Initialization code for fetching
    const popEntitySelect = document.getElementById('popEntity');
    const storeSelectInput = document.getElementById('storeSelect');
    const inputCSD = document.getElementById('csd');
    const inputCED = document.getElementById('ced');

    // Debounce utility function
    const debounce = (func, wait) => {
        let timeout;
        return (...args) => {
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(this, args), wait);
        };
    };

    // Event handler for input amount formatting
    const formatInput = debounce(function (event) {
        let inputValue = event.target.value;
        let cleanedValue = inputValue.replace(/[^0-9.]/g, "");
        cleanedValue = cleanedValue.replace(/\./g, (match, index) =>
            index === cleanedValue.indexOf(".") ? match : ""
        );
        event.target.value = cleanedValue;
    }, 250);

    // ====================================================================== FUNCTION FOR CALCULATION AND FETCHING ======================================================================

    // Function to calculate basic amounts
    function basicCalculation() {
        const grossAmount = parseFloat(inputAmount.value) || 0;
        const vatPercValue = parseFloat(vatPercent.value) || 0;
        const whtPercValue = parseFloat(whtPercent.value) || 0;

        let basic = 0;
        let vat = 0;
        let wht = 0;
        let net = 0;

        //console.log('Gross Amount: ', parseFloat(grossAmount), '\n', 'VAT Percent: ', vatPercValue, '%\n', 'WHT Percent: ', whtPercValue, '%');

        if (!isNaN(grossAmount) && (vatPercValue !== null || whtPercValue !== null)) {
            basic = grossAmount / (1 + (vatPercValue / 100));
            vat = grossAmount - basic;
            wht = basic * (whtPercValue / 100);
            net = grossAmount - wht;
        }

        basicAmount.value = basic.toFixed(2);
        vatAmount.value = vat.toFixed(2);
        whtAmount.value = wht.toFixed(2);
        netAmount.value = net.toFixed(2);
    }

    // Function to autofill tax based on charge type
    function autofillTax(charge) {
        const chargeType = charge.value.toLowerCase();

        if (chargeType !== "") {

            // Check for specific keywords
            if (chargeType.includes("rent")) {
                vatPercent.value = "12";
                whtPercent.value = "5";
            } else if (chargeType.includes("electricity")
                || chargeType.includes("water")
                || chargeType.includes("leasehold improvement")
                || chargeType.includes("repair and mainenance")
                || chargeType.includes("majoy equipment")
                || chargeType.includes("preventive maintenance")
            ) {
                vatPercent.value = "12";
                whtPercent.value = "2";
            } else if (chargeType.includes("goods")) {
                vatPercent.value = "12";
                whtPercent.value = "1";
            } else if (chargeType.includes("internet")
                || chargeType.includes("telephone")
            ) {
                vatPercent.value = "12";
                whtPercent.value = "0";
            } else if (chargeType.includes("non vat")
                || chargeType.includes("non vatable")
                || chargeType.includes("non-vat")
                || chargeType.includes("non-vatable")
                || chargeType.includes("salary")
                || chargeType.includes("salaries")
            ) {
                vatPercent.value = "0";
                whtPercent.value = "0";
            } else {
                // Default case
                vatPercent.value = "12";
                whtPercent.value = "0";
            }
        } else {
            vatPercent.value = "";
            whtPercent.value = "";
        }
    }

    function handleInput(data) {
        const chargeType = charge.value.toLowerCase(); // Convert to lowercase

        const basic = basicAmount.value;
        const half = basic / 2;
        const halfBasic = half.toFixed(2);

        // Sub-functions to check the store type
        function isStoreTypePP_SP_EP_RP(storeType) {
            return storeType === "PP" || storeType === "SP" || storeType === "EP" || storeType === "RP";
        }
        function isStoreTypeSP_EP_RP(storeType) {
            return storeType === "SP" || storeType === "EP" || storeType === "RP";
        }
        function isStoreTypePP_SP(storeType) {
            return storeType === "PP" || storeType === "SP";
        }
        function isStoreTypePP(storeType) {
            return storeType === "PP";
        }

        if (chargeType !== "") {
            // Check for specific keywords
            if (chargeType.includes("rent") && isStoreTypePP(data.StoreType)) {
                ifcContainer.style.display = 'block';
                lpiContainer.style.display = 'none';
                ifcAmount.value = basic;
                lpiAmount.value = 0;
            } else if (isStoreTypeSP_EP_RP(data.StoreType) && chargeType.includes("rent")) {
                lpiContainer.style.display = 'block';
                ifcContainer.style.display = 'none';
                lpiAmount.value = basic;
                ifcAmount.value = 0;
            } else if (isStoreTypePP_SP_EP_RP(data.StoreType) && chargeType.includes("electricity")) {
                ifcContainer.style.display = 'block';
                lpiContainer.style.display = 'block';
                ifcAmount.value = halfBasic;
                lpiAmount.value = halfBasic;
            } else if (isStoreTypePP_SP_EP_RP(data.StoreType) && [chargeType.includes("water")
                || chargeType.includes("goods")
                || chargeType.includes("telephone")
                || chargeType.includes("internet")
                || chargeType.includes("preventive")
                || chargeType.includes("preventive maintenance")]) {
                ifcContainer.style.display = 'block';
                lpiContainer.style.display = 'none';
                ifcAmount.value = basic;
                lpiAmount.value = 0;
            } else if (isStoreTypePP_SP(data.StoreType) && (chargeType.includes("leasehold") || chargeType.includes("improvement"))) {
                ifcContainer.style.display = 'block';
                lpiContainer.style.display = 'none';
                ifcAmount.value = basic;
                lpiAmount.value = 0;
            } else if (isStoreTypePP_SP_EP_RP(data.StoreType) && (chargeType.includes("repair") || chargeType.includes("maintenance"))) {
                if (data.LessThanAYear === true) {
                    lpiContainer.style.display = 'block';
                    ifcContainer.style.display = 'none';
                    lpiAmount.value = basic;
                    ifcAmount.value = 0;
                } else {
                    ifcContainer.style.display = 'block';
                    lpiContainer.style.display = 'none';
                    ifcAmount.value = basic;
                    lpiAmount.value = 0;
                }
            } else {
                ifcContainer.style.display = 'block';
                lpiContainer.style.display = 'none';
                ifcAmount.value = basic;
                lpiAmount.value = 0;
            }
        } else {
            lpiContainer.style.display = 'none';
            ifcContainer.style.display = 'none';
            lpiAmount.value = 0;
            ifcAmount.value = 0;
        }
    }

    // Function to fetch store information from the database
    function fetchStoreTypeInfo(popEntity, popStore, coverStartDate, coverEndDate) {
        return fetch(`/RFP/popEdit?handler=StoreTypeInfo&ent=${popEntity}&store=${popStore}&CoverStartDate=${coverStartDate}&CoverEndDate=${coverEndDate}`)
            .then(response => response.json())
            .then(data => {
                if (data && data.StoreType !== undefined) {
                    handleInput(data);
                } else {
                    console.error('Invalid data format');
                }
            })
            .catch(error => {
                console.error('Error fetching data: ', error);
            });
    }

    // Function to display store information
    function displayStoreTypeInfo() {
        if (popEntitySelect.value === '1003' && storeSelectInput.value !== "" && inputCSD.value !== "" && inputCED.value !== "") {
            fetchStoreTypeInfo(popEntitySelect.value, storeSelectInput.value, inputCSD.value, inputCED.value);
        } else {
            lpiContainer.style.display = 'none';
            ifcContainer.style.display = 'none';
            ifcAmount.value = "";
            lpiAmount.value = "";
        }
    }

    // ======================================================================= Event handlers =======================================================================
    inputAmount.addEventListener("input", formatInput);
    if (ticketCheckbox.checked) {
        ticketContainer.style.display = 'block';
    } else {
        ticketContainer.style.display = 'none';
    }
    if (dateCheckbox.checked) {
        billPeriodContainer.style.display = 'none';
        billDateContainer.style.display = 'block';
        document.getElementById('bdate').setAttribute('required', 'required');
    } else {
        billPeriodContainer.style.display = 'block';
        billDateContainer.style.display = 'none';
        document.getElementById('bdate').removeAttribute('required');
    }
    if (ticketCheckbox.checked) {
        ticketContainer.style.display = 'block';
    } else {
        ticketContainer.style.display = 'none';
    }
    ticketCheckbox.addEventListener('change', function () {
        ticketContainer.style.display = ticketCheckbox.checked ? 'block' : 'none';
        if (!ticketCheckbox.checked) {
            ORnum.value = "";
            SInum.value = "";
            DRnum.value = "";
            POnum.value = "";
        }
    });
    dateCheckbox.addEventListener('change', function () {
        billPeriodContainer.style.display = dateCheckbox.checked ? 'none' : 'block';
        billDateContainer.style.display = dateCheckbox.checked ? 'block' : 'none';
        if (dateCheckbox.checked) {
            inputCSD.value = "";
            inputCED.value = "";
        } else {
            document.getElementById('bdate').value = "";
        }
    });
    document.getElementById('bdate').addEventListener('change', function () {
        var inputCSD = document.getElementById('csd');
        var inputCED = document.getElementById('ced');
        inputCSD.value = this.value;
        inputCED.value = this.value;
    });

    if (charge) {
        charge.addEventListener("change", function () {
            autofillTax(charge);
            debounce(basicCalculation, 500)();
        });
    }
    if (inputAmount || vatPercent || whtPercent) {
        inputAmount.addEventListener("input", debounce(basicCalculation, 500));
        vatPercent.addEventListener("input", debounce(basicCalculation, 500));
        whtPercent.addEventListener("input", debounce(basicCalculation, 500));
    }

    // Check if all required fields have content
    if (popEntitySelect && storeSelectInput && inputCSD && inputCED) {
        debounce(displayStoreTypeInfo, 500)();
        charge.addEventListener('change', debounce(displayStoreTypeInfo, 500));
        storeSelectInput.addEventListener('change', debounce(displayStoreTypeInfo, 500));
        inputAmount.addEventListener('input', debounce(displayStoreTypeInfo, 500));
    }

    if (inputAmount.value.trim() !== '') {
        basicCalculation();
    }

});
