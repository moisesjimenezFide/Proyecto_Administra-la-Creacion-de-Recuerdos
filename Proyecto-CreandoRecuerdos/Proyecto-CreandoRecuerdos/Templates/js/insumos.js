// Función utilitaria para agregar atributos data-* personalizados a los items del dropdown
function agregarAtributosInsumos($option, $item) {
    const text = $option.text();

    // Materias Primas
    const costoPorGramoMatch = text.match(/Costo por gramo con merma: ₡([\d\.,]+)/);
    if (costoPorGramoMatch) {
        const costo = parseFloat(costoPorGramoMatch[1].replace(/\./g, '').replace(',', '.')) || 0;
        $item.attr('data-costo-por-gramo-con-merma', costo);
    }
    // Productos Preparados
    const costoPorPesoMatch = text.match(/Costo por peso: ₡([\d\.,]+)/);
    if (costoPorPesoMatch) {
        const costo = parseFloat(costoPorPesoMatch[1].replace(/\./g, '').replace(',', '.')) || 0;
        $item.attr('data-costo-por-peso', costo);
    }
    // Empaques, Implementos, Suministros
    const costoPorCantidadMatch = text.match(/Costo por cantidad: ₡([\d\.,]+)/);
    if (costoPorCantidadMatch) {
        const costo = parseFloat(costoPorCantidadMatch[1].replace(/\./g, '').replace(',', '.')) || 0;
        $item.attr('data-costo-por-cantidad', costo);
    }
    // Recetas (para Productos Finales)
    const costoRecetaMatch = text.match(/Costo total:\s*₡([\d\.,]+)/);
    if (costoRecetaMatch) {
        const costo = parseFloat(costoRecetaMatch[1].replace(/\./g, '').replace(',', '.')) || 0;
        $item.attr('data-costo-total', costo);
    }
    const porcionMatch = text.match(/Porción: ([\d\.,]+)/);
    if (porcionMatch) {
        const porcion = parseFloat(porcionMatch[1].replace(',', '.')) || 0;
        $item.attr('data-porcion', porcion);
    }

    // Otros atributos de datos
    $.each($option.data(), function (key, value) {
        $item.attr(`data-${key}`, value);
    });

    $item.attr('data-value', $option.val());
    $item.text($option.text());
    if ($option.is(':selected')) {
        $item.addClass('active');
    }
}

// =====================
// Materias Primas
// =====================
function calcularCamposMateriaPrima() {
    var costo = parseFloat($('#costo').val()) || 0;
    var cantidad = parseFloat($('#cantidad').val()) || 0;
    var volumen = parseFloat($('#volumen_de_porcion_de_presentacion').val()) || 0;
    var unidadPresentacion = ($('#unidad_de_medida_de_presentacion').val() || '').toLowerCase();
    var unidadPeso = ($('#unidad_de_medida_del_peso').val() || '').toLowerCase();
    var merma = parseFloat($('#merma_total_en_gramos').val()) || 0;

    // --- Volumen de porción convertido ---
    function esGramos(u) { return ['g', 'gr', 'grs', 'gramo', 'gramos'].includes(u); }
    function esKilos(u) { return ['kg', 'kilo', 'kilos', 'kilogramo', 'kilogramos'].includes(u); }
    function esMililitros(u) { return ['ml', 'mililitro', 'mililitros'].includes(u); }
    function esLitros(u) { return ['l', 'litro', 'litros'].includes(u); }

    var volumenConvertido = 0;
    var unidadConvertida = unidadPeso;
    if (esKilos(unidadPresentacion) && esGramos(unidadPeso)) {
        volumenConvertido = volumen * 1000;
    } else if (esGramos(unidadPresentacion) && esGramos(unidadPeso)) {
        volumenConvertido = volumen;
    } else if (esLitros(unidadPresentacion) && esMililitros(unidadPeso)) {
        volumenConvertido = volumen * 1000;
    } else if (esMililitros(unidadPresentacion) && esMililitros(unidadPeso)) {
        volumenConvertido = volumen;
    } else {
        volumenConvertido = 0;
        unidadConvertida = '';
    }
    $('#volumen_de_porcion_convertido').text(volumenConvertido.toFixed(2));
    $('#unidad_volumen_convertido_span').text(unidadConvertida);

    // --- Resto de cálculos ---
    $('#unidad_peso_span').text($('#unidad_de_medida_del_peso').val() || '');
    var peso = (cantidad > 0 && volumen > 0) ? cantidad * volumenConvertido : 0;
    var divisor = cantidad * volumenConvertido;
    var costoPorGramo = (cantidad > 0 && volumen > 0 && divisor > 0) ? (costo / divisor) : 0;
    var porcentajeMerma = (divisor > 0) ? (merma * 100.0 / divisor) : 0;
    var costoDeMermaTotal = (divisor > 0) ? ((costo / divisor) * merma) : 0;
    var costoTotalMasMerma = costo + costoDeMermaTotal;
    var costoPorGramoConMerma = (divisor > 0) ? ((costo + ((costo / divisor) * merma)) / divisor) : 0;

    $('#peso').text(peso.toFixed(2));
    $('#costo_por_gramo').text(costoPorGramo.toFixed(2));
    $('#porcentaje_de_merma').text(porcentajeMerma.toFixed(2));
    $('#costo_de_merma_total').text(costoDeMermaTotal.toFixed(2));
    $('#costo_total_mas_merma_total').text(costoTotalMasMerma.toFixed(2));
    $('#costo_por_gramo_con_merma').text(costoPorGramoConMerma.toFixed(2));
}

