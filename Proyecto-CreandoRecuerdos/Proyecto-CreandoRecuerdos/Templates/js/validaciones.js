$(function () {
    // Mensajes globales
    const mensajes = {
        required: "Este campo es obligatorio",
        email: "Correo electrónico inválido",
        minlength: $.validator.format("Debe tener al menos {0} caracteres"),
        maxlength: $.validator.format("No puede exceder los {0} caracteres"),
        digits: "Solo se permiten dígitos",
        number: "Debe ser un número válido",
        pattern: "Formato inválido",
        unique: "Este valor ya existe",
        unidades: "Unidad no permitida",
        duplicado: "Ya existe un registro con estos datos",
        existencia: "No existe en la base de datos"
    };

    // Sobrescribe los mensajes por defecto
    $.extend($.validator.messages, mensajes);

    // Método para unidades permitidas
    $.validator.addMethod("unidadPermitida", function (value, element, param) {
        if (!value) return true;
        return param.includes(value.trim().toLowerCase());
    }, mensajes.unidades);

    // Método para duplicado exacto (requiere AJAX)
    $.validator.addMethod("duplicadoExacto", function (value, element, param) {
        let isUnique = true;
        $.ajax({
            url: param.url,
            type: 'GET',
            async: false,
            data: param.data,
            success: function (response) {
                isUnique = response.isUnique;
            }
        });
        return isUnique;
    }, mensajes.duplicado);

    // Método para existencia en BD
    $.validator.addMethod("existeEnBD", function (value, element, param) {
        let existe = false;
        $.ajax({
            url: param.url,
            type: 'GET',
            async: false,
            data: { valor: value },
            success: function (response) {
                existe = response.existe;
            }
        });
        return existe;
    }, mensajes.existencia);

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
    }, "Este correo electrónico ya está registrado");

    // Método personalizado para al menos un día seleccionado
    $.validator.addMethod("atLeastOneDay", function (value, element) {
        return $('input[name="dias_semana"]:checked').length > 0;
    }, "Por favor seleccione al menos un día");

    // Validación de unidades permitidas para Materia Prima y Producto Preparado
    $.validator.addMethod("unidadPresentacionPermitida", function (value, element) {
        if (!value) return true;
        const unidades = ["kg", "kilo", "kilos", "kilogramo", "kilogramos", "g", "gr", "grs", "gramo", "gramos", "l", "litro", "litros", "ml", "mililitro", "mililitros"];
        return unidades.includes(value.trim().toLowerCase());
    }, "Unidad de medida de presentación no permitida.");

    $.validator.addMethod("unidadPesoPermitida", function (value, element) {
        if (!value) return true;
        const unidades = ["g", "gr", "grs", "gramo", "gramos", "ml", "mililitro", "mililitros"];
        return unidades.includes(value.trim().toLowerCase());
    }, "Unidad de medida del peso no permitida.");

    // Validación de pluralidad/singularidad según volumen/cantidad
    $.validator.addMethod("pluralidadPresentacion", function (value, element, param) {
        const volumen = parseFloat($(param).val().replace(',', '.'));
        const plural = ["g", "grs", "gramos", "kilos", "kilogramos", "l", "litros", "ml", "mililitros"];
        const singular = ["g", "gr", "gramo", "kilo", "kilogramo", "l", "litro", "ml", "mililitro"];
        value = value.trim().toLowerCase();
        if (volumen > 0 && volumen !== 1) return plural.includes(value);
        if (volumen === 1) return singular.includes(value);
        return true;
    }, "La unidad de medida no corresponde al volumen ingresado.");

    // Validación de unidades para Empaque/Decoración
    $.validator.addMethod("unidadEmpaquePermitida", function (value, element) {
        if (!value) return true;
        const unidades = ["unidad", "unidades"];
        return unidades.includes(value.trim().toLowerCase());
    }, "Unidad de medida no permitida.");

    $.validator.addMethod("pluralidadEmpaque", function (value, element, param) {
        const cantidad = parseInt($(param).val());
        value = value.trim().toLowerCase();
        if (cantidad > 0 && cantidad !== 1) return value === "unidades";
        if (cantidad === 1) return value === "unidad";
        return true;
    }, "La unidad de medida no corresponde a la cantidad ingresada.");

    // Validación de unidades para Implemento
    $.validator.addMethod("unidadImplementoPermitida", function (value, element) {
        if (!value) return true;
        const unidades = ["unidad", "unidades"];
        return unidades.includes(value.trim().toLowerCase());
    }, "Unidad de medida no permitida.");

    $.validator.addMethod("pluralidadImplemento", function (value, element, param) {
        const cantidad = parseInt($(param).val());
        value = value.trim().toLowerCase();
        if (cantidad > 0 && cantidad !== 1) return value === "unidades";
        if (cantidad === 1) return value === "unidad";
        return true;
    }, "La unidad de medida no corresponde a la cantidad ingresada.");

    // Validación de unidades para Suministro
    $.validator.addMethod("unidadSuministroPermitida", function (value, element) {
        if (!value) return true;
        const unidades = ["unidad", "unidades"];
        return unidades.includes(value.trim().toLowerCase());
    }, "Unidad de medida no permitida.");

    $.validator.addMethod("pluralidadSuministro", function (value, element, param) {
        const cantidad = parseInt($(param).val());
        value = value.trim().toLowerCase();
        if (cantidad > 0 && cantidad !== 1) return value === "unidades";
        if (cantidad === 1) return value === "unidad";
        return true;
    }, "La unidad de medida no corresponde a la cantidad ingresada.");

    // Validación de duplicado exacto (requiere AJAX, ejemplo para Materia Prima)
    $.validator.addMethod("duplicadoMateriaPrima", function (value, element, param) {
        let isUnique = true;
        const form = $(element).closest('form');
        const data = {
            id: form.find('[name="id"]').val() || 0,
            nombre: form.find('[name="nombre"]').val().trim().toLowerCase(),
            marca: form.find('[name="marca"]').val().trim().toLowerCase(),
            presentacion: form.find('[name="presentacion"]').val().trim().toLowerCase(),
            cantidad: form.find('[name="cantidad"]').val(),
            volumen_de_porcion_de_presentacion: form.find('[name="volumen_de_porcion_de_presentacion"]').val(),
            unidad_de_medida_de_presentacion: form.find('[name="unidad_de_medida_de_presentacion"]').val().trim().toLowerCase(),
            proveedor: form.find('[name="proveedor"]').val().trim().toLowerCase(),
            costo: form.find('[name="costo"]').val(),
            unidad_de_medida_del_peso: form.find('[name="unidad_de_medida_del_peso"]').val().trim().toLowerCase()
        };
        $.ajax({
            url: param.url,
            type: 'GET',
            async: false,
            data: data,
            success: function (response) {
                isUnique = response.isUnique;
            }
        });
        return isUnique;
    }, "Ya existe una materia prima con los mismos datos.");

    // Validación de duplicado exacto (requiere AJAX, ejemplo para Producto Preparado)
    $.validator.addMethod("duplicadoProductoPreparado", function (value, element, param) {
        let isUnique = true;
        const form = $(element).closest('form');
        const data = {
            id: form.find('[name="id"]').val() || 0,
            tipo: form.find('[name="tipo"]').val().trim().toLowerCase(),
            nombre: form.find('[name="nombre"]').val().trim().toLowerCase(),
            marca: form.find('[name="marca"]').val().trim().toLowerCase(),
            presentacion: form.find('[name="presentacion"]').val().trim().toLowerCase(),
            cantidad: form.find('[name="cantidad"]').val(),
            volumen_de_porcion_de_presentacion: form.find('[name="volumen_de_porcion_de_presentacion"]').val(),
            unidad_de_medida_de_presentacion: form.find('[name="unidad_de_medida_de_presentacion"]').val().trim().toLowerCase(),
            proveedor: form.find('[name="proveedor"]').val().trim().toLowerCase(),
            costo: form.find('[name="costo"]').val(),
            unidad_de_medida_del_peso: form.find('[name="unidad_de_medida_del_peso"]').val().trim().toLowerCase()
        };
        $.ajax({
            url: param.url,
            type: 'GET',
            async: false,
            data: data,
            success: function (response) {
                isUnique = response.isUnique;
            }
        });
        return isUnique;
    }, "Ya existe un producto preparado con los mismos datos.");

    // Validación de duplicado exacto para Empaque/Decoración (requiere AJAX)
    $.validator.addMethod("duplicadoEmpaqueDecoracion", function (value, element, param) {
        let isUnique = true;
        // Recopila todos los campos relevantes del formulario
        const form = $(element).closest('form');
        const data = {
            id: form.find('[name="id"]').val() || 0,
            nombre: form.find('[name="nombre"]').val().trim().toLowerCase(),
            marca: form.find('[name="marca"]').val().trim().toLowerCase(),
            presentacion: form.find('[name="presentacion"]').val().trim().toLowerCase(),
            proveedor: form.find('[name="proveedor"]').val().trim().toLowerCase(),
            costo: form.find('[name="costo"]').val() || 0,
            cantidad: form.find('[name="cantidad"]').val() || 0,
            unidad_de_medida: form.find('[name="unidad_de_medida"]').val().trim().toLowerCase()
        };
        $.ajax({
            url: param.url,
            type: 'GET',
            async: false,
            data: data,
            success: function (response) {
                isUnique = response.isUnique;
            }
        });
        return isUnique;
    }, "Ya existe un(a) empaque o decoración con los mismos datos.");

    // Validación de duplicado exacto para Implemento (requiere AJAX)
    $.validator.addMethod("duplicadoImplemento", function (value, element, param) {
        let isUnique = true;
        // Recopila todos los campos relevantes del formulario
        const form = $(element).closest('form');
        const data = {
            id: form.find('[name="id"]').val() || 0,
            nombre: form.find('[name="nombre"]').val().trim().toLowerCase(),
            marca: form.find('[name="marca"]').val().trim().toLowerCase(),
            presentacion: form.find('[name="presentacion"]').val().trim().toLowerCase(),
            proveedor: form.find('[name="proveedor"]').val().trim().toLowerCase(),
            costo: form.find('[name="costo"]').val() || 0,
            cantidad: form.find('[name="cantidad"]').val() || 0,
            unidad_de_medida: form.find('[name="unidad_de_medida"]').val().trim().toLowerCase()
        };
        $.ajax({
            url: param.url,
            type: 'GET',
            async: false,
            data: data,
            success: function (response) {
                isUnique = response.isUnique;
            }
        });
        return isUnique;
    }, "Ya existe un implemento con los mismos datos.");

    // Validación de duplicado exacto para Suministro (requiere AJAX)
    $.validator.addMethod("duplicadoSuministro", function (value, element, param) {
        let isUnique = true;
        // Recopila todos los campos relevantes del formulario
        const form = $(element).closest('form');
        const data = {
            id: form.find('[name="id"]').val() || 0,
            nombre: form.find('[name="nombre"]').val().trim().toLowerCase(),
            marca: form.find('[name="marca"]').val().trim().toLowerCase(),
            presentacion: form.find('[name="presentacion"]').val().trim().toLowerCase(),
            proveedor: form.find('[name="proveedor"]').val().trim().toLowerCase(),
            costo: form.find('[name="costo"]').val() || 0,
            cantidad: form.find('[name="cantidad"]').val() || 0,
            unidad_de_medida: form.find('[name="unidad_de_medida"]').val().trim().toLowerCase()
        };
        $.ajax({
            url: param.url,
            type: 'GET',
            async: false,
            data: data,
            success: function (response) {
                isUnique = response.isUnique;
            }
        });
        return isUnique;
    }, "Ya existe un suministro con los mismos datos.");

    // Validación de duplicado exacto para CostosRecetas (requiere AJAX)
    $.validator.addMethod("duplicadoReceta", function (value, element, param) {
        let isUnique = true;
        const form = $(element).closest('form');
        const data = {
            id: form.find('[name="id"]').val() || 0,
            nombre: value.trim().toLowerCase()
        };
        $.ajax({
            url: param.url,
            type: 'GET',
            async: false,
            data: data,
            success: function (response) {
                isUnique = response.isUnique;
            }
        });
        return isUnique;
    }, "Ya existe una receta con ese nombre.");

    // Validación de unidad permitida para insumos de receta
    $.validator.addMethod("unidadRecetaPermitida", function (value, element) {
        if (!value) return true;
        const unidades = ["g", "gr", "grs", "gramo", "gramos", "kg", "kilo", "kilos", "kilogramo", "kilogramos", "ml", "mililitro", "mililitros", "l", "litro", "litros"];
        return unidades.includes(value.trim().toLowerCase());
    }, "Unidad de medida no permitida.");

    $.validator.addMethod("duplicadoProductoFinal", function (value, element, param) {
        let isUnique = true;
        const form = $(element).closest('form');
        const data = {
            id: form.find('[name="id"]').val() || 0,
            nombre_receta: form.find('[name="nombre_receta"]').val().trim().toLowerCase()
        };
        $.ajax({
            url: param.url,
            type: 'GET',
            async: false,
            data: data,
            success: function (response) {
                isUnique = response.isUnique;
            }
        });
        return isUnique;
    }, "Ya existe un producto final para esa receta.");

    $("#loginForm").validate({
        rules: {
            correo: {
                required: true,
                email: true
            },
            contrasenna: {
                required: true,
                minlength: 5
            }
        },
        messages: {
            correo: {
                required: "Por favor ingrese su correo electrónico",
                email: "Por favor ingrese un correo electrónico válido"
            },
            contrasenna: {
                required: "Por favor ingrese su contraseña",
                minlength: "La contraseña debe tener al menos 5 caracteres"
            }
        },
        errorElement: 'span',
        errorClass: 'invalid-feedback',
        highlight: function (element) {
            $(element).addClass('is-invalid');
        },
        unhighlight: function (element) {
            $(element).removeClass('is-invalid');
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        }
    });

    $("#registerForm").validate({
        rules: {
            nombre: {
                required: true
            },
            correo: {
                required: true,
                email: true
            },
            password: {
                required: true,
                minlength: 8
            },
            contrasenna: {
                required: true,
                minlength: 8,
                equalTo: "#password"
            }
        },
        messages: {
            nombre: {
                required: "Por favor ingrese su nombre completo"
            },
            correo: {
                required: "Por favor ingrese su correo electrónico",
                email: "Por favor ingrese un correo electrónico válido"
            },
            password: {
                required: "Por favor ingrese una contraseña",
                minlength: "La contraseña debe tener al menos 8 caracteres"
            },
            contrasenna: {
                required: "Por favor confirme su contraseña",
                minlength: "La contraseña debe tener al menos 8 caracteres",
                equalTo: "Las contraseñas no coinciden"
            }
        },
        errorElement: 'span',
        errorClass: 'invalid-feedback',
        highlight: function (element) {
            $(element).addClass('is-invalid');
        },
        unhighlight: function (element) {
            $(element).removeClass('is-invalid');
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        }
    });

    // Inicializa validación en todos los formularios
    $('form.needs-validation').each(function () {
        // Materias Primas
        if ($("#formMateriaPrima").length) {
            $("#formMateriaPrima").validate({

                rules: {
                    nombre: { required: true, duplicadoMateriaPrima: { url: "/Insumos/VerificarDuplicadoMateriaPrima" } },
                    marca: { required: true, duplicadoMateriaPrima: { url: "/Insumos/VerificarDuplicadoMateriaPrima" } },
                    presentacion: { required: true, duplicadoMateriaPrima: { url: "/Insumos/VerificarDuplicadoMateriaPrima" } },
                    cantidad: { required: true, min: 1, duplicadoMateriaPrima: { url: "/Insumos/VerificarDuplicadoMateriaPrima" } },
                    volumen_de_porcion_de_presentacion: { required: true, min: 0.01, duplicadoMateriaPrima: { url: "/Insumos/VerificarDuplicadoMateriaPrima" } },
                    unidad_de_medida_de_presentacion: {
                        required: true,
                        unidadPresentacionPermitida: true,
                        pluralidadPresentacion: "#volumen_de_porcion_de_presentacion",
                        duplicadoMateriaPrima: { url: "/Insumos/VerificarDuplicadoMateriaPrima" }
                    },
                    proveedor: { required: true, duplicadoMateriaPrima: { url: "/Insumos/VerificarDuplicadoMateriaPrima" } },
                    costo: { required: true, min: 1, duplicadoMateriaPrima: { url: "/Insumos/VerificarDuplicadoMateriaPrima" } },
                    unidad_de_medida_del_peso: {
                        required: true,
                        unidadPesoPermitida: true,
                        duplicadoMateriaPrima: { url: "/Insumos/VerificarDuplicadoMateriaPrima" }
                    },
                    merma_total_en_gramos: { required: true, min: 0, duplicadoMateriaPrima: { url: "/Insumos/VerificarDuplicadoMateriaPrima" } }
                },
                messages: {
                    nombre: { required: "El nombre es obligatorio" },
                    marca: { required: "La marca es obligatoria" },
                    presentacion: { required: "La presentación es obligatoria" },
                    cantidad: { required: "La cantidad es obligatoria", min: "Debe ser mayor a 0" },
                    volumen_de_porcion_de_presentacion: { required: "El volumen es obligatorio", min: "Debe ser mayor a 0" },
                    unidad_de_medida_de_presentacion: { required: "La unidad de medida de presentación es obligatoria" },
                    proveedor: { required: "El proveedor es obligatorio" },
                    costo: { required: "El costo es obligatorio", min: "Debe ser mayor a ₡0.99" },
                    unidad_de_medida_del_peso: { required: "La unidad de medida del peso es obligatoria" },
                    merma_total_en_gramos: { required: "La merma es obligatoria", min: "No puede ser negativa" }
                },
                errorElement: 'span',
                errorClass: 'invalid-feedback',
                highlight: function (element) { $(element).addClass('is-invalid'); },
                unhighlight: function (element) { $(element).removeClass('is-invalid'); },
                errorPlacement: function (error, element) {
                    if (element.parent().hasClass('input-group')) {
                        error.insertAfter(element.parent());
                    } // Para los demás, comportamiento estándar
                    else {
                        error.insertAfter(element);
                    }
                },
                invalidHandler: function (event, validator) {
                    var duplicado = validator.errorList.some(function (error) {
                        return error.message.indexOf("Ya existe una materia prima con los mismos datos") !== -1;
                    });
                    if (duplicado) {
                        $("#mensajeDuplicadoGlobalMateriaPrima").show().text("Ya existe una materia prima con los mismos datos.");
                    } else {
                        $("#mensajeDuplicadoGlobalMateriaPrima").hide();
                    }
                }
            });
            return;
        }

        // Productos Preparados
        if ($("#formProductoPreparado").length) {
            $("#formProductoPreparado").validate({
                rules: {
                    tipo: {
                        required: true,
                        duplicadoProductoPreparado: {
                            url: "/Insumos/VerificarDuplicadoProductoPreparado"
                        }
                    },
                    nombre: {
                        required: true,
                        duplicadoProductoPreparado: {
                            url: "/Insumos/VerificarDuplicadoProductoPreparado"
                        }
                    },
                    marca: {
                        required: true,
                        duplicadoProductoPreparado: {
                            url: "/Insumos/VerificarDuplicadoProductoPreparado"
                        }
                    },
                    presentacion: {
                        required: true,
                        duplicadoProductoPreparado: {
                            url: "/Insumos/VerificarDuplicadoProductoPreparado"
                        }
                    },
                    cantidad: {
                        required: true, min: 1,
                        duplicadoProductoPreparado: {
                            url: "/Insumos/VerificarDuplicadoProductoPreparado"
                        }
                    },
                    volumen_de_porcion_de_presentacion: {
                        required: true, min: 0.01,
                        duplicadoProductoPreparado: {
                            url: "/Insumos/VerificarDuplicadoProductoPreparado"
                        }
                    },
                    unidad_de_medida_de_presentacion: {
                        required: true,
                        unidadPresentacionPermitida: true,
                        pluralidadPresentacion: "#volumen_de_porcion_de_presentacion",
                        duplicadoProductoPreparado: {
                            url: "/Insumos/VerificarDuplicadoProductoPreparado"
                        }
                    },
                    proveedor: {
                        required: true,
                        duplicadoProductoPreparado: {
                            url: "/Insumos/VerificarDuplicadoProductoPreparado"
                        }
                    },
                    costo: {
                        required: true, min: 1,
                        duplicadoProductoPreparado: {
                            url: "/Insumos/VerificarDuplicadoProductoPreparado"
                        }
                    },
                    unidad_de_medida_del_peso: {
                        required: true,
                        unidadPesoPermitida: true,
                        duplicadoProductoPreparado: {
                            url: "/Insumos/VerificarDuplicadoProductoPreparado"
                        }
                    }
                },
                messages: {
                    tipo: { required: "El tipo es obligatorio" },
                    nombre: { required: "El nombre es obligatorio" },
                    marca: { required: "La marca es obligatoria" },
                    presentacion: { required: "La presentación es obligatoria" },
                    cantidad: { required: "La cantidad es obligatoria", min: "Debe ser mayor a 0" },
                    volumen_de_porcion_de_presentacion: { required: "El volumen es obligatorio", min: "Debe ser mayor a 0" },
                    unidad_de_medida_de_presentacion: {
                        required: "La unidad de medida de presentación es obligatoria"
                    },
                    proveedor: { required: "El proveedor es obligatorio" },
                    costo: { required: "El costo es obligatorio", min: "Debe ser mayor a ₡0.99" },
                    unidad_de_medida_del_peso: {
                        required: "La unidad de medida del peso es obligatoria"
                    }
                },
                errorElement: 'span',
                errorClass: 'invalid-feedback',
                highlight: function (element) { $(element).addClass('is-invalid'); },
                unhighlight: function (element) { $(element).removeClass('is-invalid'); },
                errorPlacement: function (error, element) {
                    if (element.parent().hasClass('input-group')) {
                        error.insertAfter(element.parent());
                    } // Para los demás, comportamiento estándar
                    else {
                        error.insertAfter(element);
                    }
                },
                invalidHandler: function (event, validator) {
                    var duplicado = validator.errorList.some(function (error) {
                        return error.message.indexOf("Ya existe un producto preparado con los mismos datos") !== -1;
                    });
                    if (duplicado) {
                        $("#mensajeDuplicadoGlobalProductoPreparado").show().text("Ya existe un producto preparado con los mismos datos.");
                    } else {
                        $("#mensajeDuplicadoGlobalProductoPreparado").hide();
                    }
                }
            });
            return;
        }

        // Empaques y/o Decoraciones
        if ($("#formEmpaqueDecoracion").length) {
            $("#formEmpaqueDecoracion").validate({
                rules: {
                    nombre: {
                        required: true,
                        duplicadoEmpaqueDecoracion: {
                            url: "/Insumos/VerificarDuplicadoEmpaqueDecoracion"
                        }
                    },
                    marca: {
                        required: true,
                        duplicadoEmpaqueDecoracion: {
                            url: "/Insumos/VerificarDuplicadoEmpaqueDecoracion"
                        }
                    },
                    presentacion: {
                        required: true,
                        duplicadoEmpaqueDecoracion: {
                            url: "/Insumos/VerificarDuplicadoEmpaqueDecoracion"
                        }
                    },
                    proveedor: {
                        required: true,
                        duplicadoEmpaqueDecoracion: {
                            url: "/Insumos/VerificarDuplicadoEmpaqueDecoracion"
                        }
                    },
                    costo: {
                        required: true, min: 1,
                        duplicadoEmpaqueDecoracion: {
                            url: "/Insumos/VerificarDuplicadoEmpaqueDecoracion"
                        }
                    },
                    cantidad: {
                        required: true, min: 1,
                        duplicadoEmpaqueDecoracion: {
                            url: "/Insumos/VerificarDuplicadoEmpaqueDecoracion"
                        }
                    },
                    unidad_de_medida: {
                        required: true,
                        unidadEmpaquePermitida: true,
                        pluralidadEmpaque: "#cantidad",
                        duplicadoEmpaqueDecoracion: {
                            url: "/Insumos/VerificarDuplicadoEmpaqueDecoracion"
                        }
                    }
                },
                messages: {
                    nombre: { required: "El nombre es obligatorio" },
                    marca: { required: "La marca es obligatoria" },
                    presentacion: { required: "La presentación es obligatoria" },
                    proveedor: { required: "El proveedor es obligatorio" },
                    costo: { required: "El costo es obligatorio", min: "Debe ser mayor a ₡0.99" },
                    cantidad: { required: "La cantidad es obligatoria", min: "Debe ser mayor a 0" },
                    unidad_de_medida: {
                        required: "La unidad de medida es obligatoria"
                    }
                },
                errorElement: 'span',
                errorClass: 'invalid-feedback',
                highlight: function (element) { $(element).addClass('is-invalid'); },
                unhighlight: function (element) { $(element).removeClass('is-invalid'); },
                errorPlacement: function (error, element) {
                    if (element.parent().hasClass('input-group')) {
                        error.insertAfter(element.parent());
                    } // Para los demás, comportamiento estándar
                    else {
                        error.insertAfter(element);
                    }
                },
                invalidHandler: function (event, validator) {
                    var duplicado = validator.errorList.some(function (error) {
                        return error.message.indexOf("Ya existe un(a) empaque o decoración con los mismos datos") !== -1;
                    });
                    if (duplicado) {
                        $("#mensajeDuplicadoGlobalEmpaqueDecoracion").show().text("Ya existe un(a) empaque o decoración con los mismos datos.");
                    } else {
                        $("#mensajeDuplicadoGlobalEmpaqueDecoracion").hide();
                    }
                }
            });
            return;
        }

        // Implementos
        if ($("#formImplemento").length) {
            $("#formImplemento").validate({
                rules: {
                    nombre: {
                        required: true,
                        duplicadoImplemento: {
                            url: "/Insumos/VerificarDuplicadoImplemento"
                        }
                    },
                    marca: {
                        required: true,
                        duplicadoImplemento: {
                            url: "/Insumos/VerificarDuplicadoImplemento"
                        }
                    },
                    presentacion: {
                        required: true,
                        duplicadoImplemento: {
                            url: "/Insumos/VerificarDuplicadoImplemento"
                        }
                    },
                    proveedor: {
                        required: true,
                        duplicadoImplemento: {
                            url: "/Insumos/VerificarDuplicadoImplemento"
                        }
                    },
                    costo: {
                        required: true, min: 1,
                        duplicadoImplemento: {
                            url: "/Insumos/VerificarDuplicadoImplemento"
                        }
                    },
                    cantidad: {
                        required: true, min: 1,
                        duplicadoImplemento: {
                            url: "/Insumos/VerificarDuplicadoImplemento"
                        }
                    },
                    unidad_de_medida: {
                        required: true,
                        unidadEmpaquePermitida: true,
                        pluralidadEmpaque: "#cantidad",
                        duplicadoImplemento: {
                            url: "/Insumos/VerificarDuplicadoImplemento"
                        }
                    }
                },
                messages: {
                    nombre: { required: "El nombre es obligatorio" },
                    marca: { required: "La marca es obligatoria" },
                    presentacion: { required: "La presentación es obligatoria" },
                    proveedor: { required: "El proveedor es obligatorio" },
                    costo: { required: "El costo es obligatorio", min: "Debe ser mayor a ₡0.99" },
                    cantidad: { required: "La cantidad es obligatoria", min: "Debe ser mayor a 0" },
                    unidad_de_medida: {
                        required: "La unidad de medida es obligatoria"
                    }
                },
                errorElement: 'span',
                errorClass: 'invalid-feedback',
                highlight: function (element) { $(element).addClass('is-invalid'); },
                unhighlight: function (element) { $(element).removeClass('is-invalid'); },
                errorPlacement: function (error, element) {
                    if (element.parent().hasClass('input-group')) {
                        error.insertAfter(element.parent());
                    } // Para los demás, comportamiento estándar
                    else {
                        error.insertAfter(element);
                    }
                },
                invalidHandler: function (event, validator) {
                    var duplicado = validator.errorList.some(function (error) {
                        return error.message.indexOf("Ya existe un implemento con los mismos datos") !== -1;
                    });
                    if (duplicado) {
                        $("#mensajeDuplicadoGlobalImplemento").show().text("Ya existe un implemento con los mismos datos.");
                    } else {
                        $("#mensajeDuplicadoGlobalImplemento").hide();
                    }
                }
            });
            return;
        }

        // Suministros
        if ($("#formSuministro").length) {
            $("#formSuministro").validate({
                rules: {
                    nombre: {
                        required: true,
                        duplicadoSuministro: {
                            url: "/Insumos/VerificarDuplicadoSuministro"
                        }
                    },
                    marca: {
                        required: true,
                        duplicadoSuministro: {
                            url: "/Insumos/VerificarDuplicadoSuministro"
                        }
                    },
                    presentacion: {
                        required: true,
                        duplicadoSuministro: {
                            url: "/Insumos/VerificarDuplicadoSuministro"
                        }
                    },
                    proveedor: {
                        required: true,
                        duplicadoSuministro: {
                            url: "/Insumos/VerificarDuplicadoSuministro"
                        }
                    },
                    costo: {
                        required: true, min: 1,
                        duplicadoSuministro: {
                            url: "/Insumos/VerificarDuplicadoSuministro"
                        }
                    },
                    cantidad: {
                        required: true, min: 1,
                        duplicadoSuministro: {
                            url: "/Insumos/VerificarDuplicadoSuministro"
                        }
                    },
                    unidad_de_medida: {
                        required: true,
                        unidadEmpaquePermitida: true,
                        pluralidadEmpaque: "#cantidad",
                        duplicadoSuministro: {
                            url: "/Insumos/VerificarDuplicadoSuministro"
                        }
                    }
                },
                messages: {
                    nombre: { required: "El nombre es obligatorio" },
                    marca: { required: "La marca es obligatoria" },
                    presentacion: { required: "La presentación es obligatoria" },
                    proveedor: { required: "El proveedor es obligatorio" },
                    costo: { required: "El costo es obligatorio", min: "Debe ser mayor a ₡0.99" },
                    cantidad: { required: "La cantidad es obligatoria", min: "Debe ser mayor a 0" },
                    unidad_de_medida: {
                        required: "La unidad de medida es obligatoria"
                    }
                },
                errorElement: 'span',
                errorClass: 'invalid-feedback',
                highlight: function (element) { $(element).addClass('is-invalid'); },
                unhighlight: function (element) { $(element).removeClass('is-invalid'); },
                errorPlacement: function (error, element) {
                    if (element.parent().hasClass('input-group')) {
                        error.insertAfter(element.parent());
                    } // Para los demás, comportamiento estándar
                    else {
                        error.insertAfter(element);
                    }
                },
                invalidHandler: function (event, validator) {
                    var duplicado = validator.errorList.some(function (error) {
                        return error.message.indexOf("Ya existe un suministro con los mismos datos") !== -1;
                    });
                    if (duplicado) {
                        $("#mensajeDuplicadoGlobalSuministro").show().text("Ya existe un suministro con los mismos datos.");
                    } else {
                        $("#mensajeDuplicadoGlobalSuministro").hide();
                    }
                }
            });
            return;
        }

        // Costos de Recetas
        if ($("#formCostosRecetas").length) {
            $("#formCostosRecetas").validate({
                rules: {
                    nombre: {
                        required: true,
                        duplicadoReceta: { url: "/Insumos/VerificarDuplicadoReceta" }
                    },
                    porcion: { required: true, min: 1 }
                },
                messages: {
                    nombre: {
                        required: "El nombre de la receta es obligatorio"
                    },
                    porcion: {
                        required: "La porción es obligatoria",
                        min: "Debe ser mayor a cero"
                    }
                },
                errorElement: 'span',
                errorClass: 'invalid-feedback',
                highlight: function (element) { $(element).addClass('is-invalid'); },
                unhighlight: function (element) { $(element).removeClass('is-invalid'); },
                errorPlacement: function (error, element) {
                    // Si el elemento está dentro de un input-group, inserta el error después del grupo
                    if (element.parent().hasClass('input-group')) {
                        error.insertAfter(element.parent());
                    }
                    // Si es un select oculto dentro de .custom-select, pon el error después del botón visual
                    else if (element.is('select') && element.hasClass('custom-hidden')) {
                        var $customButton = element.closest('.custom-select').find('.custom-select-button');
                        error.insertAfter($customButton);
                    }
                    // Si es un select normal, pon el error después del select
                    else if (element.is('select')) {
                        error.insertAfter(element);
                    }
                    // Para los demás, comportamiento estándar
                    else {
                        error.insertAfter(element);
                    }
                },
                invalidHandler: function (event, validator) {
                    var duplicado = validator.errorList.some(function (error) {
                        return error.message.indexOf("Ya existe una receta con ese nombre") !== -1;
                    });
                    if (duplicado) {
                        $("#mensajeDuplicadoGlobalReceta").show().text("Ya existe una receta con ese nombre.");
                    } else {
                        $("#mensajeDuplicadoGlobalReceta").hide();
                    }
                }
            });

        }

        // Validación para Precios Finales Sugeridos
        if ($("#formPreciosFinalesSugeridos").length) {
            $("#formPreciosFinalesSugeridos").validate({
                rules: {
                    id_receta: {
                        required: true,
                        min: 1,
                        duplicadoProductoFinal: {
                            url: "/Insumos/VerificarDuplicadoProductoFinal"
                        }
                    },
                    margen_de_utilidad: { required: true, min: 0, max: 100 },
                    plataforma_de_envio: { required: true },
                    porcentaje_de_iva: { required: true, min: 0, max: 100 },
                    porcentaje_de_servicio: { required: true, min: 0, max: 100 }
                },
                messages: {
                    id_receta: { required: "Debe seleccionar una receta", min: "Seleccione una receta válida" },
                    margen_de_utilidad: { required: "El margen de utilidad es obligatorio", min: "No puede ser negativo", max: "No puede ser mayor a 100" },
                    plataforma_de_envio: { required: "Seleccione una plataforma de envío" },
                    porcentaje_de_iva: { required: "El porcentaje de IVA es obligatorio", min: "No puede ser negativo", max: "No puede ser mayor a 100" },
                    porcentaje_de_servicio: { required: "El porcentaje de servicio es obligatorio", min: "No puede ser negativo", max: "No puede ser mayor a 100" }
                },
                errorElement: 'span',
                errorClass: 'invalid-feedback',
                highlight: function (element) { $(element).addClass('is-invalid'); },
                unhighlight: function (element) { $(element).removeClass('is-invalid'); },
                errorPlacement: function (error, element) {
                    // Si el elemento está dentro de un input-group, inserta el error después del grupo
                    if (element.parent().hasClass('input-group')) {
                        error.insertAfter(element.parent());
                    }
                    // Si es un select oculto dentro de .custom-select, pon el error después del botón visual
                    else if (element.is('select') && element.hasClass('custom-hidden')) {
                        var $customButton = element.closest('.custom-select').find('.custom-select-button');
                        error.insertAfter($customButton);
                    }
                    // Si es un select normal, pon el error después del select
                    else if (element.is('select')) {
                        error.insertAfter(element);
                    }
                    // Para los demás, comportamiento estándar
                    else {
                        error.insertAfter(element);
                    }
                },
                invalidHandler: function (event, validator) {
                    var duplicado = validator.errorList.some(function (error) {
                        return error.message.indexOf("Ya existe un producto final para esa receta.") !== -1;
                    });
                    if (duplicado) {
                        $("#mensajeDuplicadoGlobalProductoFinal").show().text("Ya existe un producto final para esa receta.");
                    } else {
                        $("#mensajeDuplicadoGlobalProductoFinal").hide();
                    }
                }
            });
        }

        if (this.id === "formEmpleado") {
            $(this).validate({
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
                        required: "Por favor ingrese el correo electrónico",
                        email: "Por favor ingrese un correo electrónico válido",
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
                errorClass: 'invalid-feedback',
                errorPlacement: function (error, element) {
                    // Si el elemento está dentro de un input-group, inserta el error después del grupo
                    if (element.parent().hasClass('input-group')) {
                        error.insertAfter(element.parent());
                    }
                    // Si es un select oculto dentro de .custom-select, pon el error después del botón visual
                    else if (element.is('select') && element.hasClass('custom-hidden')) {
                        var $customButton = element.closest('.custom-select').find('.custom-select-button');
                        error.insertAfter($customButton);
                    }
                    // Si es un select normal, pon el error después del select
                    else if (element.is('select')) {
                        error.insertAfter(element);
                    }
                    // Para los demás, comportamiento estándar
                    else {
                        error.insertAfter(element);
                    }
                },
                highlight: function (element) {
                    $(element).addClass('is-invalid');
                },
                unhighlight: function (element) {
                    $(element).removeClass('is-invalid');
                },
                submitHandler: function (form) {
                    const idUsuario = $('#id_usuario').val();
                    const action = idUsuario === '0' ? 'registrar' : 'actualizar';

                    Swal.fire({
                        title: `Confirmar ${action} empleado`,
                        text: `Confirma estar seguro de desear ${action} este empleado`,
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#B54885',
                        cancelButtonColor: '#6C757D',
                        confirmButtonText: 'Si, continuar',
                        cancelButtonText: 'Cancelar'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            submitForm();
                        }
                    });

                    function submitForm() {
                        const formData = $('#formEmpleado').serialize();
                        const url = idUsuario === '0' ? '/Empleados/RegistrarEmpleado' : '/Empleados/ActualizarEmpleado';
                        const btnGuardar = $('#btnGuardar');

                        btnGuardar.prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Procesando...');

                        $.post(url, formData)
                            .done(function () {
                                Swal.fire('Exito', 'Operacion realizada correctamente', 'success');
                                setTimeout(() => {
                                    window.location.reload();
                                }, 1500);
                            })
                            .fail(function (xhr) {
                                Swal.fire('Error', xhr.responseText || 'Ocurrio un error al procesar la solicitud', 'error');
                            })
                            .always(function () {
                                btnGuardar.prop('disabled', false).html('Guardar');
                            });
                    }
                }
            });
        }

        if (this.id === "horarioForm") {
            $(this).validate({
                rules: {
                    id_usuario: { required: true },
                    'dias_semana': { atLeastOneDay: true },
                    hora_entrada: { required: true },
                    hora_salida: { required: true }
                },
                messages: {
                    id_usuario: { required: "Por favor seleccione un empleado" },
                    'dias_semana': { atLeastOneDay: "Por favor seleccione al menos un día" },
                    hora_entrada: { required: "Por favor ingrese la hora de entrada" },
                    hora_salida: { required: "Por favor ingrese la hora de salida" }
                },
                errorElement: 'span',
                errorPlacement: function (error, element) {
                    error.addClass('invalid-feedback');
                    if (element.attr('name') === 'dias_semana') {
                        $('#diasSemanaError').text(error.text()).show();
                        $('#diasSemanaContainer').addClass('is-invalid');
                        // Si es un select oculto dentro de .custom-select, pon el error después del botón visual
                    } else if (element.is('select') && element.hasClass('custom-hidden')) {
                        var $customButton = element.closest('.custom-select').find('.custom-select-button');
                        error.insertAfter($customButton);
                    }
                    // Si es un select normal, pon el error después del select
                    else if (element.is('select')) {
                        error.insertAfter(element);
                    }
                    // Para los demás, comportamiento estándar
                    else {
                        error.insertAfter(element);
                    }
                },
                highlight: function (element) {
                    $(element).addClass('is-invalid');
                    if ($(element).attr('name') === 'dias_semana') {
                        $('#diasSemanaContainer').addClass('is-invalid');
                    }
                },
                unhighlight: function (element) {
                    $(element).removeClass('is-invalid');
                    if ($(element).attr('name') === 'dias_semana') {
                        $('#diasSemanaError').hide();
                        $('#diasSemanaContainer').removeClass('is-invalid');
                    }
                },
                submitHandler: function (form) {
                    Swal.fire({
                        title: '¿Guardar horario?',
                        text: '¿Confirma que desea guardar este horario?',
                        icon: 'question',
                        showCancelButton: true,
                        confirmButtonColor: '#B54885',
                        cancelButtonColor: '#6C757D',
                        confirmButtonText: 'Sí, guardar',
                        cancelButtonText: 'Cancelar'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            form.submit();
                        }
                    });
                }
            });

            // Validar dinámicamente al cambiar los días
            $('input[name="dias_semana"]').on('change', function () {
                $('#horarioForm').validate().element($('input[name="dias_semana"]').first());
            });
        }

        // Reglas específicas para el formulario de pago
        if (this.id === "formPago") {
            $(this).validate({
                rules: {
                    telefonoSinpe: {
                        required: function () {
                            // Solo requerido si está visible (SINPE tab activo)
                            return $('#sinpe').hasClass('show') && $('#sinpe').hasClass('active');
                        },
                        pattern: /^[0-9]{8}$/
                    }
                },
                messages: {
                    telefonoSinpe: {
                        required: "El teléfono SINPE es obligatorio",
                        pattern: "Por favor ingrese un número de teléfono válido (8 dígitos)"
                    }
                },
                errorElement: 'span',
                errorClass: 'invalid-feedback',
                highlight: function (element) {
                    $(element).addClass('is-invalid');
                },
                unhighlight: function (element) {
                    $(element).removeClass('is-invalid');
                },
                errorPlacement: function (error, element) {
                    error.insertAfter(element);
                }
            });
        } else {
            // Validación genérica para otros formularios
            $(this).validate({
                errorElement: 'span',
                errorClass: 'invalid-feedback',
                highlight: function (element) {
                    $(element).addClass('is-invalid');
                },
                unhighlight: function (element) {
                    $(element).removeClass('is-invalid');
                },
                errorPlacement: function (error, element) {
                    // Si el elemento está dentro de un input-group, inserta el error después del grupo
                    if (element.parent().hasClass('input-group')) {
                        error.insertAfter(element.parent());
                    }
                    // Si es un select oculto dentro de .custom-select, pon el error después del botón visual
                    else if (element.is('select') && element.hasClass('custom-hidden')) {
                        var $customButton = element.closest('.custom-select').find('.custom-select-button');
                        error.insertAfter($customButton);
                    }
                    // Si es un select normal, pon el error después del select
                    else if (element.is('select')) {
                        error.insertAfter(element);
                    }
                    // Para los demás, comportamiento estándar
                    else {
                        error.insertAfter(element);
                    }
                }
            });
        }
    });
});

