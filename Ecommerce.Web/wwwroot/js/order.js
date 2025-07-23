var dataTable;

$(document).ready(function () {
    const params = new URLSearchParams(window.location.search);
    const orderStatus = params.get("orderStatus");
    const paymentStatus = params.get("paymentStatus");

    loadDataTable(orderStatus, paymentStatus);
});

function loadDataTable(orderStatus, paymentStatus) {
    let url = '/Admin/Order/GetAll?';
    if (orderStatus) url += 'orderStatus=' + orderStatus + '&';
    if (paymentStatus) url += 'paymentStatus=' + paymentStatus;

    dataTable = $('#myTable').DataTable({
        ajax: { url: url },
        pageLength: 10,
        responsive: true,
        columnDefs: [
            { targets: [0, 3, 4, 5, 6], className: 'text-center' },
            { targets: [1, 2], className: 'text-start' },
        ],
        columns: [
            { data: 'id', width: "5%" },
            { data: 'name', width: "20%" },
            { data: 'phoneNumber', width: "20%" },
            {
                data: 'paymentStatus',
                render: function (data) {
                    let badgeClass = 'secondary';
                    if (data === 'Approved') badgeClass = 'success';
                    else if (data === 'Refunded') badgeClass = 'info';
                    else if (data === 'Rejected') badgeClass = 'danger';
                    return `<span class="badge bg-${badgeClass} px-2 py-1">${data}</span>`;
                },
                width: "12%"
            },
            {
                data: 'orderStatus',
                render: function (data) {
                    let badgeClass = 'secondary';
                    if (data === 'Approved') badgeClass = 'primary';
                    else if (data === 'Shipped') badgeClass = 'warning';
                    else if (data === 'Delivered') badgeClass = 'success';
                    else if (data === 'Cancelled') badgeClass = 'danger';
                    return `<span class="badge bg-${badgeClass} px-2 py-1">${data}</span>`;
                },
                width: "12%"
            },
            {
                data: 'total',
                render: function (data) {
                    return `<span class="fw-semibold">${data.toFixed(2)} EGP</span>`;
                },
                width: "10%"
            },
            {
                data: 'id',
                render: function (data) {
                    return `
                        <div class="btn-group" role="group">
                            <a href="/Admin/Order/Details/${data}" class="btn btn-sm btn-outline-primary" title="Edit Order">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                        </div>
                    `;
                },
                width: "5%"
            }
        ]
    });
}