// =====================
// Productos Preparados
// =====================
function calcularCamposProductoPreparado() {
    var costo = parseFloat($('#costo').val()) || 0;
    var cantidad = parseFloat($('#cantidad').val()) || 0;
    var volumen = parseFloat($('#volumen_de_porcion_de_presentacion').val()) || 0;
    var unidadPresentacion = ($('#unidad_de_medida_de_presentacion').val() || '').toLowerCase();
    var unidadPeso = ($('#unidad_de_medida_del_peso').val() || '').toLowerCase();

    // --- Volumen de porción convertido ---
    function esGramos(u) { return ['g', 'gr', 'grs', 'gramo', 'gramos'].includes(u); }
    function esKilos(u) { return ['kg', 'kilo', 'kilos', 'kilogramo', 'kilogramos'].includes(u); }
    function esMililitros(u) { return ['ml', 'mililitro', 'mililitros'].includes(u); }
    function esLitros(u) { return ['l', 'litro', 'litros'].includes(u); }

    var volumenConvertido = 0;
    var unidadConvertida = unidadPeso;
    if (esKilos(unidadPresentacion) && esGramos(unidadPeso)) {
        volumenConvertido = volumen * 1000;
    } else if (esGramos(unidadPresentacion) && esGramos(unidadPeso)) {
        volumenConvertido = volumen;
    } else if (esLitros(unidadPresentacion) && esMililitros(unidadPeso)) {
        volumenConvertido = volumen * 1000;
    } else if (esMililitros(unidadPresentacion) && esMililitros(unidadPeso)) {
        volumenConvertido = volumen;
    } else {
        volumenConvertido = 0;
        unidadConvertida = '';
    }
    $('#volumen_de_porcion_convertido').text(volumenConvertido.toFixed(2));
    $('#unidad_volumen_convertido_span').text(unidadConvertida);

    // --- Resto de cálculos ---
    $('#unidad_peso_span').text($('#unidad_de_medida_del_peso').val() || '');
    var peso = (cantidad > 0 && volumen > 0) ? cantidad * volumenConvertido : 0;
    var divisor = cantidad * volumenConvertido;
    var costoPorPeso = (cantidad > 0 && volumen > 0 && divisor > 0) ? (costo / divisor) : 0;
    var costoPorPorcionConMerma = volumen * costoPorPeso;

    $('#peso').text(peso.toFixed(2));
    $('#costo_por_peso').text(costoPorPeso.toFixed(2));
    $('#costo_por_porcion_con_merma').text(costoPorPorcionConMerma.toFixed(2));
}

// =====================
// Empaques y/o Decoraciones, Implementos, Suministros
// =====================
function mostrarCostoPorCantidad(costoId, cantidadId, resultadoId) {
    var costo = parseFloat($(`#${costoId}`).val()) || 0;
    var cantidad = parseFloat($(`#${cantidadId}`).val()) || 1;
    var resultado = (cantidad > 0) ? (costo / cantidad) : 0;
    $(`#${resultadoId}`).text(resultado.toFixed(2));
}

