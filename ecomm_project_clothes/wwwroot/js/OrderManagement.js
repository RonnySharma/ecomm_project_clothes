var dataTable;

$(document).ready(function () {
    loadDataTable();
    // Removed the redundant initialization
});

function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        "ajax": {
            "url": "/Admin/OrderManagement/GetAll",
            "type": "GET"
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "orderDate", "width": "30%" },
            { "data": "applicationUser.name", "width": "10%" },
            { "data": "orderTotal", "width": "10%" },
            { "data": "orderStatus", "width": "10%" },
            {
                "data": "id",
                "width": "15%",
                "render": function (data) {
                    return `<a href="/Admin/OrderManagement/Details/${data}" class="btn btn-success">View Details</a>`;
                }
            }
        ]
    });
}

function initializeOrderTable() {
    // Removed the redundant DataTable initialization

    // Handle the form submission
    $('form').submit(function (event) {
        event.preventDefault();

        var formData = $(this).serialize();
        var url = $(this).attr('action');

        // AJAX request to get filtered order data
        $.ajax({
            type: 'GET',
            url: url,
            data: formData,
            success: function (data) {
                dataTable.clear().draw();

                $.each(data, function (index, order) {
                    dataTable.row.add([
                        order.OrderId,
                        order.DateTime,
                        order.UserName,
                        order.TotalPrice,
                        order.OrderStatus,
                        '<a href="/Order/Details/' + order.OrderId + '">Details</a>'
                    ]).draw();
                });
            },
            error: function () {
                console.log('Error fetching order data.');
            }
        });
    });
}

// Uncomment and modify the date range search function as per your requirement
function setupDateRangeSearch() {
    $('#dateRangeSearchButton').click(function () {
        var startDate = $('#fromDate').val();
        var endDate = $('#toDate').val();

        dataTable.columns(1).search('').draw();

        if (startDate && endDate) {
            dataTable.columns(1).search(startDate + ' to ' + endDate).draw();
        } else if (startDate) {
            dataTable.columns(1).search('>= ' + startDate).draw();
        } else if (endDate) {
            dataTable.columns(1).search('<= ' + endDate).draw();
        }
    });
}
