function confirmArchive() {
            return confirm('Are you sure you want to archive this RFP#?');
        }

    document.addEventListener("DOMContentLoaded", function() {
        var saveButton = document.getElementById('saveButton');

    if (saveButton) {
        saveButton.addEventListener("click", function (event) {
            var confirmationMessage = "Are you sure you want to submi this RFP request?";

            if (!confirm(confirmationMessage)) {
                event.preventDefault();
            }
        });
        }
    });


    document.addEventListener("DOMContentLoaded", function() {
        var saveButton = document.getElementById('saveButtonE');

    if (saveButton) {
        saveButton.addEventListener("click", function (event) {
            var confirmationMessage = "Are you sure you wanted to edit this RFP#?";

            if (!confirm(confirmationMessage)) {
                event.preventDefault();
            }
        });
        }
    });


    document.addEventListener("DOMContentLoaded", function () {
        var saveButton = document.getElementById('deleteButton');

    if (saveButton) {
        saveButton.addEventListener("click", function (event) {
            var confirmationMessage = "Are you sure you wanted to delete this Purpose of Payment?";

            if (!confirm(confirmationMessage)) {
                event.preventDefault();
            }
        });
        }
    });

    document.addEventListener("DOMContentLoaded", function() {
        var saveButton = document.getElementById('Active');

    if (saveButton) {
        saveButton.addEventListener("click", function (event) {
            var confirmationMessage = "Are you sure you wanted this RFP# to be activated?";

            if (!confirm(confirmationMessage)) {
                event.preventDefault();
            }
        });
        }
    });
