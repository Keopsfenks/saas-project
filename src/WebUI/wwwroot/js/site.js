$(document).ready(function () {
    $('table').DataTable({
        scrollX: true,
        responsive: true,
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.1/i18n/tr.json',
        },
        columnDefs: [
            { "orderable": false, "targets": 'no-sort' }
        ],
        initComplete: function () {
            $("table").removeClass("d-none");
        },
        scrollY: '400px',
        scrollCollapse: false,
    });
});
