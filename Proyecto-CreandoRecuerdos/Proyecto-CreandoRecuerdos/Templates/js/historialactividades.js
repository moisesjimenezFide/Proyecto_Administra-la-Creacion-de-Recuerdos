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
        error: function (error) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Error al cargar las actividades: ' + error,
                confirmButtonColor: '#B54885'
            });
        }
    });
}

// Función para manejar la exportación del historial
function exportarHistorial(formato) {
    var exportUrl = $('#filtroForm').data('export-url');
    var data = $('#filtroForm').serialize() + '&formato=' + encodeURIComponent(formato);

    $.ajax({
        url: exportUrl,
        type: 'POST',
        data: data,
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data, status, xhr) {
            var disposition = xhr.getResponseHeader('Content-Disposition');
            var type = xhr.getResponseHeader('Content-Type');
            if (type && type.indexOf('application/json') !== -1) {
                var reader = new FileReader();
                reader.onload = function () {
                    var json = JSON.parse(reader.result);
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: json.message,
                        confirmButtonColor: '#B54885'
                    });
                };
                reader.readAsText(data);
            } else if (type && type.indexOf('text/html') !== -1) {
                var reader = new FileReader();
                reader.onload = function () {
                    $('body').append(reader.result);
                };
                reader.readAsText(data);
            } else if (disposition && disposition.indexOf('attachment') !== -1) {
                var filename = disposition.split('filename=')[1].replace(/"/g, '');
                var blob = new Blob([data], { type: type });
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = filename;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }
        },
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Error al exportar: ' + xhr.statusText,
                confirmButtonColor: '#B54885'
            });
        }
    });

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
            Swal.fire({
                icon: 'warning',
                title: 'Atención',
                text: 'Seleccione un formato de exportación.',
                confirmButtonColor: '#B54885'
            });
            return;
        }
        $('#btnExportar').html('<i class="fas fa-spinner fa-spin"></i> Exportando...');
        exportarHistorial(formato);
        $('#exportModal').modal('hide');
    });
});