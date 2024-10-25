$(document).ready(function () {
    var table = $('#ifcTable').DataTable({
        "initComplete": function (settings, json) {
            $('.dataTables_filter input').attr('id', 'search-input');
        },
        columns: [
            {
                data: 'RFP_No',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Assuming data is a number, add 1000000 and format the string
                        return 'LAWHO-' + (parseInt(data) + 1000000);
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            { data: 'ChargeTo' },
            { data: 'StoreType' },
            {
                data: 'OpenDate',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the date for display
                        var openDate = new Date(data);
                        var options = {
                            year: 'numeric',
                            month: 'numeric',
                            day: 'numeric'
                        };
                        return openDate.toLocaleString('en-US', options);
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            {
                data: 'CoverStartDate',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the date for display
                        var csd = new Date(data);
                        var options = {
                            year: 'numeric',
                            month: 'numeric',
                            day: 'numeric'
                        };
                        return csd.toLocaleString('en-US', options);
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            {
                data: 'CoverEndDate',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the date for display
                        var ced = new Date(data);
                        var options = {
                            year: 'numeric',
                            month: 'numeric',
                            day: 'numeric'
                        };
                        return ced.toLocaleString('en-US', options);
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            { data: 'ToC' },
            {
                data: 'GrossAmt',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the number for display
                        var formattedNumber = Number(data).toLocaleString('en-US', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        });
                        return formattedNumber;
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            { data: 'VATPerc' },
            {
                data: 'VATAmt',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the number for display
                        var formattedNumber = Number(data).toLocaleString('en-US', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        });
                        return formattedNumber;
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            {
                data: 'BasicAmt',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the number for display
                        var formattedNumber = Number(data).toLocaleString('en-US', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        });
                        return formattedNumber;
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            { data: 'WHTPerc' },
            {
                data: 'WHTAmt',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the number for display
                        var formattedNumber = Number(data).toLocaleString('en-US', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        });
                        return formattedNumber;
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            {
                data: 'NETAmt',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the number for display
                        var formattedNumber = Number(data).toLocaleString('en-US', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        });
                        return formattedNumber;
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            { data: 'ProrateResult' },
            { data: 'ProrateDays' },
            { data: 'ProrateDivident' },
            { data: 'IFCProratePerc' },
            { data: 'LPIProratePerc' },
            {
                data: 'MatrixAmtResult',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the number for display
                        var formattedNumber = Number(data).toLocaleString('en-US', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        });
                        return formattedNumber;
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            {
                data: 'IFCPartAmt',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the number for display
                        var formattedNumber = Number(data).toLocaleString('en-US', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        });
                        return formattedNumber;
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            {
                data: 'LPIPartAmt',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the number for display
                        var formattedNumber = Number(data).toLocaleString('en-US', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        });
                        return formattedNumber;
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            {
                data: 'MatrixVATResult',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the number for display
                        var formattedNumber = Number(data).toLocaleString('en-US', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        });
                        return formattedNumber;
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            {
                data: 'IFCMatrixVat',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the number for display
                        var formattedNumber = Number(data).toLocaleString('en-US', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        });
                        return formattedNumber;
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            {
                data: 'LPIMatrixVat',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the number for display
                        var formattedNumber = Number(data).toLocaleString('en-US', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        });
                        return formattedNumber;
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            {
                data: 'TotalIFCPart',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the number for display
                        var formattedNumber = Number(data).toLocaleString('en-US', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        });
                        return formattedNumber;
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            {
                data: 'TotalLPIPart',
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Format the number for display
                        var formattedNumber = Number(data).toLocaleString('en-US', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        });
                        return formattedNumber;
                    }
                    return data; // For sorting and other purposes, return the original data
                }
            },
            {
                data: null,
                orderable: false,
                render: function (data, type, row) {
                    var editButton = '<div class="d-grid gap-2 d-md-block text-center">';
                    var hasAccessValue = document.getElementById('accessTF').value === 'true';

                    if (hasAccessValue) {
                        if (row.ChargeTo && row.ChargeTo.includes("IFC")) {
                            editButton += '<a href="/RFP/rfpView?id=' + row.RFP_No + '" class="btn btn-outline-primary">Edit</a>';
                        } else {
                            editButton += '<a href="/RFP/rfpView?id=' + row.RFP_No + '" class="btn btn-outline-secondary disabled">Edit</a>';
                        }
                    } else {
                        editButton += '<a href="/RFP/rfpView?id=' + row.RFP_No + '" class="btn btn-outline-secondary disabled">Edit</a>';
                    }
                    editButton += '</div>';
                    return editButton;
                }
            }
        ],
        "paging": true,
        "searching": true,
        "info": true,
    });

    $('#searchSelect').on('change', function () {
        if (table) {
            var selectedValue = $(this).val();
            console.log('selected: ', selectedValue);
            table.columns(1).search(selectedValue).draw(); // Dropdown for Store Name
        }
    });

    function fetchDataFromSP(param) {
        $.ajax({
            type: "GET",
            url: `/IFC/ifcIndex?handler=FetchData&param=${param}`,
            success: function (data) {
                console.log(data);

                // Assuming your data variable contains the new data
                table.clear().rows.add(data).draw();
            },
            error: function (error) {
                console.error("Error fetching data from server: " + error);
            }
        });
    }

    $('#CoverDateSelect').on('change', function () {
        var selectedValue = $(this).val();
        fetchDataFromSP(selectedValue); // Pass the selected value from the dropdown
    });
});