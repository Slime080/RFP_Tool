document.addEventListener('DOMContentLoaded', function () {
    var deptDropdown = document.getElementById('departmentDropdown');

    if (deptDropdown != null) {
        deptDropdown.addEventListener('change', retrievedUserLevel);
    }

    function retrievedUserLevel() {
        var deptName = deptDropdown.value; // Capture the current selected department value here

        fetch(`/adminSetup/admUser_Edit?handler=UserLevels&deptName=${deptName}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                $('#userLevelDropdown').empty();
                data.forEach(item => {
                    $('#userLevelDropdown').append($('<option>').text(item.Text).attr('value', item.Value));
                });
            })
            .catch(error => {
                console.error('Error fetching data:', error);
            });
    }
});
