$(document).ready(function () {
    function showAlert(icon, title, text) {
        Swal.fire({
            icon: icon,
            title: title,
            text: text,
            confirmButtonColor: '#E27AB0'
        });
    }

    // Mostrar confirmación
    function showConfirm(title, text, callback) {
        Swal.fire({
            title: title,
            text: text,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#E27AB0',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Si, continuar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                callback();
            }
        });
    }

    // Validación del correo
    $.validator.addMethod("uniqueEmail", function (value, element) {
        let isUnique = true;
        const currentId = $('#id_usuario').val();

        $.ajax({
            url: '/Empleados/VerificarCorreo', 
            type: 'GET',
            async: false,
            data: {
                correo: value,
                idUsuario: currentId
            },
            success: function (response) {
                isUnique = response.isUnique;
            }
        });

        return isUnique;
    }, "Este correo electronico ya esta registrado");

    // Vvalidación del formulario
    $('#formEmpleado').validate({
        rules: {
            nombre: {
                required: true,
                maxlength: 100
            },
            correo: {
                required: true,
                email: true,
                maxlength: 100,
                uniqueEmail: true
            },
            contrasenna: {
                minlength: 8,
                maxlength: 100
            },
            idRol: {
                required: true
            },
            activo: {
                required: true
            }
        },
        messages: {
            nombre: {
                required: "Por favor ingrese el nombre del empleado",
                maxlength: "El nombre no puede exceder los 100 caracteres"
            },
            correo: {
                required: "Por favor ingrese el correo electronico",
                email: "Por favor ingrese un correo electronico valido",
                maxlength: "El correo no puede exceder los 100 caracteres"
            },
            contrasenna: {
                minlength: "Debe tener al menos 8 caracteres",
                maxlength: "No puede exceder los 100 caracteres"
            },
            idRol: {
                required: "Por favor seleccione un rol"
            },
            activo: {
                required: "Por favor seleccione un estado"
            }
        },
        errorElement: 'span',
        errorPlacement: function (error, element) {
            error.addClass('invalid-feedback');
            element.closest('.form-group').append(error);
        },
        highlight: function (element, errorClass, validClass) {
            $(element).addClass('is-invalid');
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).removeClass('is-invalid');
        },
        submitHandler: function (form) {
            const idUsuario = $('#id_usuario').val();
            const action = idUsuario === '0' ? 'registrar' : 'actualizar';

            showConfirm(
                `Confirmar ${action} empleado`,
                `Confirma estar seguro de desear ${action} este empleado`,
                function () {
                    submitForm();
                }
            );
        }
    });

    // Función para enviar el formulario
    function submitForm() {
        const formData = $('#formEmpleado').serialize();
        const idUsuario = $('#id_usuario').val();
        const url = idUsuario === '0' ? '/Empleados/RegistrarEmpleado' : '/Empleados/ActualizarEmpleado'; 
        const btnGuardar = $('#btnGuardar');

        btnGuardar.prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Procesando...');

        $.post(url, formData)
            .done(function () {
                showAlert('success', 'Exito', 'Operacion realizada correctamente');
                setTimeout(() => {
                    window.location.reload();
                }, 1500);
            })
            .fail(function (xhr) {
                showAlert('error', 'Error', xhr.responseText || 'Ocurrio un error al procesar la solicitud');
            })
            .always(function () {
                btnGuardar.prop('disabled', false).html('Guardar');
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