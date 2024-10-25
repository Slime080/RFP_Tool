//document.addEventListener('DOMContentLoaded', function () {

//    const popCodeInput = document.getElementById('popId');
//    const popID = document.getElementById('uID');
//    const editID = document.getElementById('editStoreSelect');

//    const initializeChoices = (elementId, placeholderText) => {
//        const element = document.getElementById(elementId);
//        if (element) {
//            return new Choices(element, {
//                searchEnabled: true,
//                itemSelectText: '',
//                placeholder: true,
//                placeholderValue: placeholderText,
//                searchFields: ['value'],
//                shouldSort: false,
//            });
//        }
//        return null;
//    };

//    initializeChoices('popEntity', 'Entity');
//    const storeSelectChoices = initializeChoices('storeSelect', 'Select Store');
//    initializeChoices('ToCq', 'Select Type of Charge');

//    function fetchChargeToInfo() {
//        const popCode = popCodeInput.value;
//        const popEntity = document.getElementById('popEntity').value;
//        const charge = document.getElementById('ToCq').value;


//        let url = `/RFP/rfpEdit?handler=ChargeToInfo&popId=${popCode}&popEntity=${popEntity}`;
//        if (popID && popID.value) {
//            url += `&id=${popID.value}`;
//        }

//        fetch(url)
//            .then(response => response.json())
//            .then(chargeToList => {
//                storeSelectChoices.clearStore();

//                if (chargeToList && chargeToList.length > 0) {
//                    if (charge.includes("internet")) {
//                        storeSelectChoices.setChoices([{
//                            value: 'SDWAN',
//                            label: 'SD-WAN',
//                            selected: false,
//                            disabled: false
//                        }], 'value', 'label', false);
//                    }

//                    const choices = chargeToList.map(chargeTo => ({
//                        value: chargeTo,
//                        label: chargeTo,
//                        selected: false,
//                        disabled: false
//                    }));
//                    storeSelectChoices.setChoices(choices, 'value', 'label', false);
//                } else {
//                    storeSelectChoices.clearStore();
//                }
//            })
//            .catch(error => {
//                console.error('Error fetching chargeToList: ', error);
//            });
//    }

//    var typeOfChargesElement = document.getElementById('Payee');
//    if (typeOfChargesElement) {
//        new Choices(typeOfChargesElement, {
//            searchEnabled: true,
//            searchResultLimit: 10,
//            itemSelectText: '',
//            placeholder: true,
//            placeholderValue: 'Select Type of Charge'
//        });
//    }

//    document.getElementById('popEntity').addEventListener('change', fetchChargeToInfo);
//    document.getElementById('ToCq').addEventListener('change', fetchChargeToInfo);

//    const initialPopEntityValue = document.getElementById('popEntity').value.trim();
//    if (initialPopEntityValue !== '') {
//        fetchChargeToInfo();
//    }

//});


document.addEventListener('DOMContentLoaded', function () {

    const popCodeInput = document.getElementById('popId');
    const popID = document.getElementById('uID');
    const editID = document.getElementById('editStoreSelect');

    const initializeChoices = (elementId, placeholderText) => {
        const element = document.getElementById(elementId);
        if (element) {
            return new Choices(element, {
                searchEnabled: true,
                itemSelectText: '',
                placeholder: true,
         /*       placeholderValue: placeholderText,*/
                searchFields: ['value'],
                shouldSort: false,
            });
        }
        return null;
    };
    
    initializeChoices('popEntity', 'Entity');
    const storeSelectChoices = initializeChoices('storeSelect', 'Select Store');
    initializeChoices('ToCq', 'Select Type of Charge');

    function fetchChargeToInfo() {
        const popCode = popCodeInput.value;
        const popEntity = document.getElementById('popEntity').value;
        const charge = document.getElementById('ToCq').value;

        let url = `/RFP/rfpEdit?handler=ChargeToInfo&popId=${popCode}&popEntity=${popEntity}`;
        if (popID && popID.value) {
            url += `&id=${popID.value}`;
        }
      
        fetch(url)
            .then(response => response.json())
            .then(chargeToList => {
                // Clear the dropdown before adding new choices
                storeSelectChoices.clearStore();

                if (chargeToList && chargeToList.length > 0) {
                    if (charge.includes("internet")) {
                        storeSelectChoices.setChoices([{
                            value: 'SDWAN',
                            label: 'SD-WAN',
                            selected: false,
                            disabled: false
                        }], 'value', 'label', false);
                    }

                    const choices = chargeToList.map(chargeTo => ({
                        value: chargeTo,
                        label: chargeTo,
                        selected: false,
                        disabled: false
                    }));
                    storeSelectChoices.setChoices(choices, 'value', 'label', false);
                } else {
                    storeSelectChoices.clearStore();
                }
            })
            .catch(error => {
                console.error('Error fetching chargeToList: ', error);
            });
    }

    var typeOfChargesElement = document.getElementById('Payee');
    if (typeOfChargesElement) {
        new Choices(typeOfChargesElement, {
            searchEnabled: true,
            searchResultLimit: 10,
            itemSelectText: '',
            placeholder: true,
            placeholderValue: 'Select Type of Charge'
        });
    }

    document.getElementById('popEntity').addEventListener('change', fetchChargeToInfo);

    const initialPopEntityValue = document.getElementById('popEntity').value.trim();
    if (initialPopEntityValue !== '') {
        fetchChargeToInfo();
 
    }

   

});



