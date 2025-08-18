$(document).ready(function () {
  $('.dataTable').each(function () {
        const tableElement = $(this);
        const tableId = '#' + tableElement.attr('id');
        const totalFilas = tableElement.find('tbody tr').length;
        const pageLength = tableElement.data('page-length') || 10;
        const hasExportButtons = tableElement.hasClass('con-exportaciones');
        const hasSearch = tableElement.hasClass('con-busqueda');

        // Opciones de cantidad de registros
        let opciones = [];
        for (let i = 5; i <= totalFilas; i += 5) {
            opciones.push(i);
        }
        const ultimoMultiplo = Math.floor(totalFilas / 5) * 5;
        const siguienteValor = totalFilas !== ultimoMultiplo ? totalFilas : null;
        if (siguienteValor && !opciones.includes(siguienteValor)) {
            opciones.push(siguienteValor);
        }
        opciones.sort((a, b) => a - b);
        const valoresNumericos = [...opciones, -1];
        const valoresVisibles = [...opciones.map(n => n.toString()), "Todos"];

        // Botones de exportación
        const exportButtons = [
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
        ];

        // Configuración de DataTables
        const dataTableConfig = {
            pageLength: pageLength,
            searching: hasSearch,
            pagingType: "full_numbers",
            lengthMenu: [valoresNumericos, valoresVisibles],
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
        };

        // Añade los botones y el 'dom' si la tabla tiene la clase 'con-exportaciones'
        if (hasExportButtons) {
            dataTableConfig.dom = "<'dt-buttons mb-2'B><'d-flex justify-content-between align-items-center mb-3'<'dt-length'l><'dataTables_filter'f>>rtip";
            dataTableConfig.buttons = exportButtons;
        }

        // Inicializa DataTables
        const table = tableElement.DataTable(dataTableConfig);

    function createCustomLengthDropdown() {
        var lengthContainer = tableElement.closest('.dataTables_wrapper').find('.dataTables_length');
        var originalSelect = lengthContainer.find('select');
        if (originalSelect.length === 0 || lengthContainer.find('.custom-length-dropdown').length > 0) {
            return;
        }

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
                    <div class="custom-dropdown-button" id="lengthDropdownBtn">${pageLength}</div>
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

      // Se llama a la función para cada tabla
      setTimeout(function () {
          createCustomLengthDropdown();
      }, 100);

  });

    // Eventos para el dropdown personalizado para la cantidad de registros por página
    $(document).on('click', '.custom-dropdown-button', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('.custom-dropdown-menu').not($(this).siblings('.custom-dropdown-menu')).removeClass('show');
        $(this).siblings('.custom-dropdown-menu').toggleClass('show');
    });

    $(document).on('click', '.custom-dropdown-item', function (e) {
        e.preventDefault();
        e.stopPropagation();

        var value = $(this).data('value');
        var text = $(this).text();

        // Se busca la instancia de la tabla asociada al dropdown
        const tableWrapper = $(this).closest('.dataTables_wrapper');
        const table = tableWrapper.find('.dataTable').DataTable();

        tableWrapper.find('.custom-dropdown-button').text(text);
        tableWrapper.find('.custom-dropdown-item').removeClass('active');
        $(this).addClass('active');
        tableWrapper.find('.custom-dropdown-menu').removeClass('show');
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
});