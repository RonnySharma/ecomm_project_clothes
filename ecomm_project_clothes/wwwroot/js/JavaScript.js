
var dataTable;
$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            { "data": "name", "width": "20%" },
            { "data": "email", "width": "20%" },
            { "data": "phoneNumber", "width": "20%" },
            { "data": "company.name", "width": "15%" },
            { "data": "role", "width": "15%" },
            {
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },

                "render": function (data) {
                    var today = new Date().getTime();
                    var lockOut = new Date(data.lockoutEnd).getTime();
                    if (lockOut > today) {
                        //User Locked
                        return `
                        <div class="text-center">
                        <a class="btn btn-danger btn-sm mr-1" onclick=LockUnLock('${data.id}')>UnLock</a></div>
                        `;
                    }
                    else {
                        //User Lock
                        return `
                        <div class="text-center">
                        <a class="btn btn-success btn-sm mr-1" onclick=LockUnLock('${data.id}')>Lock</a></div>
                        `;
                    }
                }
            }
        ]
    })
}

function LockUnLock(id) {
    //alert(id);
    $.ajax({
        url: "/Admin/User/LockUnLock",
        type: "POST",
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
            else {
                toastr.error(data.message);
            }
        }
    })
}