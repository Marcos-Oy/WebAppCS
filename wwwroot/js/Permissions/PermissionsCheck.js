$(document).ready(function () {
    // Función para habilitar o deshabilitar los checkboxes de permisos
    function togglePermissions(modulo, isEnabled) {
        $('input[name="permisos[' + modulo + '].Crear"]').prop('disabled', !isEnabled).prop('checked', isEnabled ? $('input[name="permisos[' + modulo + '].Crear"]').prop('checked') : false);
        $('input[name="permisos[' + modulo + '].Editar"]').prop('disabled', !isEnabled).prop('checked', isEnabled ? $('input[name="permisos[' + modulo + '].Editar"]').prop('checked') : false);
        $('input[name="permisos[' + modulo + '].Eliminar"]').prop('disabled', !isEnabled).prop('checked', isEnabled ? $('input[name="permisos[' + modulo + '].Eliminar"]').prop('checked') : false);
        $('input[name="permisos[' + modulo + '].Activar_Desactivar"]').prop('disabled', !isEnabled).prop('checked', isEnabled ? $('input[name="permisos[' + modulo + '].Activar_Desactivar"]').prop('checked') : false);
        $('input[name="permisos[' + modulo + '].Restaurar"]').prop('disabled', !isEnabled).prop('checked', isEnabled ? $('input[name="permisos[' + modulo + '].Restaurar"]').prop('checked') : false);
        $('input[name="permisos[' + modulo + '].Cambiar_Password"]').prop('disabled', !isEnabled).prop('checked', isEnabled ? $('input[name="permisos[' + modulo + '].Cambiar_Password"]').prop('checked') : false);
        $('input[name="permisos[' + modulo + '].Migrar_Rol"]').prop('disabled', !isEnabled).prop('checked', isEnabled ? $('input[name="permisos[' + modulo + '].Migrar_Rol"]').prop('checked') : false);

    }

    // Escucha los cambios de los checkboxes de acceso
    $('input[type="checkbox"][name$=".Acceso"]').change(function() {
        var modulo = $(this).attr('id').split('_')[1];
        var label = $(this).next('input').next('label'); // Selecciona el label
        
        if ($(this).prop('checked')) {
            togglePermissions(modulo, true);
            label.text("Permitido"); // Actualiza el texto
        } else {
            togglePermissions(modulo, false);
            label.text("Denegado"); // Actualiza el texto
        }
    });

    // Asegúrate de que los checkboxes de permisos estén habilitados o deshabilitados al cargar la página
    $('input[type="checkbox"][name$=".Acceso"]').each(function () {
        var modulo = $(this).attr('id').split('_')[1]; // Obtén el módulo del id
        if ($(this).prop('checked')) {
            togglePermissions(modulo, true);  // Habilita los permisos si el checkbox de acceso está marcado
        } else {
            togglePermissions(modulo, false); // Deshabilita los permisos si el checkbox de acceso no está marcado
        }
    });
});