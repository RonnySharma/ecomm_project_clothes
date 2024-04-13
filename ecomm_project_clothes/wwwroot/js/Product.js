var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        lengthMenu: [
            [5, 8, 12, 15, 18, -1],
            [5, 8, 12, 15, 18, 50],
        ],
        "ajax": {
            "url": "/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "description", "width": "15%" },
            { "data": "category.name", "width": "15%" },
            { "data": "clothesType.name", "width": "15%" },
            { "data": "price", "width": "10%" },
            {
                "data":"imgUrl", "width": "15%",
                "render": function (data) {
                    return `
                    <div class="text-center">
                    <img src="${data}"width="90" height="110"/>
                    </div>
                     `;
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                   <div class="text-center">
                        <a href="/Admin/Product/Upsert/${data}" class="btn btn-primary btn-sm mr-1">
                            <i class="fas fa-edit"></i>Edit
                        </a>

                         <a class="btn btn-danger btn-sm mr-1" onclick=Delete("/Admin/Product/Delete/${data}")>
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
                        dataTable().ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}



