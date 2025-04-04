$(document).ready(function () {
    $("#RolesTable").DataTable({
        responsive: true,
        searching: true,
        lengthChange: true,
        autoWidth: false,
        ordering: true,
        info: true,
        paging: true,
        buttons: [
            {
                extend: "copy",
                text: "Copiar",
                title: "Lista de Usuarios",
                exportOptions: {
                    columns: ':not(:last-child)'
                }
            },
            {
                extend: "pdf",
                text: "PDF",
                title: "Lista de Usuarios",
                exportOptions: {
                    columns: ':not(:last-child)'
                }
            },
            {
                extend: "csv",
                text: "CSV",
                title: "Lista de Usuarios",
                exportOptions: {
                    columns: ':not(:last-child)'
                }
            },
            {
                extend: "excel",
                text: "EXCEL",
                title: "Lista de Usuarios",
                exportOptions: {
                    columns: ':not(:last-child)'
                }
            },
            {
                extend: "print",
                text: "Imprimir",
                title: "Lista de Usuarios",
                exportOptions: {
                    columns: ':not(:last-child)'
                }
            },
        ],
        language: {
            lengthMenu: "Mostrar _MENU_ registros",
            zeroRecords: "No se encontraron resultados",
            info: "Mostrando página _PAGE_ de _PAGES_",
            infoEmpty: "No hay registros disponibles",
            infoFiltered: "(filtrado de _MAX_ registros totales)",
            search: "Buscar:",
            paginate: {
                first: "Primero",
                last: "Último",
                next: "Siguiente",
                previous: "Anterior"
            }
        },
    })
    .buttons()
    .container()
    .appendTo("#RolesTable_wrapper .col-md-6:eq(0)");
});
