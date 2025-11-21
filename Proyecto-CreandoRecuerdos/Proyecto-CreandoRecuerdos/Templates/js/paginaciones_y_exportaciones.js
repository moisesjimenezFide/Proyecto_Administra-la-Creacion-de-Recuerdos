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
                customize: function (xlsx) {
                    // 1) Referencias a sheet y styles
                    const sheet = xlsx.xl.worksheets['sheet1.xml'];
                    const styles = xlsx.xl['styles.xml'];

                    // 2) jQuery XML wrappers
                    const $sheet = $(sheet);
                    const $styles = $(styles);

                    // 3) Asegurar nodos base en styles.xml
                    const ensureNode = (selector, xml) => {
                        if ($styles.find(selector).length === 0) {
                            $styles.find('styleSheet').append(xml);
                        }
                        return $styles.find(selector);
                    };

                    // Crear secciones si no existen
                    ensureNode('fonts', '<fonts count="0"></fonts>');
                    ensureNode('fills', '<fills count="0"></fills>');
                    ensureNode('borders', '<borders count="0"></borders>');
                    ensureNode('cellXfs', '<cellXfs count="0"></cellXfs>');

                    const $fonts = $styles.find('fonts');
                    const $fills = $styles.find('fills');
                    const $borders = $styles.find('borders');
                    const $cellXfs = $styles.find('cellXfs');

                    // 4) Añadir recursos de estilos (font, fill, border)
                    
                    // Fuente para título (tamaño 20, negrita, centrado)
                    $fonts.append(
                        '<font><sz val="20"/><color rgb="FFFFFFFF"/><name val="Calibri"/><b/></font>'
                    );
                    const titleFontId = $fonts.find('font').length - 1;

                    // Relleno para título (rosa oscuro #B54885)
                    $fills.append(
                        '<fill><patternFill patternType="solid"><fgColor rgb="FFB54885"/></patternFill></fill>'
                    );
                    const titleFillId = $fills.find('fill').length - 1;

                    // Fuente encabezado (blanca, negrita, 14)
                    $fonts.append(
                        '<font><sz val="14"/><color rgb="FFFFFFFF"/><name val="Calibri"/><b/></font>'
                    );
                    const headerFontId = $fonts.find('font').length - 1;

                    // Fuente filas (oscura, 12)
                    $fonts.append(
                        '<font><sz val="12"/><color rgb="FF2C2C2C"/><name val="Calibri"/></font>'
                    );
                    const rowFontId = $fonts.find('font').length - 1;

                    // Rellenos: rosa oscuro, blanco, rosa pastel
                    $fills.append(
                        '<fill><patternFill patternType="solid"><fgColor rgb="FFB54885"/></patternFill></fill>'
                    );
                    const darkPinkFillId = $fills.find('fill').length - 1;

                    $fills.append(
                        '<fill><patternFill patternType="solid"><fgColor rgb="FFFFFFFF"/></patternFill></fill>'
                    );
                    const whiteFillId = $fills.find('fill').length - 1;

                    $fills.append(
                        '<fill><patternFill patternType="solid"><fgColor rgb="FFF9F9F9"/></patternFill></fill>'
                    );
                    const grayFillId = $fills.find('fill').length - 1;

                    // Borde completo en color oscuro #2C2C2C
                    $borders.append(
                        '<border>' +
                        '<left style="thin"><color rgb="FF2C2C2C"/></left>' +
                        '<right style="thin"><color rgb="FF2C2C2C"/></right>' +
                        '<top style="thin"><color rgb="FF2C2C2C"/></top>' +
                        '<bottom style="thin"><color rgb="FF2C2C2C"/></bottom>' +
                        '</border>'
                    );
                    const darkBorderId = $borders.find('border').length - 1;

                    // 5) Crear estilos de celda (cellXfs)
                    const addXf = (fontId, fillId, borderId, alignCenter) => {
                        const alignment = alignCenter
                            ? '<alignment horizontal="center" vertical="center"/>'
                            : '<alignment vertical="center"/>';
                        const xf =
                            `<xf xfId="0" fontId="${fontId}" fillId="${fillId}" borderId="${borderId}" applyFont="1" applyFill="1" applyBorder="1">${alignment}</xf>`;
                        $cellXfs.append(xf);
                        return $cellXfs.find('xf').length - 1;
                    };

                    // Estilo para título
                    const titleStyleId = addXf(titleFontId, titleFillId, darkBorderId, true);

                    // Estilo para encabezado
                    const headerStyleId = addXf(headerFontId, darkPinkFillId, darkBorderId, true);

                    // Estilos para filas alternas
                    const rowWhiteStyleId = addXf(rowFontId, whiteFillId, darkBorderId, true);
                    const rowPastelStyleId = addXf(rowFontId, grayFillId, darkBorderId, true);

                    // Actualizar atributos count
                    $fonts.attr('count', $fonts.find('font').length);
                    $fills.attr('count', $fills.find('fill').length);
                    $borders.attr('count', $borders.find('border').length);
                    $cellXfs.attr('count', $cellXfs.find('xf').length);

                    // 6) Aplicar estilos
                    // Título: primera fila
                    $sheet.find('sheetData row').first().find('c').attr('s', titleStyleId);

                    // Encabezados de columnas (segunda fila)
                    $sheet.find('sheetData row').eq(1).find('c').attr('s', headerStyleId);

                    // Filas alternas de datos (desde la tercera fila en adelante)
                    $sheet.find('sheetData row').slice(2).each(function (idx, row) {
                        const styleId = idx % 2 === 0 ? rowWhiteStyleId : rowPastelStyleId;
                        $(row).find('c').attr('s', styleId);
                    });

                }
            },

            // ========================== PDF ==========================
            {
                extend: 'pdfHtml5',
                text: 'Exportar a PDF',
                className: 'btn btn-custom-pink',
                title: tituloExportacion,
                pageSize: 'TABLOID',
                orientation: 'landscape',
                exportOptions: { columns: columnasVisibles },

                customize: function (doc) {

                    // === 1. Página TABLOID horizontal ===
                    doc.pageSize = {
                        width: 1284,
                        height: 1684
                    };

                    // === 2. Márgenes ===
                    doc.pageMargins = [20, 20, 20, 20];

                    // === 3. Guardamos tabla original ===
                    var originalTable = doc.content[1].table;
                    var colCount = originalTable.body[0].length;

                    // === 4. Centrar todas las celdas ===
                    originalTable.body.forEach(row => {
                        row.forEach(cell => {
                            if (cell && typeof cell === "object") {
                                cell.alignment = "center";
                            }
                        });
                    });

                    // === 5. REEMPLAZAR contenido por columnas que empujan la tabla al centro ===
                    doc.content = [

                        // Título centrado
                        {
                            text: tituloExportacion,
                            style: 'title',
                            alignment: 'center',
                            bold: true,
                            fontSize: 20,
                            margin: [0, 0, 0, 15]
                        },

                        // Tabla centrada con columnas fantasma
                        {
                            columns: [
                                { width: '*', text: '' },  // empuja desde la izquierda
                                {
                                    width: 'auto',
                                    table: {
                                        widths: Array(colCount).fill('auto'),
                                        body: originalTable.body
                                    },
                                    layout: {
                                        hLineWidth: () => 0.3,
                                        vLineWidth: () => 0.3,
                                        hLineColor: () => '#999',
                                        vLineColor: () => '#999',
                                        paddingLeft: () => 2,
                                        paddingRight: () => 2,
                                        paddingTop: () => 2,
                                        paddingBottom: () => 2
                                    }
                                },
                                { width: '*', text: '' }   // empuja desde la derecha
                            ]
                        }
                    ];

                    // === 6. Fuente global ===
                    doc.defaultStyle = {
                        font: 'DejaVuSans',
                        fontSize: 12
                    };

                    // === 7. Encabezado pastel centrado ===
                    doc.styles.tableHeader = {
                        fillColor: '#B54885',
                        color: 'white',
                        bold: true,
                        alignment: 'center',
                        fontSize: 14
                    };
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