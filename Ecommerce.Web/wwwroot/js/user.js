var dataTable;
let masterEmail = 'abdelrahman.zagloul.dev@gmail.com';

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": {
            url: '/Admin/User/GetAll'
        },
        "columns": [
            { data: 'userName', width: "20%" },
            { data: 'email', width: "20%" },
            { data: 'phoneNumber', width: "20%" },
            {
                data: 'role',
                width: "15%",
                className: 'text-start',
                render: function (data, type, row) {
                    const role = data.toLowerCase();
                    if (role === 'admin') {
                        if (row.email === masterEmail) {
                            return `<span class="badge bg-dark">Master Admin</span>`;
                        } else {
                            return `<span class="badge bg-danger">Admin</span>`;
                        }
                    } else {
                        return `<span class="badge bg-success">${data}</span>`;
                    }
                }
            },
            {
                data: null,
                width: "25%",
                render: function (data, type, row) {
                    const isMasterAdmin = data.email === masterEmail;
                    const isLocked = data.isLocked;

                    let lockUnlockButton = `
                        <button class="btn btn-sm ${isLocked ? 'btn-danger' : 'btn-success'}"
                                onclick="${isMasterAdmin ? 'showProtectedMessage()' : `handleAction('/Admin/User/${isLocked ? 'Unlock' : 'Lock'}/${data.id}')`}"
                                title="${isLocked ? 'Unlock' : 'Lock'}">
                            <i class="bi ${isLocked ? 'bi-lock-fill' : 'bi-unlock-fill'}"></i>
                        </button>`;

                    let editButton = `
                        <button class="btn btn-sm btn-warning"
                                onclick="${isMasterAdmin ? 'showProtectedMessage()' : `window.location='/Admin/User/Update/${data.id}'`}"
                                title="Edit">
                            <i class="bi bi-pencil-square"></i>
                        </button>`;

                    return `
                        <div class="d-flex justify-content-center gap-2 text-nowrap">
                            ${lockUnlockButton}
                            ${editButton}
                        </div>`;
                }
            }
        ]
    });
}

function handleAction(url) {
    $.ajax({
        type: "POST",
        url: url,
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload(null, false);
            } else {
                toastr.error(data.message);
            }
        },
        error: function () {
            toastr.error("Error while processing request.");
        }
    });
}

function showProtectedMessage() {
    toastr.error("Can't modify the Master Admin!");
}
