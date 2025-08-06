// =====================
// CUSTOM DROPDOWNS PARA FORMULARIOS
// =====================

// Función para convertir dropdowns normales al estilo personalizado
function convertToCustomDropdown(selectElement) {
    var $select = $(selectElement);
    var selectedValue = $select.val();
    var selectedText = $select.find('option:selected').text();

    // Si no hay valor seleccionado, usar el placeholder
    if (!selectedValue || selectedValue === '') {
        selectedText = $select.find('option:first').text();
    }

    // Crear el HTML del dropdown personalizado
    var dropdownItems = '';
    $select.find('option').each(function () {
        var value = $(this).val();
        var text = $(this).text();
        var isSelected = value === selectedValue;
        var activeClass = isSelected ? 'active' : '';

        dropdownItems += `<div class="custom-select-item ${activeClass}" data-value="${value}">${text}</div>`;
    });

    var customDropdown = $(`
        <div class="custom-select">
            <div class="custom-select-button">${selectedText}</div>
            <div class="custom-select-menu">
                ${dropdownItems}
            </div>
        </div>
    `);

    // Insertar el dropdown personalizado después del select original
    $select.after(customDropdown);

    // Ocultar el select original
    $select.addClass('custom-hidden');

    return customDropdown;
}

// Función para actualizar dropdowns cuando se agregan dinámicamente
function updateCustomDropdowns() {
    $('.form-control:not(.custom-hidden)').each(function () {
        if ($(this).is('select') && !$(this).next('.custom-select').length) {
            convertToCustomDropdown(this);
        }
    });
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
// Empaques y Decoraciones
// =====================
function mostrarCostoPorCantidadEmpaque() {
    var costo = parseFloat($('#costo').val()) || 0;
    var cantidad = parseFloat($('#cantidad').val()) || 1;
    var resultado = (cantidad > 0) ? (costo / cantidad) : 0;
    $('#costoPorCantidad').text(resultado.toFixed(2));
}

// =====================
// Implementos
// =====================
function mostrarCostoPorCantidadImplemento() {
    var costo = parseFloat($('#costo').val()) || 0;
    var cantidad = parseFloat($('#cantidad').val()) || 1;
    var resultado = (cantidad > 0) ? (costo / cantidad) : 0;
    $('#costoPorCantidad').text(resultado.toFixed(2));
}

// =====================
// Suministros
// =====================
function mostrarCostoPorCantidadSuministro() {
    var costo = parseFloat($('#costo').val()) || 0;
    var cantidad = parseFloat($('#cantidad').val()) || 1;
    var resultado = (cantidad > 0) ? (costo / cantidad) : 0;
    $('#costoPorCantidad').text(resultado.toFixed(2));
}

// =====================
// Recetas
// =====================
function obtenerCostoPorGramoConMerma(nombre) {
    if (typeof materiasPrimas === "undefined") return 0;
    var found = materiasPrimas.find(x => x.nombre === nombre);
    return found ? parseFloat(found.costo_por_gramo_con_merma) : 0;
}

function obtenerCostoPorPeso(nombre) {
    var found = productosPreparados.find(x => x.nombre === nombre);
    return found ? parseFloat(found.costo_por_peso) : 0;
}

function calcularCostosReceta() {
    var total = 0;

    // Materias primas
    $('#materias_primas-container .fila-insumo:not(.template-materia_prima)').each(function () {
        var idMateriaPrima = $(this).find('select[name*="id_materia_prima_utilizada"]').val();
        var cantidad = parseFloat($(this).find('input[name*=".cantidad"]').val()) || 0;
        var nombre = $(this).find('select[name*="id_materia_prima_utilizada"] option:selected').text();
        var costo = obtenerCostoPorGramoConMerma(nombre);
        console.log("Materia Prima - ID:", idMateriaPrima, "Cantidad:", cantidad, "Nombre:", nombre, "Costo:", costo);
        total += cantidad * costo;
    });

    // Productos preparados
    $('#productos_preparados-container .fila-insumo:not(.template-producto_preparado)').each(function () {
        var idProductoPreparado = $(this).find('select[name*="id_producto_preparado_utilizado"]').val();
        var cantidad = parseFloat($(this).find('input[name*=".cantidad"]').val()) || 0;
        var nombre = $(this).find('select[name*="id_producto_preparado_utilizado"] option:selected').text();
        var costo = obtenerCostoPorPeso(nombre);
        console.log("Producto Preparado - ID:", idProductoPreparado, "Cantidad:", cantidad, "Nombre:", nombre, "Costo:", costo);
        total += cantidad * costo;
    });

    $('#costoTotalReceta').text(total.toFixed(2));
    var porcion = parseFloat($('#porcion').val()) || 0;
    var costoPorPorcion = porcion > 0 ? total / porcion : 0;
    $('#costoPorPorcion').text(costoPorPorcion.toFixed(2));
}

// =====================
// Productos Finales (precio_final)
// =====================

// Catálogos globales (deben ser definidos en la vista Razor)
var recetas = window.recetas || [];
var empaques = window.empaques || [];
var implementos = window.implementos || [];
var suministros = window.suministros || [];

function obtenerCostoReceta(nombreReceta) {
    var receta = recetas.find(x => x.nombre === nombreReceta);
    return receta ? parseFloat(receta.costo_total_receta) : 0;
}

function obtenerCostoUnitario(nombre, catalogo) {
    var found = catalogo.find(x => x.nombre === nombre);
    return found ? parseFloat(found.costo_por_cantidad) : 0;
}

function calcularTotalesPorInsumo(container, catalogo) {
    var total = 0;
    $(container + ' .fila-insumo:not([style*="display: none"])').each(function () {
        var nombre = $(this).find('select').val();
        var cantidad = parseFloat($(this).find('input[type="number"]').val()) || 0;
        var costoUnitario = obtenerCostoUnitario(nombre, catalogo);
        total += cantidad * costoUnitario;
    });
    return total;
}

function calcularPrecioFinalProductoFinal() {
    // Costo de la receta
    var nombreReceta = $('#nombre_receta').val();
    var receta = recetas.find(x => x.nombre === nombreReceta) || {};
    var costoReceta = parseFloat(receta.costo_total_receta) || 0;
    var porcion = parseFloat(receta.porcion) || 1;

    // Margen de utilidad
    var margenUtilidad = parseFloat($('#margen_de_utilidad').val()) || 0;
    var costoSinUtilidad = 100 - margenUtilidad;
    var costoConUtilidad = costoReceta / (costoSinUtilidad / 100);

    // Suministros normales y de impresión
    var suministrosUtilizados = [];
    $('#suministros-container .fila-insumo:not([style*="display: none"])').each(function () {
        var id = $(this).find('select').val();
        var cantidad = parseFloat($(this).find('input[name$=".cantidad"]').val()) || 0;
        var esImpresion = $(this).find('input[type="checkbox"]').is(':checked');
        var obj = suministros.find(s => s.id == id) || {};
        suministrosUtilizados.push({
            ...obj,
            cantidad: cantidad,
            es_impresion_de_facturas: esImpresion,
            costo_por_cantidad: parseFloat(obj.costo_por_cantidad) || 0
        });
    });

    var suministrosNormales = suministrosUtilizados.filter(s => !s.es_impresion_de_facturas);
    var suministroImpresion = suministrosUtilizados.find(s => s.es_impresion_de_facturas);

    var totalSuministros = suministrosNormales.reduce((acc, s) => acc + ((s.costo_por_cantidad || 0) * (s.cantidad || 0)), 0);

    // Costo impresión factura
    var costoImpresionFacturaPorInsumo = 0;
    var costoTotalImpresionFactura = 0;
    if (suministroImpresion) {
        costoImpresionFacturaPorInsumo = (suministroImpresion.costo_por_cantidad || 0) / 20;
        costoTotalImpresionFactura = porcion * costoImpresionFacturaPorInsumo;
    }

    // Empaques y decoraciones
    var sumaEmpaquesPorCantidad = 0;
    $('#empaques_decoraciones-container .fila-insumo:not([style*="display: none"])').each(function () {
        var id = $(this).find('select').val();
        var cantidad = parseFloat($(this).find('input[name$=".cantidad"]').val()) || 0;
        var obj = empaques.find(e => e.id == id) || {};
        sumaEmpaquesPorCantidad += (parseFloat(obj.costo_por_cantidad) || 0) * cantidad;
    });

    // Implementos
    var sumaImplementosPorCantidad = 0;
    $('#implementos-container .fila-insumo:not([style*="display: none"])').each(function () {
        var id = $(this).find('select').val();
        var cantidad = parseFloat($(this).find('input[name$=".cantidad"]').val()) || 0;
        var obj = implementos.find(i => i.id == id) || {};
        sumaImplementosPorCantidad += (parseFloat(obj.costo_por_cantidad) || 0) * cantidad;
    });

    // Suministros normales (ya calculado arriba)
    var sumaSuministrosPorCantidad = totalSuministros;

    var costoTotalInsumos = sumaEmpaquesPorCantidad + sumaImplementosPorCantidad + sumaSuministrosPorCantidad; // NUEVO

    // Factura por insumo y factura total
    var facturaPorInsumo = costoTotalInsumos + costoImpresionFacturaPorInsumo;
    var facturaTotal = costoTotalInsumos + costoTotalImpresionFactura;

    // Total insumos con ganancia
    var totalInsumosConGanancia = facturaTotal * 1.10;

    // IVA y Servicio
    var ivaPorcentaje = parseFloat($('#iva_porcentaje').val()) || 0;
    var servicioPorcentaje = parseFloat($('#servicio_porcentaje').val()) || 0;
    var baseImpuestos = costoConUtilidad + totalInsumosConGanancia;
    var iva = baseImpuestos * (ivaPorcentaje / 100);
    var servicio = baseImpuestos * (servicioPorcentaje / 100);

    // Envío
    var plataforma = $('#plataforma_de_envio').val();
    var envio = 0;
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
            envio = 0;
            break;
    }

    // Precio final sugerido
    var precioFinal = baseImpuestos + iva + servicio + envio;

    // Mostrar resultados en la vista
    $('#costo_total_receta').text(costoReceta.toFixed(2));
    $('#costo_sin_margen_de_utilidad').text(costoSinUtilidad.toFixed(2));
    $('#costo_con_margen_de_utilidad').text(costoConUtilidad.toFixed(2));
    $('#costo_empaque_decoracion_utilizado').text(sumaEmpaquesPorCantidad.toFixed(2));
    $('#costo_implemento_utilizado').text(sumaImplementosPorCantidad.toFixed(2));
    $('#costo_suministro_utilizado').text(sumaSuministrosPorCantidad.toFixed(2));
    $('#costo_total_insumos').text(costoTotalInsumos.toFixed(2));
    $('#costo_de_impresion_de_factura_por_insumo').text(costoImpresionFacturaPorInsumo.toFixed(2));
    $('#costo_total_de_impresion_de_factura').text(costoTotalImpresionFactura.toFixed(2));
    $('#factura_por_insumo').text(facturaPorInsumo.toFixed(2));
    $('#factura_total').text(facturaTotal.toFixed(2));
    $('#costo_total_empaque_decoracion_implemento_suministro_por_porcentaje_de_ganancia').val(totalInsumosConGanancia.toFixed(2));
    $('#iva').text(iva.toFixed(2));
    $('#impuesto_de_servicio').text(servicio.toFixed(2));
    $('#envio').text(envio.toFixed(2));
    $('#precio_final_sugerido').text(precioFinal.toFixed(2));
}

