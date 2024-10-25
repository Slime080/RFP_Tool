document.addEventListener("DOMContentLoaded", function () {
    function filterTable() {
        var input = document.getElementById("searchInput").value.toLowerCase();
        var table = document.getElementById("rfpTable");
        var rows = table.getElementsByTagName("tr");

        var statusOpenChecked = document.getElementById("statusOpen").checked;
        var statusClosedChecked = document.getElementById("statusClosed").checked;
        var lockLockedChecked = document.getElementById("lockLocked").checked;
        var lockUnlockedChecked = document.getElementById("lockUnlocked").checked;

        for (var i = 1; i < rows.length; i++) {
            var cells = rows[i].getElementsByTagName("td");
            var status = cells[22].textContent;
            var lockStatus = cells[21].textContent;

            var matchesSearch = cells[0].textContent.toLowerCase().includes(input) ||
                cells[1].textContent.toLowerCase().includes(input) ||
                cells[2].textContent.toLowerCase().includes(input);

            var matchesStatus =
                (statusOpenChecked && status.trim() === "Open") ||
                (statusClosedChecked && status.trim() === "Closed");

            var matchesLockStatus =
                (lockLockedChecked && lockStatus.trim() === "Locked") ||
                (lockUnlockedChecked && lockStatus.trim() === "Unlocked");

            if (matchesSearch && matchesStatus && matchesLockStatus) {
                rows[i].style.display = "";
            } else {
                rows[i].style.display = "none";
            }
        }
    }

    // Event listeners for search and checkbox inputs
    document.getElementById("searchInput").addEventListener("input", filterTable);
    document.getElementById("statusOpen").addEventListener("change", filterTable);
    document.getElementById("statusClosed").addEventListener("change", filterTable);
    document.getElementById("lockLocked").addEventListener("change", filterTable);
    document.getElementById("lockUnlocked").addEventListener("change", filterTable);
});