function aplicarValidacionFilaReceta($fila, index, tipo) {
    if (tipo === "materia") {
        $fila.find('[name^="MateriasPrimasUtilizadas"][name$=".id_materia_prima_utilizada"]').rules('add', {
            required: true,
            messages: { required: `Fila ${index + 1}: Debe seleccionar una materia prima.` }
        });
        $fila.find('[name^="MateriasPrimasUtilizadas"][name$=".cantidad"]').rules('add', {
            required: true,
            min: 1,
            messages: {
                required: `Fila ${index + 1}: La cantidad es obligatoria.`,
                min: `Fila ${index + 1}: La cantidad debe ser mayor a cero.`
            }
        });
        $fila.find('[name^="MateriasPrimasUtilizadas"][name$=".unidad_de_medida"]').rules('add', {
            required: true,
            unidadRecetaPermitida: true,
            messages: {
                required: `Fila ${index + 1}: La unidad de medida es obligatoria.`,
                unidadRecetaPermitida: `Fila ${index + 1}: Unidad de medida no permitida.`
            }
        });
    } else if (tipo === "producto") {
        $fila.find('[name^="ProductosPreparadosUtilizados"][name$=".id_producto_preparado_utilizado"]').rules('add', {
            required: true,
            messages: { required: `Fila ${index + 1}: Debe seleccionar un producto preparado.` }
        });
        $fila.find('[name^="ProductosPreparadosUtilizados"][name$=".cantidad"]').rules('add', {
            required: true,
            min: 1,
            messages: {
                required: `Fila ${index + 1}: La cantidad es obligatoria.`,
                min: `Fila ${index + 1}: La cantidad debe ser mayor a cero.`
            }
        });
        $fila.find('[name^="ProductosPreparadosUtilizados"][name$=".unidad_de_medida"]').rules('add', {
            required: true,
            unidadRecetaPermitida: true,
            messages: {
                required: `Fila ${index + 1}: La unidad de medida es obligatoria.`,
                unidadRecetaPermitida: `Fila ${index + 1}: Unidad de medida no permitida.`
            }
        });
    }
}