// =====================
// Recetas
// =====================
function calcularCostosReceta() {
    let costoTotalReceta = 0;
    const porcion = parseFloat($('#porcion').val()) || 0;

    // Calcular costos de materias primas
    $('#materias_primas-container .row.fila-insumo:not(.template-materia_prima)').each(function () {
        // Solo procesar filas visibles
        if ($(this).is(':visible')) {
            const cantidad = parseFloat($(this).find('input[name$="cantidad"]').val()) || 0;
            const selectedOptionText = $(this).find('select option:selected').text();

            // Extraer el costo del texto de la opción
            const costoMatch = selectedOptionText.match(/Costo por gramo con merma: ₡([\d\.,]+)/);
            if (costoMatch && costoMatch[1]) {
                const costoPorCantidad = parseFloat(costoMatch[1].replace(',', '.')) || 0;
                costoTotalReceta += cantidad * costoPorCantidad;
            }
        }
    });

    // Calcular costos de productos preparados
    $('#productos_preparados-container .row.fila-insumo:not(.template-producto_preparado)').each(function () {
        // Solo procesar filas visibles
        if ($(this).is(':visible')) { // <-- Agregamos esta línea
            const cantidad = parseFloat($(this).find('input[name$="cantidad"]').val()) || 0;
            const selectedOptionText = $(this).find('select option:selected').text();

            // Extraer el costo del texto de la opción
            const costoMatch = selectedOptionText.match(/Costo por peso: ₡([\d\.,]+)/);
            if (costoMatch && costoMatch[1]) {
                const costoPorCantidad = parseFloat(costoMatch[1].replace(',', '.')) || 0;
                costoTotalReceta += cantidad * costoPorCantidad;
            }
        }
    });

    // Actualizar los campos en la vista
    $('#costo_total_receta').text(costoTotalReceta.toFixed(2));
    if (porcion > 0) {
        const costoPorPorcion = costoTotalReceta / porcion;
        $('#costoPorPorcion').text(costoPorPorcion.toFixed(2));
    } else {
        $('#costoPorPorcion').text('0.00');
    }
}

// =====================
// Productos Finales (precio_final_sugerido)
// =====================

// =====================
// Función de Ayuda para el Cálculo
// =====================
function extraerValorDeTexto(texto, patron) {
    const match = texto.match(patron);
    if (match && match[1]) {
        let valor = match[1];

        // Elimina todos los puntos (separadores de miles).
        // Por ejemplo, "1.234,56" se convierte en "1234,56".
        valor = valor.replace(/\./g, '');

        // Reemplaza la coma (,) por un punto (separador decimal).
        // El valor queda como "1234.56".
        valor = valor.replace(',', '.');

        // Convierte el valor a un número flotante.
        return parseFloat(valor) || 0;
    }
    return 0;
}

