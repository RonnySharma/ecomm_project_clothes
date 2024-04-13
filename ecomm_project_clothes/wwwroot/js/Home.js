var dataTable;
$(document).ready(function ()
{
    loadDataTable();
});
function loadDataTable() {
    dataTable =$('#productTable').DataTable({
        "order": [], // Disable initial sorting
        "paging": true, // Enable pagination
        "searching": true, // Enable search box
        "info": true, // Show information about the table
        "language": {
            "search": "Search products:", // Search box label
            "lengthMenu": "Show _MENU_ products per page", // Page length label
            "info": "Showing _START_ to _END_ of _TOTAL_ products", // Information label
            "paginate": {
                "first": "First",
                "last": "Last",
                "next": "Next",
                "previous": "Previous"
            }
        },
        "columnDefs": [
            { "orderable": false, "targets": [0, 5] } // Disable sorting for the image and action columns
        ],
        "columns": [
            {
                "data": "ImgUrl", "render": function (data) {
                    return '<img src="' + data + '" height="100px" class="rounded" />';
                }
            },
            { "data": "Brand.Name" },
            { "data": "ClothesType.Name" },
            {
                "data": "ListPrice", "render": function (data) {
                    return '<strike>₹' + data.toFixed(2) + '</strike>';
                }
            },
            {
                "data": "Price75", "render": function (data) {
                    return '<span style="color: maroon">₹' + data.toFixed(2) + '</span>';
                }
            },
            {
                "data": null, "render": function (data, type, row) {
                    return '<a href="/Home/Details/' + row.Id + '" class="btn btn-primary">Details</a>';
                }
            }
        ]
    });
}
