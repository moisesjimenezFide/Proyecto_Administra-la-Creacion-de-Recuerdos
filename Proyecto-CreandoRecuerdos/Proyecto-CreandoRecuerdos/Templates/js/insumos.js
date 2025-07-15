// =====================
// Materias Primas
// =====================
function calcularCamposMateriaPrima() {
    var costo = parseFloat($('#costo').val()) || 0;
    var peso = parseFloat($('#peso').val()) || 1;
    var merma = parseFloat($('#merma_total_en_gramos').val()) || 0;
    var costoPorGramo = (peso > 0) ? (costo / peso) : 0;
    var porcentajeMerma = (peso > 0) ? (merma / peso) * 100 : 0;
    var costoMermaTotal = costoPorGramo * merma;
    var costoTotalMasMerma = costo + costoMermaTotal;
    var costoPorGramoConMerma = (peso > 0) ? (costoTotalMasMerma / peso) : 0;

    $('#costo_por_gramo').val(costoPorGramo.toFixed(2));
    $('#porcentaje_de_merma').val(porcentajeMerma.toFixed(2));
    $('#costo_de_merma_total').val(costoMermaTotal.toFixed(2));
    $('#costo_total_mas_merma_total').val(costoTotalMasMerma.toFixed(2));
    $('#costo_por_gramo_con_merma').val(costoPorGramoConMerma.toFixed(2));
}

// =====================
// Productos Preparados
// =====================
function calcularCamposProductoPreparado() {
    var costo = parseFloat($('#costo').val()) || 0;
    var peso = parseFloat($('#peso').val()) || 1;
    var porcion = parseFloat($('#volumen_de_porcion').val()) || 1;
    var costoPorPeso = (peso > 0) ? (costo / peso) : 0;
    var costoPorPorcion = (porcion > 0) ? (costo / porcion) : 0;
    $('#costo_por_peso').text(costoPorPeso.toFixed(4));
    $('#costo_por_porcion_con_merma').text(costoPorPorcion.toFixed(4));
}

// =====================
// Empaques y Decoraciones
// =====================
function mostrarCostoPorCantidadEmpaque() {
    var costo = parseFloat($('#costo').val()) || 0;
    var cantidad = parseFloat($('#cantidad').val()) || 1;
    var resultado = (cantidad > 0) ? (costo / cantidad) : 0;
    $('#costoPorCantidad').val(resultado.toFixed(4));
}

// =====================
// Implementos
// =====================
function mostrarCostoPorCantidadImplemento() {
    var costo = parseFloat($('#costo').val()) || 0;
    var cantidad = parseFloat($('#cantidad').val()) || 1;
    var resultado = (cantidad > 0) ? (costo / cantidad) : 0;
    $('#costoPorCantidad').val(resultado.toFixed(4));
}

// =====================
// Suministros
// =====================
function mostrarCostoPorCantidadSuministro() {
    var costo = parseFloat($('#costo').val()) || 0;
    var cantidad = parseFloat($('#cantidad').val()) || 1;
    var resultado = (cantidad > 0) ? (costo / cantidad) : 0;
    $('#costoPorCantidad').val(resultado.toFixed(4));
}

