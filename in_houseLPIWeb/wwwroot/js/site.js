// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//This is for the navigation extra menu
$(document).ready(function () {
    function setupDropdown($container, menuClass) {
        $('.menu.' + menuClass).mouseenter(function () {
            // Show the dropdown menu when the mouse enters the element
            $container.stop(true, true).delay(200).fadeIn(300);
        });

        // Keep the dropdown open when clicked
        $container.click(function (e) {
            e.stopPropagation(); // Prevent the click event from propagating to the document
        });

        // Close the dropdown when clicking outside of it or when the mouse leaves the dropdown
        $(document).on('click mouseleave', function (e) {
            if (!$(e.target).closest('.menu.' + menuClass).length && !$(e.target).closest('#' + menuClass).length) {
                $container.fadeOut(300);
            }
        });
    }

    var $rfpContainer = $('#rfpContainer');
    setupDropdown($rfpContainer, 'rfpMenu');

});


document.addEventListener('DOMContentLoaded', function () {
    function updateDateTime() {
        var now = new Date();
        var formattedDateTime = now.getFullYear() + '-' + ('0' + (now.getMonth() + 1)).slice(-2) + '-' + ('0' + now.getDate()).slice(-2) +
            ' ' + ('0' + now.getHours()).slice(-2) + ':' + ('0' + now.getMinutes()).slice(-2) + ':' + ('0' + now.getSeconds()).slice(-2);
        document.getElementById('currentDateTime').innerText = formattedDateTime;
    }

    // Call updateDateTime initially and then set up the interval
    updateDateTime();
    setInterval(updateDateTime, 1000);

    // Disable right-click context menu
    document.addEventListener('contextmenu', function (e) {
        e.preventDefault();
    });

    // Below are used to prevent the user from visting the Developers tool
    document.onkeydown = (e) => {
        // Define keys to prevent
        const keysToPrevent = ['I', 'i', 'C', 'c', 'J', 'j', 'U', 'u'];

        // Prevent F12 (often opens developer tools)
        if (e.key === 'F12') {
            e.preventDefault();
        }

        // Prevent Ctrl+Shift+{I, C, J} and Ctrl+U
        if ((e.ctrlKey && e.shiftKey && keysToPrevent.includes(e.key)) || (e.ctrlKey && keysToPrevent.slice(-2).includes(e.key))) {
            e.preventDefault();
            e.stopPropagation();
        }
    };
});
