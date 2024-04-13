var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        lengthMenu: [
            [5, 10, 20, 30, 40, 50, -1],
            [5, 10, 20, 30, 40, '50']
        ],
        "ajax": {
            "url": "/Admin/Company/GetAll",
            "method": "GET" // Specify the HTTP method for the AJAX request
        },
        "columns": [
            { "data": "name", "width": "10%" },
            { "data": "streetAddress", "width": "15%" },
            { "data": "city", "width": "10%" },
            { "data": "state", "width": "10%" },
            { "data": "postalCode", "width": "10%" },
            { "data": "phoneNumber", "width": "10%" },
            {
                "data": "iAuthorizedCompany","width":"10%",
                "render": function (data) {
                    if (data) {
                        return `<input type="checkbox" checked disabled/>`;
                    }
                    else {
                        return `<input type="checkbox" disabled/>`;
                    }
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Company/upsert/${data}" class="btn btn-info">
                                <i class="fas fa-edit"></i>Edit
                            </a>
                            <a class="btn btn-danger" onclick="Delete('/Admin/Company/Delete/${data}')">
                                <i class="fas fa-trash-alt"></i>Delete
                            </a>
                        </div>
                    `;
                }
            }
        ]
    });
}

function Delete(url) {
    swal({
        title: "Want To Delete Data?",
        text: "Delete Information",
        buttons: true,
        icon: "warning",
        dangerModel: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE", // Correct the HTTP method to "DELETE"
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function (xhr, status, error) {
                    toastr.error("An error occurred while deleting the data.");
                }
            });
        }
    });
}

