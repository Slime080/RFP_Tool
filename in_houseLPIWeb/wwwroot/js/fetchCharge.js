document.addEventListener('DOMContentLoaded', function () {
    const storeSelectInput = document.getElementById('storeSelect');
    const popEntitySelect = document.getElementById('popEntity');
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
                placeholderValue: placeholderText,
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

                logSelectedStore();
                setStoredStoreSelection();
            })
            .catch(error => {
                console.error('Error fetching chargeToList: ', error);
            });
    }

    function logSelectedStore() {
        const selectedOption = storeSelectInput.options[storeSelectInput.selectedIndex];
        const selectedEntity = popEntitySelect.value;

        if (selectedOption) {
            const storeId = selectedOption.value;
            const storeName = selectedOption.text;

            console.log('Selected Id:', storeId);
            console.log('Selected StoreName:', storeName);
            console.log('Selected Entity:', selectedEntity);

            // Store values in localStorage
            localStorage.setItem('StoredId', storeId);
            localStorage.setItem('StoredName', storeName); // Update to use actual store name
            localStorage.setItem('StoredEntity', selectedEntity);

            console.log('Stored Id:', localStorage.getItem('StoredId'));
            console.log('Stored StoreName:', localStorage.getItem('StoredName'));
            console.log('Stored Entity:', localStorage.getItem('StoredEntity'));
        } else {
            console.log('No option selected.');
            // Clear stored values if no option is selected
            localStorage.removeItem('StoredId');
            localStorage.removeItem('StoredName');
            localStorage.removeItem('StoredEntity');
        }
    }

    function setStoredStoreSelection() {
        const storedStoreName = localStorage.getItem('StoredName');

        const selectElement = storeSelectChoices.passedElement.element;
        const optionToSelect = Array.from(selectElement.options).find(option => option.value === storedStoreName);

        if (storedStoreName) {
            storeSelectChoices.setChoiceByValue(storedStoreName);
        }
    }

    document.getElementById('popEntity').addEventListener('change', fetchChargeToInfo);
    /*    document.getElementById('ToCq').addEventListener('change', fetchChargeToInfo);*/

    const initialPopEntityValue = document.getElementById('popEntity').value.trim();
    if (initialPopEntityValue !== '') {
        fetchChargeToInfo();
    }
});