// =====================
// Recetas
// =====================
function obtenerCostoPorGramo(nombre) {
    if (typeof materiasPrimas === "undefined") return 0;
    var found = materiasPrimas.find(x => x.nombre === nombre);
    return found ? parseFloat(found.costo_por_gramo) : 0;
}
function obtenerCostoPorPeso(nombre) {
    if (typeof productosPreparados === "undefined") return 0;
    var found = productosPreparados.find(x => x.nombre === nombre);
    return found ? parseFloat(found.costo_por_peso) : 0;
}
function calcularCostosReceta() {
    var total = 0;
    $('#materias_primas-container .fila-insumo:not(.template-materia_prima)').each(function () {
        var nombre = $(this).find('select[name="MateriaPrimaSeleccionada"]').val();
        var cantidad = parseFloat($(this).find('input[name="CantidadMateriaPrima"]').val()) || 0;
        var costo = obtenerCostoPorGramo(nombre);
        total += cantidad * costo;
    });
    $('#productos_preparados-container .fila-insumo:not(.template-producto_preparado)').each(function () {
        var nombre = $(this).find('select[name="ProductoPreparadoSeleccionado"]').val();
        var cantidad = parseFloat($(this).find('input[name="CantidadProductoPreparado"]').val()) || 0;
        var costo = obtenerCostoPorPeso(nombre);
        total += cantidad * costo;
    });
    $('#costoTotalReceta').text(total.toFixed(2));
    var porcion = parseFloat($('#porcion').val()) || 0;
    var costoPorPorcion = porcion > 0 ? total / porcion : 0;
    $('#costoPorPorcion').text(costoPorPorcion.toFixed(4));
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
    var nombreReceta = $('#nombre_receta').val();
    var costoTotalReceta = obtenerCostoReceta(nombreReceta);

    var margen_de_utilidad = parseFloat($('#margen_de_utilidad').val()) || 0;
    var costoSinUtilidad = costoTotalReceta;
    var costoConUtilidad = costoTotalReceta / ((100 - margen_de_utilidad) / 100);

    var totalEmpaques = calcularTotalesPorInsumo('#empaques_decoraciones-container', empaques);
    var totalImplementos = calcularTotalesPorInsumo('#implementos-container', implementos);
    var totalSuministros = calcularTotalesPorInsumo('#suministros-container', suministros);

    // 1. Costo de impresión de factura por insumo (suministro de impresión láser)
    var costoImpresionFacturaPorInsumo = 0;
    var suministroImpresionLaser = $('#suministros-container .fila-insumo:not([style*="display: none"]) input[name$=".costo_por_cantidad"]').first().val();
    if (suministroImpresionLaser) {
        costoImpresionFacturaPorInsumo = parseFloat(suministroImpresionLaser) / 20;
    }

    // 2. Costo total de impresión de factura
    var porcion = parseFloat($('#porcion').val()) || 0;
    var costoTotalImpresionFactura = porcion * costoImpresionFacturaPorInsumo;

    // 3. Factura
    var factura = totalImplementos + totalEmpaques + totalSuministros + costoTotalImpresionFactura;

    // 4. Costos por porcentaje de ganancia
    var costoEmpaqueDecoracionPorPorcentajeDeGanancia = factura * 0.10;
    var costoImplementoPorPorcentajeDeGanancia = factura * 0.10;
    var costoSuministroPorPorcentajeDeGanancia = factura * 0.10;

    // 5. Costo total empaque, implemento, suministro
    var costoTotalEmpaqueDecoracionImplementoSuministro =
        costoEmpaqueDecoracionPorPorcentajeDeGanancia +
        costoImplementoPorPorcentajeDeGanancia +
        costoSuministroPorPorcentajeDeGanancia;

    // 6. Factura por insumo
    var facturaPorInsumo = 0;
    $('#implementos-container .fila-insumo:not([style*="display: none"]) input[name$=".costo_por_cantidad"]').each(function () {
        facturaPorInsumo += parseFloat($(this).val()) || 0;
    });
    $('#empaques_decoraciones-container .fila-insumo:not([style*="display: none"]) input[name$=".costo_por_cantidad"]').each(function () {
        facturaPorInsumo += parseFloat($(this).val()) || 0;
    });
    $('#suministros-container .fila-insumo:not([style*="display: none"]) input[name$=".costo_por_cantidad"]').each(function () {
        facturaPorInsumo += parseFloat($(this).val()) || 0;
    });
    facturaPorInsumo += costoImpresionFacturaPorInsumo;

    // 7. Base para impuestos
    var baseImpuestos = costoConUtilidad + costoEmpaqueDecoracionPorPorcentajeDeGanancia + costoImplementoPorPorcentajeDeGanancia + costoSuministroPorPorcentajeDeGanancia;
    var iva = baseImpuestos * 0.13;
    var servicio = baseImpuestos * 0.10;

    // 8. Envío
    var plataforma = $('#plataforma_de_envio').val();
    var porcentajeEnvio = 0;
    switch (plataforma) {
        case "PedidosYa (25%)":
        case "Rappi (25%)":
            porcentajeEnvio = 0.25;
            break;
        case "DidiFood (30%)":
            porcentajeEnvio = 0.30;
            break;
        case "UberEats (40%)":
            porcentajeEnvio = 0.40;
            break;
        case "Propio (0%)":
        default:
            porcentajeEnvio = 0.0;
            break;
    }
    var envio = baseImpuestos * porcentajeEnvio;

    // 9. Precio final sugerido
    var precioFinal = baseImpuestos + iva + servicio + envio;

    // Mostrar resultados en la vista
    $('#costo_total_receta').val(costoTotalReceta.toFixed(2));
    $('#costo_sin_margen_de_utilidad').val(costoSinUtilidad.toFixed(2));
    $('#costo_con_margen_de_utilidad').val(costoConUtilidad.toFixed(2));
    $('#costo_empaque_decoracion_utilizado').val(totalEmpaques.toFixed(2));
    $('#costo_implemento_utilizado').val(totalImplementos.toFixed(2));
    $('#costo_suministro_utilizado').val(totalSuministros.toFixed(2));
    $('#costo_de_impresion_de_factura_por_insumo').val(costoImpresionFacturaPorInsumo.toFixed(2));
    $('#costo_total_de_impresion_de_factura').val(costoTotalImpresionFactura.toFixed(2));
    $('#factura').val(factura.toFixed(2));
    $('#costo_empaque_decoracion_por_porcentaje_de_ganancia').val(costoEmpaqueDecoracionPorPorcentajeDeGanancia.toFixed(2));
    $('#costo_implemento_por_porcentaje_de_ganancia').val(costoImplementoPorPorcentajeDeGanancia.toFixed(2));
    $('#costo_suministro_por_porcentaje_de_ganancia').val(costoSuministroPorPorcentajeDeGanancia.toFixed(2));
    $('#costo_total_empaque_decoracion_implemento_suministro').val(costoTotalEmpaqueDecoracionImplementoSuministro.toFixed(2));
    $('#factura_por_insumo').val(facturaPorInsumo.toFixed(2));
    $('#iva').val(iva.toFixed(2));
    $('#impuesto_de_servicio').val(servicio.toFixed(2));
    $('#envio').val(envio.toFixed(2));
    $('#precio_final_sugerido').val(precioFinal.toFixed(2));
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
    // Materias Primas
    if ($('#costo_por_gramo').length) {
        $('#costo, #peso, #merma_total_en_gramos').on('input', calcularCamposMateriaPrima);
        $('form').on('submit', validarMateriaPrimaForm);
        calcularCamposMateriaPrima();
    }

    // Productos Preparados
    if ($('#costo_por_peso').length) {
        $('#costo, #peso, #volumen_de_porcion').on('input', calcularCamposProductoPreparado);
        $('form').on('submit', validarProductoPreparadoForm);
        calcularCamposProductoPreparado();
    }

    // Empaques y Decoraciones
    if ($('#costoPorCantidad').length && $('body').text().includes('Empaques y/o Decoraciones')) {
        $('#costo, #cantidad').on('input', mostrarCostoPorCantidadEmpaque);
        $('form').on('submit', validarEmpaqueDecoracionForm);
        mostrarCostoPorCantidadEmpaque();
    }

    // Implementos
    if ($('#costoPorCantidad').length && $('body').text().includes('Implementos')) {
        $('#costo, #cantidad').on('input', mostrarCostoPorCantidadImplemento);
        $('form').on('submit', validarImplementoForm);
        mostrarCostoPorCantidadImplemento();
    }

    // Suministros
    if ($('#costoPorCantidad').length && $('body').text().includes('Suministros')) {
        $('#costo, #cantidad').on('input', mostrarCostoPorCantidadSuministro);
        $('form').on('submit', validarSuministroForm);
        mostrarCostoPorCantidadSuministro();
    }

    // Recetas
    if ($('#costoTotalReceta').length) {
        $(document).on('input change', '#materias_primas-container input, #materias_primas-container select, #productos_preparados-container input, #productos_preparados-container select, #porcion', calcularCostosReceta);
        calcularCostosReceta();
    }

    // Productos Finales (precio_final)
    if ($('#precio_final_sugerido').length) {
        $(document).on('change', '#nombre_receta', calcularPrecioFinalProductoFinal);
        $(document).on('input', '#margen_de_utilidad', calcularPrecioFinalProductoFinal);
        $(document).on('input change', '#empaques_decoraciones-container input, #empaques_decoraciones-container select', calcularPrecioFinalProductoFinal);
        $(document).on('input change', '#implementos-container input, #implementos-container select', calcularPrecioFinalProductoFinal);
        $(document).on('input change', '#suministros-container input, #suministros-container select', calcularPrecioFinalProductoFinal);
        calcularPrecioFinalProductoFinal();
    }

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
});