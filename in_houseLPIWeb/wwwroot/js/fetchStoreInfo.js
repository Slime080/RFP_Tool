document.addEventListener('DOMContentLoaded', function () {
    // Initialization code
    const entitySelect = document.getElementById('entity');
    const ifcContainer = document.getElementById('ifcContainer');
    const storeNameInput = document.getElementById('storeNameInput');
    const combineStoreName = document.getElementById('combineStoreName');
    const userDepartment = document.getElementById('dept');
    const sdwanContainer = document.getElementById('sdwanEdit');
    const sdwanToggle = document.getElementById('toggleSwitchSDWAN');
    const sdwanInput = document.getElementById('inputMRC');
    const inputToolPrice1_input = document.getElementById('inputToolPrice1');
    const inputToolPrice2_input = document.getElementById('inputToolPrice2');

    ifcContainer.style.display = 'none';
    sdwanContainer.style.display = 'none';

    // Function to update store name based on entity and input
    const updateStoreName = async () => {
        const entityValue = entitySelect.value;
        const storeNameInputValue = storeNameInput.value;

        try {
            const response = await fetch(`/adminSetup/admStore_Index?handler=EntityInfo&entityValue=${entityValue}`); // change path to approriate PATH
            const entityInfo = await response.json();

            if (entityInfo) {
                const entityPrefixValue = entityInfo.EntityName ? entityInfo.EntityName + ' - ' : '';
                combineStoreName.value = entityPrefixValue + storeNameInputValue;
            } else {
                combineStoreName.value = '';
            }
        } catch (error) {
            console.error('Error fetching entityInfo:', error);
        }
    };

    // Event listener for entity select change
    storeNameInput.addEventListener('input', updateStoreName);

    fetchStoreInfo();

    // Function to fetch store information
    function fetchStoreInfo() {
        const entityValue = entitySelect.value;
        ifcContainer.style.display = entityValue === '1003' ? 'block' : 'none';

        const storeNameValue = combineStoreName.value;
        fetch(`/adminSetup/admStore_Edit?handler=StoreFromDb&name=${storeNameValue}`)
            .then(response => response.json())
            .then(storeInfo => {
                if (storeInfo) {
                    const parts = storeNameValue.split(' - ');
                    storeNameInput.value = parts.length > 1 ? parts[1] : storeNameValue;
                } else {
                    storeNameInput.value = '';
                }
            })
            .catch(error => {
                console.error('Error fetching storeInfo:', error);
            });
    }

    showSDWAN();
    
    sdwanToggle.addEventListener("change", function () {
        if (sdwanToggle.checked) {
            sdwanInput.disabled = false;
            inputToolPrice1_input.disabled = false;
            inputToolPrice2_input.disabled = false;
        } else {
            sdwanInput.value = "0.00"; // This clears the value if checkbox is unchecked
            inputToolPrice1_input.value = "0.00"; // This clears the value if checkbox is unchecked
            inputToolPrice2_input.value = "0.00"; // This clears the value if checkbox is unchecked
            sdwanInput.disabled = true;
            inputToolPrice1_input.disabled = true;
            inputToolPrice2_input.disabled = true;
        }
    });

    // Function to show the SD-WAN Container
    function showSDWAN() {
        if (userDepartment) {
            sdwanContainer.style.display = userDepartment.value === 'IT' ? 'block' : 'none';
        }
        
        if (sdwanInput) {
            function inputFormat(event) {
                let inputValue = event.target.value;
                let cleanedValue = inputValue.replace(/[^0-9.]/g, "");
                cleanedValue = cleanedValue.replace(/\./g, (match, index) =>
                    index === cleanedValue.indexOf(".") ? match : ""
                );
                event.target.value = cleanedValue;
            }
            sdwanInput.addEventListener("input", inputFormat);
        }
    }
});
