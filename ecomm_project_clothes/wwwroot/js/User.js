var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        lengthMenu: [
            [5, 8, 12, 15, 18, -1],
            [5, 8, 12, 15, 18, '50'],
        ],
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phone_number", "width": "15%" },
            { "data": "company.name", "width": "15%" },
            { "data": "role", "width": "15%" },
            {
                "data": null,
                "render": function (data, type, row) {
                    return '<button class="lock-button">Lock</button>';
                },
                "width": "10%"
            },
            {
                "data": null,
                "render": function (data, type, row) {
                    return '<button class="unlock-button">Unlock</button>';
                },
                "width": "10%"
            }
        ]
    });

    // Add event handlers for lock and unlock buttons
    $('#tblData').on('click', '.lock-button', function () {
        var data = dataTable.api().row($(this).closest('tr')).data();
        // Handle lock button click here
        console.log('Lock button clicked for', data);
    });

    $('#tblData').on('click', '.unlock-button', function () {
        var data = dataTable.api().row($(this).closest('tr')).data();
        // Handle unlock button click here
        console.log('Unlock button clicked for', data);
    });
}
