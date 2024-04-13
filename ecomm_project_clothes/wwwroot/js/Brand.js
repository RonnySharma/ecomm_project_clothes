var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        lengthMenu: [
            [5, 8, 12, 15, 18, -1],
            [5, 8, 12, 15, 18,'50'],
        ],
        "ajax": {
            "url": "/Admin/Brand/GetAll"
        },
        "columns": [
            { "data": "name", "width": "70%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                   <div class="text-center">
                        <a href="/Admin/Brand/Upsert/${data}" class="btn btn-primary btn-sm mr-1">
                            <i class="fas fa-edit"></i> Edit
                        </a>
                         <a class="btn btn-danger btn-sm mr-1" onclick=Delete("/Admin/Brand/Delete/${data}")>
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
                        toastr.success(data.message);
                        setTimeout(function () {
                            location.reload();
                        }, 1000);
                        location.reload(); // Refresh the page
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}