// =====================
// Utilidades y eventos globales
// =====================
function agregarFila(containerId, templateClass) {
    const container = document.getElementById(containerId);
    const template = container.querySelector(`.${templateClass}[style*="display: none"]`);
    const filas = container.querySelectorAll('.fila-insumo:not([style*="display: none"])');
    const index = filas.length;

    const clone = template.cloneNode(true);
    clone.style.display = '';
    clone.classList.remove(templateClass);

    clone.querySelectorAll('input, select').forEach(input => {
        if (input.name) {
            input.name = input.name.replace(/__index__/g, index);
        }
        if (input.tagName === "SELECT") {
            input.selectedIndex = 0;
        }
        if (input.tagName === "INPUT") {
            input.value = "";
        }
    });

    container.appendChild(clone);
}

function eliminarFila(boton) {
    const fila = boton.closest(".fila-insumo");
    const container = fila.parentElement;
    fila.remove();

    const filas = container.querySelectorAll('.fila-insumo:not([style*="display: none"])');
    filas.forEach((f, idx) => {
        f.querySelectorAll('input, select').forEach(input => {
            if (input.name) {
                input.name = input.name.replace(/\[\d+\]/g, `[${idx}]`);
            }
        });
    });
}

// =====================
// Inicialización por vista
// =====================
$(document).ready(function () {

    // Convertir todos los dropdowns con clase form-control al cargar la página
    $('.form-control').each(function () {
        if ($(this).is('select')) {
            convertToCustomDropdown(this);
        }
    });

    // Manejar clicks en el botón del dropdown de formularios
    $(document).on('click', '.custom-select-button', function (e) {
        e.preventDefault();
        e.stopPropagation();

        // Cerrar otros dropdowns abiertos
        $('.custom-select-menu').not($(this).siblings('.custom-select-menu')).removeClass('show');
        $('.custom-dropdown-menu').removeClass('show'); // También cerrar dropdowns de DataTable

        // Toggle del dropdown actual
        var $menu = $(this).siblings('.custom-select-menu');
        $menu.toggleClass('show');
    });

    // Manejar selección de items en formularios
    $(document).on('click', '.custom-select-item', function (e) {
        e.preventDefault();
        e.stopPropagation();

        var value = $(this).data('value');
        var text = $(this).text();
        var $dropdown = $(this).closest('.custom-select');
        var $button = $dropdown.find('.custom-select-button');
        var $menu = $dropdown.find('.custom-select-menu');
        var $originalSelect = $dropdown.prev('select.custom-hidden');

        // Actualizar el texto del botón
        $button.text(text);

        // Actualizar estados activos
        $menu.find('.custom-select-item').removeClass('active');
        $(this).addClass('active');

        // Actualizar el select original
        $originalSelect.val(value).trigger('change');

        // Cerrar el dropdown
        $menu.removeClass('show');
    });

    // Cerrar dropdowns al hacer click fuera
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.custom-select, .custom-length-dropdown').length) {
            $('.custom-select-menu').removeClass('show');
            $('.custom-dropdown-menu').removeClass('show');
        }
    });

    // Prevenir menús contextuales en los dropdowns personalizados
    $(document).on('contextmenu selectstart dragstart', '.custom-select, .custom-select-button, .custom-select-item', function (e) {
        e.preventDefault();
        return false;
    });

    // Observar cambios en el DOM para dropdowns agregados dinámicamente
    var observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            if (mutation.type === 'childList') {
                mutation.addedNodes.forEach(function (node) {
                    if (node.nodeType === 1) { // Element node
                        var $node = $(node);
                        // Buscar selects en el nodo agregado
                        $node.find('select.form-control').each(function () {
                            if (!$(this).hasClass('custom-hidden')) {
                                convertToCustomDropdown(this);
                            }
                        });
                    }
                });
            }
        });
    });

    // Iniciar observación
    observer.observe(document.body, {
        childList: true,
        subtree: true
    });

    // Botón eliminar con confirmación (Swal)
    $(document).on('click', '.btn-eliminar', function (e) {
        e.preventDefault();
        var url = $(this).data('url');
        Swal.fire({
            title: '¿Estás seguro?',
            text: "¡Esta acción no se puede deshacer!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#E27AB0',
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
    }

    // Materias Primas
    $('#cantidad, #volumen_de_porcion_de_presentacion, #unidad_de_medida_de_presentacion, #costo, #unidad_de_medida_del_peso, #merma_total_en_gramos')
        .on('input change', calcularCamposMateriaPrima);
    calcularCamposMateriaPrima();

    // Productos Preparados
    $('#cantidad, #volumen_de_porcion_de_presentacion, #unidad_de_medida_de_presentacion, #costo, #unidad_de_medida_del_peso')
        .on('input change', calcularCamposProductoPreparado);
    calcularCamposProductoPreparado();

    // Empaques y Decoraciones
    $('#costo, #cantidad').on('input change', mostrarCostoPorCantidadEmpaque);
    mostrarCostoPorCantidadEmpaque();

    // Implementos
    $('#costo, #cantidad').on('input change', mostrarCostoPorCantidadImplemento);
    mostrarCostoPorCantidadImplemento();

    // Suministros
    $('#costo, #cantidad').on('input change', mostrarCostoPorCantidadSuministro);
    mostrarCostoPorCantidadSuministro();

    // Recetas
    // Delegar eventos a los inputs y selects internos de materias primas y productos preparados
    $('#materias_primas-container').on('input change', 'input[name*=".cantidad"]', calcularCostosReceta);
    $('#materias_primas-container').on('change', 'select[name*="id_materia_prima_utilizada"]', calcularCostosReceta);

    $('#productos_preparados-container').on('input change', 'input[name*=".cantidad"]', calcularCostosReceta);
    $('#productos_preparados-container').on('change', 'select[name*="id_producto_preparado_utilizada"]', calcularCostosReceta);

    $('#porcion').on('input change', calcularCostosReceta);

    calcularCostosReceta();

    // Precio Final Sugerido
    $('#nombre_receta, #margen_de_utilidad, #plataforma_de_envio, #iva_porcentaje, #servicio_porcentaje, #empaques_decoraciones-container, #implementos-container, #suministros-container')
        .on('input change', calcularPrecioFinalProductoFinal);
    calcularPrecioFinalProductoFinal();
});