function calcularPrecioFinalProductoFinal() {
    // Costo de la receta
    const selectedRecetaText = $('#id_receta option:selected').text();
    const costoReceta = extraerValorDeTexto(selectedRecetaText, /Costo total:\s*₡([\d\.,]+)/);
    const porcion = extraerValorDeTexto(selectedRecetaText, /Porción: ([\d\.,]+)/);

    // Margen de utilidad
    const margenUtilidad = parseFloat($('#margen_de_utilidad').val()) || 0;
    const costoConUtilidad = (margenUtilidad >= 100) ? 0 : costoReceta / (1 - (margenUtilidad / 100));

    // Empaques y decoraciones Utilizad@s
    let sumaEmpaquesPorCantidad = 0;
    $('#empaques_decoraciones-container .fila-insumo').each(function () {
        if ($(this).is(':visible')) { // <-- Agregamos esta línea
            const cantidad = parseFloat($(this).find('input[name$=".cantidad"]').val()) || 0;
            const selectedOptionText = $(this).find('select option:selected').text();
            const costoPorCantidad = extraerValorDeTexto(selectedOptionText, /Costo por cantidad: ₡([\d\.,]+)/);
            sumaEmpaquesPorCantidad += costoPorCantidad * cantidad;
        }
    })

    // Implementos Utilizados
    let sumaImplementosPorCantidad = 0;
    $('#implementos-container .fila-insumo').each(function () {
        if ($(this).is(':visible')) { // <-- Agregamos esta línea
            const cantidad = parseFloat($(this).find('input[name$=".cantidad"]').val()) || 0;
            const selectedOptionText = $(this).find('select option:selected').text();
            const costoPorCantidad = extraerValorDeTexto(selectedOptionText, /Costo por cantidad: ₡([\d\.,]+)/);
            sumaImplementosPorCantidad += costoPorCantidad * cantidad;
        }
    });

    // Suministros Utilizados
    let totalSuministros = 0;
    let costoImpresionFacturaPorInsumo = 0;
    let costoTotalImpresionFactura = 0;

    $('#suministros-container .fila-insumo').each(function () {
        if ($(this).is(':visible')) { // <-- Agregamos esta línea
            const cantidad = parseFloat($(this).find('input[name$=".cantidad"]').val()) || 0;
            const esImpresion = $(this).find('input[type="checkbox"]').is(':checked');
            const selectedOptionText = $(this).find('select option:selected').text();
            const costoPorCantidad = extraerValorDeTexto(selectedOptionText, /Costo por cantidad: ₡([\d\.,]+)/);

            if (esImpresion) {
                costoImpresionFacturaPorInsumo = (costoPorCantidad || 0) / 20;
                costoTotalImpresionFactura = porcion > 0 ? porcion * costoImpresionFacturaPorInsumo : costoImpresionFacturaPorInsumo;
            } else {
                totalSuministros += costoPorCantidad * cantidad;
            }
        }
    });

    // Costo total de insumos
    const costoTotalInsumos = sumaEmpaquesPorCantidad + sumaImplementosPorCantidad + totalSuministros;

    // Factura por Insumo
    const facturaPorInsumo = costoTotalInsumos + costoImpresionFacturaPorInsumo;

    //Factura Total
    const facturaTotal = costoTotalInsumos + costoTotalImpresionFactura;

    // Costo Total de Insumos con Ganancia
    const totalInsumosConGanancia = facturaTotal * 1.10;

    // IVA y Servicio
    let baseImpuestos = costoConUtilidad + totalInsumosConGanancia;
    const porcentajeIva = parseFloat($('#porcentaje_de_iva').val()) || 0;
    const porcentajeServicio = parseFloat($('#porcentaje_de_servicio').val()) || 0;
    const costoConIva = baseImpuestos * (porcentajeIva / 100);
    const costoConServicio = baseImpuestos * (porcentajeServicio / 100);

    // Envío según plataforma
    const plataforma = $('#plataforma_de_envio').val();
    let envio = 0;
    switch (plataforma) {
        case "PedidosYa (25%)":
        case "Rappi (25%)":
            envio = baseImpuestos * 0.25;
            break;
        case "DidiFood (30%)":
            envio = baseImpuestos * 0.30;
            break;
        case "UberEats (40%)":
            envio = baseImpuestos * 0.40;
            break;
        default:
            envio = 0; // Propio (0%)
            break;
    }

    // Precio final sugerido
    const precioFinalSugerido = baseImpuestos + costoConIva + costoConServicio + envio;

    // Actualizar los valores en la vista
    $('#costo_receta').text(costoReceta.toFixed(2));
    $('#costo_sin_margen_de_utilidad').text((100 - margenUtilidad).toFixed(2));
    $('#costo_con_margen_de_utilidad').text(costoConUtilidad.toFixed(2));
    $('#costo_empaque_decoracion_utilizado').text(sumaEmpaquesPorCantidad.toFixed(2));
    $('#costo_implemento_utilizado').text(sumaImplementosPorCantidad.toFixed(2));
    $('#costo_suministro_utilizado').text(totalSuministros.toFixed(2));
    $('#costo_total_insumos').text(costoTotalInsumos.toFixed(2));
    $('#costo_de_impresion_de_factura_por_insumo').text(costoImpresionFacturaPorInsumo.toFixed(2));
    $('#costo_total_de_impresion_de_factura').text(costoTotalImpresionFactura.toFixed(2));
    $('#factura_por_insumo').text(facturaPorInsumo.toFixed(2));
    $('#factura_total').text(facturaTotal.toFixed(2));
    $('#costo_total_empaque_decoracion_implemento_suministro_por_porcentaje_de_ganancia').text(totalInsumosConGanancia.toFixed(2));
    $('#costo_con_iva').text(costoConIva.toFixed(2));
    $('#costo_con_servicio').text(costoConServicio.toFixed(2));
    $('#envio').text(envio.toFixed(2));
    $('#precio_final_sugerido').text(precioFinalSugerido.toFixed(2));
}

