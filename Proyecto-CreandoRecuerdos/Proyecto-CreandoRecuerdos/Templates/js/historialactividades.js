function cargarActividades() {
    $.ajax({
        url: $('#filtroForm').attr('action'),
        type: 'POST',
        data: $('#filtroForm').serialize(),
        success: function (result) {
            $('#actividadesContainer').html(result);
        },
        error: function (xhr, status, error) {
            alert('Error al cargar las actividades: ' + error);
        }
    });
}

// Función para manejar la exportación
function exportarHistorial(formato) {

    var exportUrl = $('#filtroForm').data('export-url'); 
    var formData = $('#filtroForm').serialize();
    formData += '&formato=' + formato;

    $('#btnExportar').html('<i class="fas fa-spinner fa-spin"></i> Exportando...');

    // Crear formulario temporal
    var form = document.createElement('form');
    form.action = exportUrl; 
    form.method = 'POST';
    form.style.display = 'none';

    // Agregar campos del formulario
    $('#filtroForm').find('input, select, textarea').each(function () {
        if (this.name && (this.type !== 'checkbox' && this.type !== 'radio' || this.checked)) {
            var input = document.createElement('input');
            input.type = 'hidden';
            input.name = this.name;
            input.value = $(this).val();
            form.appendChild(input);
        }
    });

    // Agregar formato
    var formatoInput = document.createElement('input');
    formatoInput.type = 'hidden';
    formatoInput.name = 'formato';
    formatoInput.value = formato;
    form.appendChild(formatoInput);

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

    // Restaurar botón después de 1 segundo
    setTimeout(function () {
        $('#btnExportar').html('Exportar');
    }, 1000);
}

// Inicialización cuando el documento está listo
$(document).ready(function () {
    cargarActividades();

    $('#filtroForm').submit(function (e) {
        e.preventDefault();
        cargarActividades();
    });

    $('#btnExportar').click(function () {
        $('#exportModal').modal('show');
    });

    $('#confirmExport').click(function () {
        var formato = $('input[name="exportFormat"]:checked').val();
        exportarHistorial(formato);
        $('#exportModal').modal('hide');
    });
});