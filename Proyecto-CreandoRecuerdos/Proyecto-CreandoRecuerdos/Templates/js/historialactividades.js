// Función para cargar actividades filtradas en la tabla
function cargarActividades() {
    $.ajax({
        url: $('#filtroForm').attr('action'),
        type: 'POST',
        data: $('#filtroForm').serialize(),
        success: function (result) {
            $('#actividadesContainer').html(result);
            // Inicializar DataTables si está disponible
            if (window.inicializarDataTables) {
                window.inicializarDataTables();
            }
        },
        error: function (xhr, status, error) {
            alert('Error al cargar las actividades: ' + error);
        }
    });
}

// Función para manejar la exportación del historial
function exportarHistorial(formato) {
    var exportUrl = $('#filtroForm').data('export-url');
    var form = document.createElement('form');
    form.action = exportUrl;
    form.method = 'POST';
    form.style.display = 'none';

    // Agregar campos del formulario de filtros
    $('#filtroForm').find('input, select, textarea').each(function () {
        if (this.name && (this.type !== 'checkbox' && this.type !== 'radio' || this.checked)) {
            var input = document.createElement('input');
            input.type = 'hidden';
            input.name = this.name;
            input.value = $(this).val();
            form.appendChild(input);
        }
    });

    // Agregar campo de formato
    var formatoInput = document.createElement('input');
    formatoInput.type = 'hidden';
    formatoInput.name = 'formato';
    formatoInput.value = formato;
    form.appendChild(formatoInput);

    // Agregar token antifalsificación si existe
    var token = $('input[name="__RequestVerificationToken"]').val();
    if (token) {
        var tokenInput = document.createElement('input');
        tokenInput.type = 'hidden';
        tokenInput.name = '__RequestVerificationToken';
        tokenInput.value = token;
        form.appendChild(tokenInput);
    }

    document.body.appendChild(form);
    form.submit();
    document.body.removeChild(form);

    // Restaurar el texto del botón después de 1 segundo
    setTimeout(function () {
        $('#btnExportar').html('Exportar');
    }, 1000);
}

// Inicialización al cargar el documento
$(document).ready(function () {
    // Cargar actividades al inicio
    cargarActividades();

    // Manejar el envío del formulario de filtros por AJAX
    $('#filtroForm').submit(function (e) {
        e.preventDefault();
        cargarActividades();
    });

    // Mostrar el modal de exportación al hacer clic en el botón Exportar
    $('#btnExportar').click(function () {
        $('#exportModal').modal('show');
    });

    // Ejecutar la exportación al confirmar el formato
    $('#confirmExport').click(function () {
        var formato = $('input[name="exportFormat"]:checked').val();
        if (!formato) {
            alert('Seleccione un formato de exportación.');
            return;
        }
        $('#btnExportar').html('<i class="fas fa-spinner fa-spin"></i> Exportando...');
        exportarHistorial(formato);
        $('#exportModal').modal('hide');
    });
});