// =====================
// Utilidades y eventos globales
// =====================
function agregarFila(containerId, templateClass) {
    const container = $(`#${containerId}`);
    const template = container.find(`.${templateClass}`);
    const newRow = template.clone();
    const index = container.find('.fila-insumo').length - 1;
    newRow.removeClass(templateClass).show();

    newRow.find('input, select, textarea').each(function () {
        const $el = $(this);
        const name = $el.attr('name');
        const id = $el.attr('id');

        if (name) {
            $el.attr('name', name.replace(/\[__index__\]/g, `[${index}]`));
        }
        if (id) {
            $el.attr('id', id.replace(/__index__/g, index));
        }
    });

    // Se mantiene el boton de eliminar, ya que es parte de la fila
    const deleteButton = $('<div class="col-3 d-flex align-items-center justify-content-end"><button type="button" class="btn btn-danger btn-eliminar-fila">- Eliminar</button></div>');
    newRow.find('.col-md-5 .row > div:last-child').replaceWith(deleteButton);
    container.find('.btn-agregar-fila').before(newRow);

    newRow.find('select.form-control').each(function () {
        convertToCustomDropdown(this);
    });

    // Disparar el evento 'change' en la nueva fila para que los cálculos se actualicen.
    newRow.find('input, select').trigger('change');

    // Actualizar el valor del TOTAL_FORMS
    const totalFormsId = `id_${containerId.split('-')[0]}-TOTAL_FORMS`;
    $(`#${totalFormsId}`).val(index + 1);

    // Llamar a los cálculos
    if (typeof calcularCostosReceta === 'function') {
        calcularCostosReceta();
    }
    if (typeof calcularPrecioFinalProductoFinal === 'function') {
        calcularPrecioFinalProductoFinal();
    }

    // --- INTEGRACIÓN DE VALIDACIÓN DINÁMICA ---
    if (containerId === "materias_primas-container") {
        aplicarValidacionFilaReceta(newRow, index, "materia");
        // Fuerza la validación en los nuevos inputs
        newRow.find('input, select').each(function () {
            $("#formCostosRecetas").validate().element(this);
        });
    }
    if (containerId === "productos_preparados-container") {
        aplicarValidacionFilaReceta(newRow, index, "producto");
        newRow.find('input, select').each(function () {
            $("#formCostosRecetas").validate().element(this);
        });
    }
    if (containerId === "empaques_decoraciones-container") {
        aplicarValidacionFilaProductoFinal(newRow, index, "empaque");
        newRow.find('input, select').each(function () {
            $("#formPreciosFinalesSugeridos").validate().element(this);
        });
    }
    if (containerId === "implementos-container") {
        aplicarValidacionFilaProductoFinal(newRow, index, "implemento");
        newRow.find('input, select').each(function () {
            $("#formPreciosFinalesSugeridos").validate().element(this);
        });
    }
    if (containerId === "suministros-container") {
        aplicarValidacionFilaProductoFinal(newRow, index, "suministro");
        newRow.find('input, select').each(function () {
            $("#formPreciosFinalesSugeridos").validate().element(this);
        });
    }
}

function eliminarFila(boton) {
    // Encuentra la fila y el contenedor.
    const fila = $(boton).closest(".fila-insumo");
    const container = fila.parent();

    // Marca el campo DELETE (hidden checkbox) para que Django sepa que debe eliminar esta fila.
    // El id del checkbox de borrado normalmente termina en '-DELETE'.
    const deleteInput = fila.find('input[type="checkbox"][id$="-DELETE"]');
    if (deleteInput.length) {
        deleteInput.prop('checked', true);
    }

    // Oculta la fila en lugar de eliminarla.
    fila.hide();

    // Llama a las funciones de cálculo inmediatamente después de ocultar la fila.
    if (typeof calcularCostosReceta === 'function') {
        calcularCostosReceta();
    }
    if (typeof calcularPrecioFinalProductoFinal === 'function') {
        calcularPrecioFinalProductoFinal();
    }

    // Reindexa las filas restantes que están visibles.
    const filas = container.find('.fila-insumo:visible');
    filas.each(function (idx) {
        $(this).find('input, select, textarea').each(function () {
            if (this.name) {
                this.name = this.name.replace(/\[\d+\]/g, `[${idx}]`);
            }
            if (this.id) {
                this.id = this.id.replace(/-\d+-/g, `-${idx}-`);
            }
        });
    });

    // Actualiza el valor de TOTAL_FORMS, contando solo las filas visibles.
    const containerId = container.attr('id');
    const totalFormsId = `id_${containerId.split('-')[0]}-TOTAL_FORMS`;
    $(`#${totalFormsId}`).val(filas.length);

    // Llama a las funciones de cálculo para actualizar los valores.
    if (typeof calcularCostosReceta === 'function') {
        calcularCostosReceta();
    }
    if (typeof calcularPrecioFinalProductoFinal === 'function') {
        calcularPrecioFinalProductoFinal();
    }
}

