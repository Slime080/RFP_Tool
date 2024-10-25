document.addEventListener('DOMContentLoaded', function () {
    const entitySelect = document.getElementById('entity');
    const ifcContainer = document.getElementById('ifcContainer');

    ifcContainer.style.display = 'none';

    const updateStoreName = async () => {
        const entityValue = entitySelect.value;
        const storeNameInputValue = document.getElementById('storeNameInput').value;

        try {
            const response = await fetch(`/adminSetup/admStore_Index?handler=EntityInfo&entityValue=${entityValue}`);
            const entityInfo = await response.json();

            const entityPrefixValue = entityInfo ? entityInfo.EntityName + ' - ' : '';
            const concatenatedValue = entityPrefixValue + storeNameInputValue;
            document.getElementById('combineStoreName').value = entityInfo ? concatenatedValue : '';
        } catch (error) {
            console.error('Error fetching entityInfo:', error);
        }
    };

    entitySelect.addEventListener('change', () => {
        ifcContainer.style.display = entitySelect.value === '1003' ? 'block' : 'none';
        updateStoreName();
    });

    document.getElementById('storeNameInput').addEventListener('input', updateStoreName);

    entitySelect.addEventListener('change', () => {
        const entityValue = entitySelect.value;
        const entityPrefixInput = document.getElementById('entityPrefix');

        fetch(`/adminSetup/admStore_Index?handler=EntityInfo&entityValue=${entityValue}`)
            .then(response => response.json())
            .then(entityInfo => {
                const prefix = entityInfo ? entityInfo.EntityName : '';
                const regex = new RegExp(`^${prefix} - `);
                const replacedValue = entityPrefixInput.value.replace(regex, '');
                entityPrefixInput.value = prefix ? `${prefix} - ${entityPrefixInput.value.replace(replacedValue, '')}` : '';
            })
            .catch(error => {
                console.error('Error fetching entityInfo:', error);
            });
    });
});