function aplicarValidacionFilaProductoFinal($fila, index, tipo) {
    if (tipo === "empaque") {
        $fila.find('[name$=".id_empaque_decoracion_utilizado"]').rules('add', {
            required: true,
            messages: { required: `Fila ${index + 1}: Debe seleccionar un empaque/decoración.` }
        });
        $fila.find('[name$=".cantidad"]').rules('add', {
            required: true,
            min: 1,
            messages: {
                required: `Fila ${index + 1}: La cantidad es obligatoria.`,
                min: `Fila ${index + 1}: La cantidad debe ser mayor a cero.`
            }
        });
        $fila.find('[name$=".unidad_de_medida"]').rules('add', {
            required: true,
            unidadEmpaquePermitida: true,
            pluralidadEmpaque: $fila.find('[name$=".cantidad"]'),
            messages: {
                required: `Fila ${index + 1}: La unidad de medida es obligatoria.`,
                unidadEmpaquePermitida: `Fila ${index + 1}: Unidad de medida no permitida.`,
                pluralidadEmpaque: `Fila ${index + 1}: La unidad de medida no corresponde a la cantidad.`
            }
        });
    } else if (tipo === "implemento") {
        $fila.find('[name$=".id_implemento_utilizado"]').rules('add', {
            required: true,
            messages: { required: `Fila ${index + 1}: Debe seleccionar un implemento.` }
        });
        $fila.find('[name$=".cantidad"]').rules('add', {
            required: true,
            min: 1,
            messages: {
                required: `Fila ${index + 1}: La cantidad es obligatoria.`,
                min: `Fila ${index + 1}: La cantidad debe ser mayor a cero.`
            }
        });
        $fila.find('[name$=".unidad_de_medida"]').rules('add', {
            required: true,
            unidadImplementoPermitida: true,
            pluralidadImplemento: $fila.find('[name$=".cantidad"]'),
            messages: {
                required: `Fila ${index + 1}: La unidad de medida es obligatoria.`,
                unidadImplementoPermitida: `Fila ${index + 1}: Unidad de medida no permitida.`,
                pluralidadImplemento: `Fila ${index + 1}: La unidad de medida no corresponde a la cantidad.`
            }
        });
    } else if (tipo === "suministro") {
        $fila.find('[name$=".id_suministro_utilizado"]').rules('add', {
            required: true,
            messages: { required: `Fila ${index + 1}: Debe seleccionar un suministro.` }
        });
        $fila.find('[name$=".cantidad"]').rules('add', {
            required: true,
            min: 1,
            messages: {
                required: `Fila ${index + 1}: La cantidad es obligatoria.`,
                min: `Fila ${index + 1}: La cantidad debe ser mayor a cero.`
            }
        });
        $fila.find('[name$=".unidad_de_medida"]').rules('add', {
            required: true,
            unidadSuministroPermitida: true,
            pluralidadSuministro: $fila.find('[name$=".cantidad"]'),
            messages: {
                required: `Fila ${index + 1}: La unidad de medida es obligatoria.`,
                unidadSuministroPermitida: `Fila ${index + 1}: Unidad de medida no permitida.`,
                pluralidadSuministro: `Fila ${index + 1}: La unidad de medida no corresponde a la cantidad.`
            }
        });
    }
}

// Llama a la función al cargar la página
$(document).ready(function () {
    $('form.needs-validation').each(function () {
        $(this).validate();
    });

    // Aplica validación a las filas existentes en edición para Precios Finales Sugeridos
    if ($("#formPreciosFinalesSugeridos").length) {
        $("#empaques_decoraciones-container .fila-insumo:visible").each(function (i) {
            aplicarValidacionFilaProductoFinal($(this), i, "empaque");
        });
        $("#implementos-container .fila-insumo:visible").each(function (i) {
            aplicarValidacionFilaProductoFinal($(this), i, "implemento");
        });
        $("#suministros-container .fila-insumo:visible").each(function (i) {
            aplicarValidacionFilaProductoFinal($(this), i, "suministro");
        });
    }
});