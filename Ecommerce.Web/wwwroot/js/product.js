var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": { url: '/Admin/Product/GetAll' },
        "columns": [
            { data: 'name', width: "25%", className: 'text-start' },
            { data: 'author', width: "15%", className: 'text-start' },
            { data: 'isbn', width: "15%", className: 'text-start' },
            { data: 'listPrice', width: "10%", className: 'text-start' },
            { data: 'categoryName', width: "15%", className: 'text-start' },
            {
                data: 'id',
                render: function (data) {
                    return `
                        <div class="btn-group" role="group">
                            <a href="/Admin/Product/Update/${data}" class="btn btn-outline-warning mx-2">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onClick="Delete('/Admin/Product/Delete/${data}')" class="btn btn-outline-danger mx-2">     
                                <i class="bi bi-trash"></i>
                            </a>
                        </div>
                    `;
                },
                width: "20%",
                className: 'text-center'
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to remove this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function () {
                    toastr.error("Something went wrong!");
                }
            });
        }
    });
}
