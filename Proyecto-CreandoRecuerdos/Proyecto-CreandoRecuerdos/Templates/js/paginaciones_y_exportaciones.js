window.inicializarDataTables = function () {
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

      const columnasOcultas = tableElement.data('columnas-ocultas')
          ? tableElement.data('columnas-ocultas').toString().split(',').map(Number)
          : [];

      const tituloExportacion = tableElement.data('titulo-exportacion') || "Exportación";

      const totalColumnas = tableElement.find("thead th").length;

      // columnas visibles = todas excepto las ocultas
      const columnasVisibles = [...Array(totalColumnas).keys()].filter(i => !columnasOcultas.includes(i));

      // Botones de exportación
      const exportButtons = [

          // ========================== COPIAR ==========================
          {
              extend: 'copyHtml5',
              text: 'Copiar',
              className: 'btn btn-custom-pink',
              title: tituloExportacion,
              exportOptions: { columns: columnasVisibles }
          },

          // ========================== EXCEL ==========================
          {
              extend: 'excelHtml5',
              text: 'Exportar a Excel',
              className: 'btn btn-custom-pink',
              title: tituloExportacion,
              exportOptions: { columns: columnasVisibles },
          },

          // ========================== PDF ==========================
          {
              extend: 'pdfHtml5',
              text: 'Exportar a PDF',
              className: 'btn btn-custom-pink',
              title: tituloExportacion,
              orientation: 'landscape',
              pageSize: 'A4',
              exportOptions: { columns: columnasVisibles },

              customize: function (doc) {

                  // -- Color del título --
                  doc.styles.title = {
                      fontSize: 16,
                      bold: true,
                      color: '#B54885'
                  };

                  // -- Encabezado de tabla --
                  const headerColor = '#B54885';
                  const textWhite = '#FFFFFF';

                  doc.content[1].table.headerRows = 1;

                  doc.content[1].table.body.forEach(function (row, index) {
                      // primera fila = encabezado
                      if (index === 0) {
                          row.forEach(function (cell) {
                              cell.fillColor = headerColor;
                              cell.color = textWhite;
                              cell.bold = true;
                              cell.alignment = 'center';
                          });
                      }
                  });

                  // -- Márgenes --
                  doc.pageMargins = [30, 30, 30, 30];
              }
          },
      ];

        // Configuración de DataTables
        const dataTableConfig = {
            pageLength: pageLength,
            searching: hasSearch,
            pagingType: "full_numbers",
            lengthMenu: [valoresNumericos, valoresVisibles],
            language: {
                lengthMenu: "Mostrar _MENU_ registros por página",
                info: "Mostrando _START_ a _END_ de _TOTAL_ registros",
                infoEmpty: "No hay registros disponibles",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                emptyTable: "",
                zeroRecords: "",
                search: "Buscar:",
                paginate: {
                    first: "Primer página",
                    previous: "Página anterior",
                    next: "Siguiente Página",
                    last: "Última página",
                }
            }
        };

      // DOM dinámico según si hay botones de exportación
      if (hasExportButtons) {
          dataTableConfig.dom = "<'dt-toolbar d-flex align-items-center'B l f>rt<'dt-info-bar' i><'dt-paginate-bar' p>";
          dataTableConfig.buttons = exportButtons;
      } else {
          dataTableConfig.dom = "<'dt-toolbar d-flex align-items-center'l f>rt<'dt-info-bar' i><'dt-paginate-bar' p>";
      }

      // Destruir instancia previa si existe
      if ($.fn.DataTable.isDataTable(tableElement)) {
          tableElement.DataTable().destroy();
      }

      // Inicializa DataTables
      const table = tableElement.DataTable(dataTableConfig);

    function createCustomLengthDropdown() {
        var lengthContainer = tableElement.closest('.dataTables_wrapper').find('.dataTables_length');
        var originalSelect = lengthContainer.find('select');
        // Elimina el select original antes de agregar el dropdown personalizado
        if (originalSelect.length > 0) {
            originalSelect.remove();
        }
        if (lengthContainer.find('.custom-length-dropdown').length > 0) {
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
        lengthContainer.append(customDropdown);

        var label = lengthContainer.find('label');
        label.contents().filter(function () {
            return this.nodeType === 3;
        }).remove();
        label.prepend('Mostrar ');
        label.append(' registros por página');
    }

      // Llama a la función cada vez que la tabla se dibuja
        table.on('draw', function () {
            createCustomLengthDropdown();
        });

        // Llama la primera vez
        createCustomLengthDropdown();
  });
};

function formatearBotonesPaginacion() {
    $('.dataTables_paginate .paginate_button').each(function () {
        var txt = $(this).text().trim();
        if (txt === "Primer página") {
            $(this).html('Primer<br>página');
            $(this).addClass('btn-paginacion-larga');
        } else if (txt === "Página anterior") {
            $(this).html('Página<br>anterior');
            $(this).addClass('btn-paginacion-larga');
        } else if (txt === "Siguiente Página" || txt === "Siguiente página") {
            $(this).html('Siguiente<br>página');
            $(this).addClass('btn-paginacion-larga');
        } else if (txt === "Última página") {
            $(this).html('Última<br>página');
            $(this).addClass('btn-paginacion-larga');
        }
    });
}

// Ejecuta cada vez que se dibuja la tabla
$(document).on('draw.dt', function () {
    formatearBotonesPaginacion();
});

$(document).ready(function () {

    // Estiliza los botones de texto largo en la paginación
    formatearBotonesPaginacion();
    function estilizarBotonesPaginacion() {
        $('.dataTables_paginate .paginate_button').each(function () {
            var txt = $(this).text().trim();
            if (
                txt === "Primer página" ||
                txt === "Página anterior" ||
                txt === "Siguiente página" ||
                txt === "Última página"
            ) {
                $(this).addClass('btn-paginacion-larga');
            }
        });
    }

    // Llama la función cada vez que se dibuja la tabla
    $(document).on('draw.dt', function () {
        estilizarBotonesPaginacion();
    });

    // Llama la primera vez
    estilizarBotonesPaginacion();

    // Eventos para el dropdown personalizado para la cantidad de registros por página
    $(document).on('click', '.custom-dropdown-button', function (e) {
        e.preventDefault();
        e.stopPropagation();
        // Cierra otros menús y quita la clase .open de otros botones
        $('.custom-dropdown-menu').not($(this).siblings('.custom-dropdown-menu')).removeClass('show');
        $('.custom-dropdown-button').not(this).removeClass('open');
        // Alterna el menú actual
        $(this).siblings('.custom-dropdown-menu').toggleClass('show');
        // Si el menú está abierto, agrega la clase .open al botón
        if ($(this).siblings('.custom-dropdown-menu').hasClass('show')) {
            $(this).addClass('open');
        } else {
            $(this).removeClass('open');
        }
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
            $('.custom-dropdown-button').removeClass('open');
        }
    });

    $(document).on('contextmenu selectstart dragstart', '.custom-length-dropdown, .custom-dropdown-button, .custom-dropdown-item', function (e) {
        e.preventDefault();
        return false;
    });
});

// Inicialización global automática SOLO si hay tablas .dataTable en la vista
document.addEventListener('DOMContentLoaded', function () {
    if (window.inicializarDataTables) {
        $('.dataTable').not('.no-auto-dt').length && window.inicializarDataTables();
    }
});