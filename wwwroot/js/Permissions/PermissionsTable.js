$(document).ready(function () {
    $("#PermissionsTable").DataTable({
        responsive: true,
        searching: true,
        lengthChange: true,
        autoWidth: false,
        ordering: true,
        info: true,
        paging: true,
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
    .appendTo("#PermissionsTable_wrapper .col-md-6:eq(0)");
});
