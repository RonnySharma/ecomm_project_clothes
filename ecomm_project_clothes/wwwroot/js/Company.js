var dataTable;
$(document).ready(function () {
    loadDataTable();
})
function loadDataTable() {
    dataTable = $('#tblData').dataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "10%" },
            { "data": "streetAddress", "width": "10%" },
            { "data": "city", "width": "10%" },
            { "data": "state", "width": "10%" },
            { "data": "postalCode", "width": "10%" },
            { "data": "phoneNumber", "width": "10%" },
            {
                "data": "isAuthorizedCompany","width":"5%",
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
                    <a href="/Admin/Company/Upsert/${data}" class="btn btn-info btn-sm mr-1">
                    <i class="fas fa-edit"></i>Edit
                    </a>
                    <a class="btn btn-danger btn-sm mr-1" onclick=Delete("/Admin/Company/Delete/${data}")>
                    <i class="fas fa-trash-alt"></i>Delete
                    </a>
                    </div>
                    `;
                }
            }
        ]
    })
}
function Delete(url) {
    swal({
        title: "Want To Delete Data?",
        text: "Delete Information",
        buttons: true,
        icon: "warning",
        dangerModel: true
    }).then((willdelete) => {
        if (willdelete) {
            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        setTimeout(function () {
                            location.reload();
                        }, 1000);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}