// =====================
// Inicialización por vista
// =====================
$(document).ready(function () {

    // Convertir todos los dropdowns con clase form-control al cargar la página
    /*$('.form-control').each(function () {
        if ($(this).is('select')) {
            convertToCustomDropdown(this);
        }
    });

    // Botones de agregar y eliminar filas
    $(document).on('click', '.btn-agregar-fila', function () {
        const containerId = $(this).data('container');
        const templateClass = $(this).data('template');
        agregarFila(containerId, templateClass);
    });

    $(document).on('click', '.btn-eliminar-fila', function () {
        eliminarFila(this);
    });

    // Manejar clicks en el botón del dropdown de formularios
    $(document).on('click', '.custom-select-button', function (e) {
        e.preventDefault();
        e.stopPropagation();

        $('.custom-select-menu').not($(this).siblings('.custom-select-menu')).removeClass('show');
        $(this).siblings('.custom-select-menu').toggleClass('show');
    });

    // Manejar selección de items en formularios
    $(document).on('click', '.custom-select-item', function (e) {
        e.preventDefault();
        e.stopPropagation();

        const $item = $(this);
        const value = $item.attr('data-value');
        const text = $item.text();

        const $dropdown = $item.closest('.custom-select');
        const $originalSelect = $dropdown.find('select.custom-hidden');

        $originalSelect.val(value).trigger('change');

        const $button = $dropdown.find('.custom-select-button');
        $button.text(text);

        $dropdown.find('.custom-select-item').removeClass('active');
        $item.addClass('active');

        $dropdown.find('.custom-select-menu').removeClass('show');

        // Llama a las funciones de cálculo de las vistas que correspondan al seleccionar una opción
        if (typeof calcularCostosReceta === 'function') {
            calcularCostosReceta();
        }
        if (typeof calcularPrecioFinalProductoFinal === 'function') {
            calcularPrecioFinalProductoFinal();
        }
    });

    // Cerrar dropdowns al hacer click fuera
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.custom-select').length) {
            $('.custom-select-menu').removeClass('show');
        }
    });

    // Prevenir menús contextuales en los dropdowns personalizados
    $(document).on('contextmenu selectstart dragstart', '.custom-select, .custom-select-button, .custom-select-item', function (e) {
        e.preventDefault();
        return false;
    });

    // Observar cambios en el DOM para dropdowns agregados dinámicamente
    const observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            if (mutation.type === 'childList') {
                $(mutation.addedNodes).find('select.form-control').each(function () {
                    if (!$(this).hasClass('custom-hidden')) {
                        convertToCustomDropdown(this);
                    }
                });
            }
        });
    });

    // Iniciar observación
    observer.observe(document.body, {
        childList: true,
        subtree: true
    });*/

    // Botón eliminar con confirmación (Swal)
    $(document).on('click', '.btn-eliminar', function (e) {
        e.preventDefault();
        var url = $(this).data('url');
        Swal.fire({
            title: '¿Estás seguro?',
            text: "¡Esta acción no se puede deshacer!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#B54885',
            cancelButtonColor: '#E74C3C',
            confirmButtonText: 'Aceptar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = url;
            }
        });
    });

    // =====================
    // Lógica de cálculo para cada vista
    // =====================

    // Materias Primas
    $('#cantidad, #volumen_de_porcion_de_presentacion, #unidad_de_medida_de_presentacion, #costo, #unidad_de_medida_del_peso, #merma_total_en_gramos')
        .on('input change', calcularCamposMateriaPrima);
    if (typeof calcularCamposMateriaPrima === 'function') {
        calcularCamposMateriaPrima();
    }

    // Productos Preparados
    $('#cantidad, #volumen_de_porcion_de_presentacion, #unidad_de_medida_de_presentacion, #costo, #unidad_de_medida_del_peso')
        .on('input change', calcularCamposProductoPreparado);
    if (typeof calcularCamposProductoPreparado === 'function') {
        calcularCamposProductoPreparado();
    }

    // Empaques y Decoraciones
    $('#costo, #cantidad').on('input change', function () {
        mostrarCostoPorCantidad('costo', 'cantidad', 'costoPorCantidad');
    });
    if (typeof mostrarCostoPorCantidad === 'function') {
        mostrarCostoPorCantidad('costo', 'cantidad', 'costoPorCantidad');
    }

    // Implementos
    $('#costo, #cantidad').on('input change', function () {
        mostrarCostoPorCantidad('costo', 'cantidad', 'costoPorCantidad');
    });
    if (typeof mostrarCostoPorCantidad === 'function') {
        mostrarCostoPorCantidad('costo', 'cantidad', 'costoPorCantidad');
    }

    // Suministros
    $('#costo, #cantidad').on('input change', function () {
        mostrarCostoPorCantidad('costo', 'cantidad', 'costoPorCantidad');
    });
    if (typeof mostrarCostoPorCantidad === 'function') {
        mostrarCostoPorCantidad('costo', 'cantidad', 'costoPorCantidad');
    }

    // Recetas

    // Delegar eventos a inputs y selects de las filas dinámicas
    $('#materias_primas-container, #productos_preparados-container').on('input change', '.fila-insumo input, .fila-insumo select', function () {
        calcularCostosReceta();
    });

    // Delegar eventos al campo de porción
    $('#porcion').on('input change', calcularCostosReceta);

    // Ejecutar los cálculos al cargar la página
    if (typeof calcularCostosReceta === 'function') {
        calcularCostosReceta();
    }

    // Productos Finales (precios_finales_sugeridos)

    // Delegar eventos a inputs y selects de las filas dinámicas
    $('#empaques_decoraciones-container, #implementos-container, #suministros-container').on('input change', '.fila-insumo input, .fila-insumo select, .fila-insumo input[type="checkbox"]', function () {
        calcularPrecioFinalProductoFinal();
    });

    // Delegar eventos a los campos principales
    $('#id_receta, #margen_de_utilidad, #plataforma_de_envio, #porcentaje_de_iva, #porcentaje_de_servicio').on('input change', calcularPrecioFinalProductoFinal);

    // Ejecutar los cálculos de precios finales al cargar la página
    if (typeof calcularPrecioFinalProductoFinal === 'function') {
        calcularPrecioFinalProductoFinal();
    }

    /* =====================
    // DataTables y Exportaciones para todas las tablas de insumos
    // =====================
    if ($('#tabla_insumos').length) {
        const totalFilas = $('#tabla_insumos tbody tr').length;

        // Opciones de cantidad
        let opciones = [];
        for (let i = 5; i <= totalFilas; i += 5) {
            opciones.push(i);
        }
        const ultimoMultiplo = Math.floor(totalFilas / 5) * 5;
        const siguienteValor = totalFilas !== ultimoMultiplo ? totalFilas - 1 : null;
        if (siguienteValor && !opciones.includes(siguienteValor)) {
            opciones.push(siguienteValor);
        }
        opciones.sort((a, b) => a - b);
        const valoresNumericos = [...opciones, -1];
        const valoresVisibles = [...opciones.map(n => n.toString()), "Todos"];

        var table = $('#tabla_insumos').DataTable({
            pageLength: 3,
            searching: false,
            pagingType: "full_numbers",
            lengthMenu: [valoresNumericos, valoresVisibles],
            dom: "<'dt-buttons mb-2'B><'d-flex justify-content-between align-items-center mb-3'<'dt-length'l><'dataTables_filter'f>>rtip",
            buttons: [
                {
                    extend: 'copyHtml5',
                    text: 'Copiar',
                    className: 'btn btn-custom-pink btn-sm'
                },
                {
                    extend: 'excelHtml5',
                    text: 'Exportar a Excel',
                    className: 'btn btn-custom-pink btn-sm'
                },
                {
                    extend: 'pdfHtml5',
                    text: 'Exportar a PDF',
                    orientation: 'landscape',
                    pageSize: 'A4',
                    className: 'btn btn-custom-pink btn-sm'
                },
                {
                    extend: 'print',
                    text: 'Imprimir',
                    className: 'btn btn-custom-pink btn-sm'
                }
            ],
            language: {
                url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json",
                lengthMenu: "Mostrar _MENU_ registros por página",
                info: "Mostrando _START_ a _END_ de _TOTAL_ registros",
                infoEmpty: "No hay registros disponibles",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                paginate: {
                    first: "Primer página",
                    last: "Última página",
                    previous: false,
                    next: false,
                }
            }
        });

        // Dropdown personalizado para la cantidad de registros por página 
        setTimeout(function () {
            createCustomLengthDropdown();
        }, 100);

        function createCustomLengthDropdown() {
            var lengthContainer = $('.dataTables_length');
            var originalSelect = lengthContainer.find('select');

            // Crear HTML del dropdown personalizado para la cantidad de registros por página
            var dropdownItems = '';
            for (let i = 0; i < valoresNumericos.length; i++) {
                const value = valoresNumericos[i];
                const text = valoresVisibles[i];
                const activeClass = value === 3 ? 'active' : '';
                dropdownItems += `<div class="custom-dropdown-item ${activeClass}" data-value="${value}">${text}</div>`;
            }

            var customDropdown = $(`
                <div class="custom-length-dropdown">
                    <div class="custom-dropdown-button" id="lengthDropdownBtn">3</div>
                    <div class="custom-dropdown-menu" id="lengthDropdownMenu">
                        ${dropdownItems}
                    </div>
                </div>
            `);

            // Reemplazar el select por el dropdown personalizado para la cantidad de registros por página
            originalSelect.after(customDropdown);

            // Actualizar el texto del label
            var label = lengthContainer.find('label');
            label.contents().filter(function () {
                return this.nodeType === 3;
            }).remove();
            label.prepend('Mostrar ');
            label.append(' registros por página');
        }

        // Eventos para el dropdown personalizado para la cantidad de registros por página
        $(document).on('click', '.custom-dropdown-button', function (e) {
            e.preventDefault();
            e.stopPropagation();
            $('.custom-dropdown-menu').toggleClass('show');
        });

        $(document).on('click', '.custom-dropdown-item', function (e) {
            e.preventDefault();
            e.stopPropagation();

            var value = $(this).data('value');
            var text = $(this).text();

            $('.custom-dropdown-button').text(text);
            $('.custom-dropdown-item').removeClass('active');
            $(this).addClass('active');
            $('.custom-dropdown-menu').removeClass('show');
            table.page.len(value).draw();
        });

        $(document).on('click', function (e) {
            if (!$(e.target).closest('.custom-length-dropdown').length) {
                $('.custom-dropdown-menu').removeClass('show');
            }
        });

        $(document).on('contextmenu selectstart dragstart', '.custom-length-dropdown, .custom-dropdown-button, .custom-dropdown-item', function (e) {
            e.preventDefault();
            return false;
        });
    }*/

    // Vaciar selects al darle cancelar
    const cancelButton = document.querySelector('button[type="reset"]');

    if (cancelButton) {
        cancelButton.addEventListener('click', function () {
            // Selecciona todos los elementos <select> que están visibles o ocultos.
            const originalSelects = document.querySelectorAll('select');

            originalSelects.forEach(select => {
                // 1. Resetea el select original (oculto)
                select.selectedIndex = 0;

                // 2. Busca el dropdown personalizado asociado y actualiza su botón.
                // Esto es crucial para que el cambio sea visible para el usuario.
                const customDropdownWrapper = select.closest('.custom-select');
                if (customDropdownWrapper) {
                    const customButton = customDropdownWrapper.querySelector('.custom-select-button');
                    const firstOptionText = select.options[0].text;

                    // Actualiza el texto del botón visible.
                    if (customButton) {
                        customButton.textContent = firstOptionText;
                    }

                    // Asegúrate de que no haya opciones marcadas como 'active' en el menú.
                    const customItems = customDropdownWrapper.querySelectorAll('.custom-select-item');
                    customItems.forEach(item => item.classList.remove('active'));
                }
            });
        });
    }
});