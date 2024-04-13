var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        "ajax": {
            "url": "/Admin/Category/GetAll"
        },
        "columns": [
            { "data": "name", "width": "70%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="text-center">
                        <a href="/Admin/Category/Upsert/${data}" class="btn btn-primary btn-sm mr-1">
                            <i class="fas fa-edit"></i>Edit
                        </a>
                        <a class="btn btn-danger btn-sm mr-1" onclick=Delete("/Admin/Category/Delete/${data}")>
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
        title: "Want to delete data?",
        text: "Delete Information!!!",
        buttons: true,
        icon: "warning",
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.Success) {
                        toastr.success(data.message)
                       // $('tbldata').DataTable().ajax.reload();
                        setTimeout(function () {
                            location.reload();
                        }, 1000);
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}
