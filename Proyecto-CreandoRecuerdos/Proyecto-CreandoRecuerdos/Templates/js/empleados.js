$(document).ready(function () {
    function showAlert(icon, title, text) {
        Swal.fire({
            icon: icon,
            title: title,
            text: text,
            confirmButtonColor: '#B54885'
        });
    }

    // Mostrar confirmaci�n
    function showConfirm(title, text, callback) {
        Swal.fire({
            title: title,
            text: text,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#B54885',
            cancelButtonColor: '#6C757D',
            confirmButtonText: 'Sí, continuar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                callback();
            }
        });
    }

    // Editar empleado
    $(document).on('click', '.btn-editar', function () {
        const id = $(this).data('id');

        $.get('/Empleados/ObtenerEmpleado', { id: id }, function (data) {
            $('#id_usuario').val(data.id_usuario);
            $('#nombre').val(data.nombre);
            $('#correo').val(data.correo);
            $('#idRol').val(data.idRol);
            $('#activo').val(data.activo.toString());
            $('#contrasenna').val('');

            $('#btnGuardar').text('Actualizar');
            $('html, body').animate({
                scrollTop: $('#formEmpleado').offset().top - 20
            }, 500);
        }).fail(function () {
            showAlert('error', 'Error', 'No se pudo cargar la informacion del empleado');
        });
    });

    // Eliminar empleado
    $(document).on('click', '.btn-eliminar', function () {
        const id = $(this).data('id');
        const nombre = $(this).closest('tr').find('td:first').text();

        showConfirm(
            'Eliminar empleado',
            `Confirma que deseas eliminar al empleado ${nombre}?`,
            function () {
                $.post('/Empleados/EliminarEmpleado', { id: id })
                    .done(function () {
                        showAlert('success', 'Exito', 'Empleado eliminado correctamente');
                        setTimeout(() => {
                            window.location.reload();
                        }, 1500);
                    })
                    .fail(function (xhr) {
                        showAlert('error', 'Error', xhr.responseText || 'Ocurrio un error al eliminar el empleado');
                    });
            }
        );
    });

    // Cancelar edición
    $('#btnCancelar').click(function () {
        $('#id_usuario').val('0');
        $('#formEmpleado')[0].reset();
        $('#formEmpleado').validate().resetForm();
        $('#btnGuardar').text('Guardar');
    });
});