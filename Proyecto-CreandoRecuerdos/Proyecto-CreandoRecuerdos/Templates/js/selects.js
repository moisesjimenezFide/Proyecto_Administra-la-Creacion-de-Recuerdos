// Función para convertir un elemento select en un dropdown personalizado
function convertToCustomDropdown(selectElement) {
    const $select = $(selectElement);

    if ($select.closest('.custom-select').length) {
        return;
    }

    const selectedText = $select.find('option:selected').text();
    const $wrapper = $('<div class="custom-select"></div>');

    $select.addClass('custom-hidden');
    $select.wrap($wrapper);

    const $button = $('<button type="button" class="form-control custom-select-button"></button>').text(selectedText);
    const $menu = $('<div class="custom-select-menu"></div>');

    $select.find('option').each(function () {
        const $option = $(this);
        const $item = $('<a class="dropdown-item custom-select-item" href="#"></a>');

        if (typeof agregarAtributosInsumos === 'function') {
            agregarAtributosInsumos($option, $item);
        } else {
            $item.attr('data-value', $option.val());
            $item.text($option.text());
            if ($option.is(':selected')) {
                $item.addClass('active');
            }
        }

        $menu.append($item);
    });

    $select.parent().append($button).append($menu);
}

// CLASE PRINCIPAL QUE MANEJA CADA SELECTOR DE FECHA
class CustomDatePicker {
    constructor(container) {
        this.container = container;
        this.inputId = container.dataset.input;
        this.input = document.getElementById(this.inputId);
        this.button = container.querySelector('.datepicker-input');
        this.menu = container.querySelector('.datepicker-calendar');

        this.currentDate = new Date();
        this.selectedDate = null;
        this.locale = 'es-ES';

        this.init();
    }

    init() {
        this.button.addEventListener('click', () => this.toggleCalendar());
        document.addEventListener('click', (e) => this.handleOutsideClick(e));
        this.input.addEventListener('change', () => this.updateFromInput());

        if (this.input.value) {
            // Parsear dd-MM-yyyy a Date
            const partes = this.input.value.split('-');
            if (partes.length === 3) {
                const dia = parseInt(partes[0], 10);
                const mes = parseInt(partes[1], 10) - 1;
                const anio = parseInt(partes[2], 10);
                const fecha = new Date(anio, mes, dia);
                if (!isNaN(fecha.getTime())) {
                    this.selectedDate = fecha;
                    this.currentDate = new Date(fecha);
                }
            }
            this.updateButton();
        }
    }

    toggleCalendar() {
        if (this.menu.classList.contains('show')) {
            this.hideCalendar();
        } else {
            this.showCalendar();
        }
    }

    showCalendar() {
        this.menu.classList.add('show');
        this.renderCalendar();
    }

    hideCalendar() {
        this.menu.classList.remove('show');
    }

    handleOutsideClick(e) {
        if (!this.container.contains(e.target)) {
            this.hideCalendar();
        }
    }

    renderCalendar() {
        const year = this.currentDate.getFullYear();
        const month = this.currentDate.getMonth();

        const currentMonthName = new Date(year, month, 1).toLocaleDateString(this.locale, { month: 'long' });
        const capitalizedMonth = currentMonthName.charAt(0).toUpperCase() + currentMonthName.slice(1);

        this.menu.innerHTML = `
                <div class="calendar-header">
                    <div class="calendar-title">
                        <div class="calendar-title-part" data-type="month">
                            ${capitalizedMonth} ▼
                            <div class="calendar-dropdown" data-dropdown="month">
                                ${this.renderMonthDropdown(month)}
                            </div>
                        </div>
                        <div class="calendar-title-part" data-type="year">
                            ${year} ▼
                            <div class="calendar-dropdown" data-dropdown="year">
                                ${this.renderYearDropdown(year)}
                            </div>
                        </div>
                    </div>
                </div>
                <div class="calendar-grid">
                    ${this.renderDayHeaders()}
                    ${this.renderDays()}
                </div>
            `;

        this.attachCalendarEvents();
    }

    renderMonthDropdown(currentMonth) {
        const months = [];
        for (let i = 0; i < 12; i++) {
            const date = new Date(2024, i, 1);
            const monthName = date.toLocaleDateString(this.locale, { month: 'long' });
            months.push(`<div class="calendar-dropdown-item ${i === currentMonth ? 'selected' : ''}" data-month="${i}">
                    ${monthName.charAt(0).toUpperCase() + monthName.slice(1)}
                </div>`);
        }
        return months.join('');
    }

    renderYearDropdown(currentYear) {
        const startYear = currentYear - 10;
        const endYear = currentYear + 10;
        let years = '';

        for (let year = startYear; year <= endYear; year++) {
            years += `<div class="calendar-dropdown-item ${year === currentYear ? 'selected' : ''}" data-year="${year}">
                    ${year}
                </div>`;
        }

        return years;
    }

    attachCalendarEvents() {
        this.menu.querySelectorAll('.calendar-title-part').forEach(titlePart => {
            const dropdown = titlePart.querySelector('.calendar-dropdown');

            titlePart.addEventListener('click', (e) => {
                e.stopPropagation();
                this.menu.querySelectorAll('.calendar-dropdown').forEach(dd => {
                    if (dd !== dropdown) dd.classList.remove('show');
                });
                dropdown.classList.toggle('show');
            });
        });

        this.menu.querySelectorAll('[data-dropdown="month"] .calendar-dropdown-item').forEach(item => {
            item.addEventListener('click', (e) => {
                e.stopPropagation();
                const month = parseInt(item.dataset.month);
                this.currentDate.setMonth(month);
                this.renderCalendar();
            });
        });

        this.menu.querySelectorAll('[data-dropdown="year"] .calendar-dropdown-item').forEach(item => {
            item.addEventListener('click', (e) => {
                e.stopPropagation();
                const year = parseInt(item.dataset.year);
                this.currentDate.setFullYear(year);
                this.renderCalendar();
            });
        });

        // Solo días no deshabilitados pueden ser seleccionados
        this.menu.querySelectorAll('.calendar-day[data-date]:not(.disabled)').forEach(day => {
            day.addEventListener('click', () => {
                const dateStr = day.dataset.date;
                this.selectDate(new Date(dateStr + 'T00:00:00'));
            });
        });
    }

    renderDayHeaders() {
        const dayHeaders = [];
        for (let i = 0; i < 7; i++) {
            const date = new Date(2024, 0, i);
            date.setDate(date.getDate() - date.getDay() + i);
            const dayName = date.toLocaleDateString(this.locale, { weekday: 'short' });
            dayHeaders.push(`<div class="calendar-day-header">${dayName.charAt(0).toUpperCase() + dayName.slice(1)}</div>`);
        }
        return dayHeaders.join('');
    }

    renderDays() {
        const year = this.currentDate.getFullYear();
        const month = this.currentDate.getMonth();
        const firstDay = new Date(year, month, 1);
        const lastDay = new Date(year, month + 1, 0);
        const startDate = new Date(firstDay);
        startDate.setDate(startDate.getDate() - firstDay.getDay());

        const days = [];
        const today = new Date();

        // Detectar si se debe restringir a fechas futuras
        const minToday = this.container.hasAttribute('data-min-today');

        for (let i = 0; i < 42; i++) {
            const date = new Date(startDate);
            date.setDate(startDate.getDate() + i);

            const isCurrentMonth = date.getMonth() === month;
            const isToday = date.toDateString() === today.toDateString();
            const isSelected = this.selectedDate && date.toDateString() === this.selectedDate.toDateString();

            let classes = ['calendar-day'];
            if (!isCurrentMonth) classes.push('other-month');
            if (isToday) classes.push('today');
            if (isSelected) classes.push('selected');

            // Solo deshabilitar días pasados si minToday está activo
            let disabled = false;
            if (minToday && date < today) {
                classes.push('disabled');
                disabled = true;
            }

            const dateStr = date.getFullYear() + '-' +
                String(date.getMonth() + 1).padStart(2, '0') + '-' +
                String(date.getDate()).padStart(2, '0');

            days.push(`
                    <div class="${classes.join(' ')}" data-date="${dateStr}">
                        ${date.getDate()}
                    </div>
                `);
        }

        return days.join('');
    }

    selectDate(date) {
        this.selectedDate = date;
        this.currentDate = new Date(date);

        const dateStr = String(date.getDate()).padStart(2, '0') + '-' +
            String(date.getMonth() + 1).padStart(2, '0') + '-' +
            date.getFullYear();

        this.input.value = dateStr;
        this.input.dispatchEvent(new Event('change'));
        this.updateButton();
        this.hideCalendar();
    }

    updateButton() {
        const iconHtml = '<i class="fas fa-calendar-days calendar-icon"></i>';

        if (this.selectedDate) {
            const formattedDate = this.selectedDate.toLocaleDateString(this.locale, {
                day: 'numeric',
                month: 'long',
                year: 'numeric'
            });
            const capitalizedDate = formattedDate.charAt(0).toUpperCase() + formattedDate.slice(1);
            this.button.innerHTML = `${capitalizedDate}${iconHtml}`;
        } else {
            const defaultText = this.inputId === 'FechaInicio' ? 'Seleccionar fecha de inicio' : 'Seleccionar fecha final';
            this.button.innerHTML = `${defaultText}${iconHtml}`;
        }
    }

    updateFromInput() {
        if (this.input.value) {
            // Parsear dd-MM-yyyy a Date
            const partes = this.input.value.split('-');
            if (partes.length === 3) {
                // partes[0]=día, partes[1]=mes, partes[2]=año
                const dia = parseInt(partes[0], 10);
                const mes = parseInt(partes[1], 10) - 1; // JS: 0=enero
                const anio = parseInt(partes[2], 10);
                const fecha = new Date(anio, mes, dia);
                if (!isNaN(fecha.getTime())) {
                    this.selectedDate = fecha;
                    this.currentDate = new Date(fecha);
                } else {
                    this.selectedDate = null;
                }
            } else {
                this.selectedDate = null;
            }
        } else {
            this.selectedDate = null;
        }
        this.updateButton();
    }
}

// CLASE PRINCIPAL QUE MANEJA CADA SELECTOR DE HORA
class CustomTimePicker {
    constructor(container) {
        this.container = container;
        this.inputId = container.dataset.input;
        this.input = document.getElementById(this.inputId);
        this.button = container.querySelector('.timepicker-input');
        this.menu = container.querySelector('.timepicker-selector');

        this.selectedHour = null;
        this.selectedMinute = null;
        this.currentHour = new Date().getHours();
        this.currentMinute = new Date().getMinutes();

        this.init();
    }

    init() {
        this.button.addEventListener('click', () => this.toggleSelector());
        document.addEventListener('click', (e) => this.handleOutsideClick(e));
        this.input.addEventListener('change', () => this.updateFromInput());

        if (this.input.value) {
            // Parsear HH:MM
            const partes = this.input.value.split(':');
            if (partes.length === 2) {
                const hora = parseInt(partes[0], 10);
                const minuto = parseInt(partes[1], 10);
                if (!isNaN(hora) && !isNaN(minuto) && hora >= 0 && hora <= 23 && minuto >= 0 && minuto <= 59) {
                    this.selectedHour = hora;
                    this.selectedMinute = minuto;
                    this.currentHour = hora;
                    this.currentMinute = minuto;
                }
            }
            this.updateButton();
        }
    }

    toggleSelector() {
        if (this.menu.classList.contains('show')) {
            this.hideSelector();
        } else {
            this.showSelector();
        }
    }

    showSelector() {
        this.menu.classList.add('show');
        this.renderSelector();
    }

    hideSelector() {
        this.menu.classList.remove('show');
    }

    handleOutsideClick(e) {
        if (!this.container.contains(e.target)) {
            this.hideSelector();
        }
    }

    renderSelector() {
        const displayHour = this.selectedHour !== null ? String(this.selectedHour).padStart(2, '0') : String(this.currentHour).padStart(2, '0');
        const displayMinute = this.selectedMinute !== null ? String(this.selectedMinute).padStart(2, '0') : String(this.currentMinute).padStart(2, '0');

        this.menu.innerHTML = `
                    <div class="selector-header">
                        <div class="selector-title">
                            <div class="selector-title-part" data-type="hour">
                                ${displayHour} ▼
                                <div class="selector-dropdown" data-dropdown="hour">
                                    ${this.renderHourDropdown()}
                                </div>
                            </div>
                            <span style="font-weight: 600; color: #2C2C2C;">:</span>
                            <div class="selector-title-part" data-type="minute">
                                ${displayMinute} ▼
                                <div class="selector-dropdown" data-dropdown="minute">
                                    ${this.renderMinuteDropdown()}
                                </div>
                            </div>
                        </div>
                    </div>
                    <button type="button" class="time-confirm-btn">
                        <i class="fas fa-check"></i> Confirmar Hora
                    </button>
                `;

        // Guardar referencia para el botón
        this.menu.timePicker = this;
        this.attachSelectorEvents();
    }

    renderHourDropdown() {
        let hours = '';
        const currentHour = this.selectedHour !== null ? this.selectedHour : this.currentHour;

        for (let i = 0; i < 24; i++) {
            const hourStr = String(i).padStart(2, '0');
            hours += `<div class="selector-dropdown-item ${i === currentHour ? 'selected' : ''}" data-hour="${i}">
                        ${hourStr}
                    </div>`;
        }
        return hours;
    }

    renderMinuteDropdown() {
        let minutes = '';
        const currentMinute = this.selectedMinute !== null ? this.selectedMinute : this.currentMinute;

        for (let i = 0; i < 60; i++) {
            const minuteStr = String(i).padStart(2, '0');
            minutes += `<div class="selector-dropdown-item ${i === currentMinute ? 'selected' : ''}" data-minute="${i}">
                        :${minuteStr}
                    </div>`;
        }
        return minutes;
    }

    attachSelectorEvents() {
        this.menu.querySelectorAll('.selector-title-part').forEach(titlePart => {
            const dropdown = titlePart.querySelector('.selector-dropdown');

            titlePart.addEventListener('click', (e) => {
                e.stopPropagation();
                this.menu.querySelectorAll('.selector-dropdown').forEach(dd => {
                    if (dd !== dropdown) dd.classList.remove('show');
                });
                dropdown.classList.toggle('show');
            });
        });

        this.menu.querySelectorAll('[data-dropdown="hour"] .selector-dropdown-item').forEach(item => {
            item.addEventListener('click', (e) => {
                e.stopPropagation();
                const hour = parseInt(item.dataset.hour);
                this.currentHour = hour;
                this.selectedHour = hour;
                this.renderSelector(); // Solo actualiza la vista, NO confirma ni cierra
            });
        });

        this.menu.querySelectorAll('[data-dropdown="minute"] .selector-dropdown-item').forEach(item => {
            item.addEventListener('click', (e) => {
                e.stopPropagation();
                const minute = parseInt(item.dataset.minute);
                this.currentMinute = minute;
                this.selectedMinute = minute;
                this.renderSelector(); // Solo actualiza la vista, NO confirma ni cierra
            });
        });

        // Evita el submit del formulario al confirmar la hora
        this.menu.querySelector('.time-confirm-btn').addEventListener('click', (e) => {
            e.preventDefault();
            this.confirmSelection();
        });
    }

    confirmSelection() {
        this.selectedHour = this.currentHour;
        this.selectedMinute = this.currentMinute;

        const timeStr = String(this.selectedHour).padStart(2, '0') + ':' +
            String(this.selectedMinute).padStart(2, '0');

        this.input.value = timeStr;
        this.input.dispatchEvent(new Event('change'));
        this.updateButton();
        this.hideSelector();
    }

    updateButton() {
        const iconHtml = '<i class="fas fa-clock clock-icon"></i>';

        if (this.selectedHour !== null && this.selectedMinute !== null) {
            const timeStr = String(this.selectedHour).padStart(2, '0') + ':' +
                String(this.selectedMinute).padStart(2, '0');
            this.button.innerHTML = `${timeStr}${iconHtml}`;
        } else {
            const defaultText = this.inputId === 'HoraInicio' ? 'Seleccionar hora de inicio' : 'Seleccionar hora final';
            this.button.innerHTML = `${defaultText}${iconHtml}`;
        }
    }

    updateFromInput() {
        if (this.input.value) {
            // Parsear HH:MM
            const partes = this.input.value.split(':');
            if (partes.length === 2) {
                const hora = parseInt(partes[0], 10);
                const minuto = parseInt(partes[1], 10);
                if (!isNaN(hora) && !isNaN(minuto) && hora >= 0 && hora <= 23 && minuto >= 0 && minuto <= 59) {
                    this.selectedHour = hora;
                    this.selectedMinute = minuto;
                    this.currentHour = hora;
                    this.currentMinute = minuto;
                } else {
                    this.selectedHour = null;
                    this.selectedMinute = null;
                }
            } else {
                this.selectedHour = null;
                this.selectedMinute = null;
            }
        } else {
            this.selectedHour = null;
            this.selectedMinute = null;
        }
        this.updateButton();
    }
}

// INICIALIZACIÓN CUANDO CARGA LA PÁGINA
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.custom-datepicker[data-input]').forEach(container => {
        new CustomDatePicker(container);
    });

    document.querySelectorAll('.custom-timepicker[data-input]').forEach(container => {
        new CustomTimePicker(container);
    });

    // Evento submit del formulario
    const form = document.getElementById('horarioForm');
    if (form) {
        form.addEventListener('submit', function (e) {
            // Forzar confirmación de hora si el selector está abierto
            document.querySelectorAll('.custom-timepicker').forEach(picker => {
                const menu = picker.querySelector('.timepicker-selector');
                if (menu.classList.contains('show') && menu.timePicker) {
                    menu.timePicker.confirmSelection();
                }
            });
        });
    }
});

// Variable para generar IDs únicos
let contadorId = 6;

// Función para guardar horario
function guardarHorario() {
    const empleadoSelect = document.getElementById('empleadoSelect');
    const horaEntrada = document.getElementById('hora_entrada').value;
    const horaSalida = document.getElementById('hora_salida').value;

    // Obtener días seleccionados
    const diasSeleccionados = [];
    document.querySelectorAll('input[name="dias_semana"]:checked').forEach(checkbox => {
        diasSeleccionados.push(checkbox.value);
    });

    // Validar que todos los campos estén completos
    if (!empleadoSelect.value || diasSeleccionados.length === 0 || !horaEntrada || !horaSalida) {
        mostrarMensaje('Por favor complete todos los campos', 'error');
        return;
    }

    const empleadoNombre = empleadoSelect.selectedOptions[0].text;
    const tablaBody = document.getElementById('tablaHorarios');

    // Crear una fila por cada día seleccionado
    diasSeleccionados.forEach(dia => {
        const nuevaFila = document.createElement('tr');
        nuevaFila.innerHTML = `
                    <td>${empleadoNombre}</td>
                    <td>${dia}</td>
                    <td>${horaEntrada}</td>
                    <td>${horaSalida}</td>
                    <td>
                        <form method="post" action="#" style="display:inline;">
                            <input type="hidden" name="id" value="${contadorId}" />
                            <button type="submit" class="btn btn-danger btn-sm" onclick="return confirmarEliminacion(event)">
                                <i class="fas fa-trash"></i>
                            </button>
                        </form>
                    </td>
                `;
        tablaBody.appendChild(nuevaFila);
        contadorId++;
    });

    // Limpiar formulario
    document.getElementById('horarioForm').reset();

    // Resetear selectores de hora (solo el texto del botón)
    document.querySelectorAll('.custom-timepicker').forEach(picker => {
        const button = picker.querySelector('.timepicker-input');
        if (picker.dataset.input === 'hora_entrada') {
            button.innerHTML = 'Seleccionar hora entrada <i class="fas fa-clock clock-icon"></i>';
        } else {
            button.innerHTML = 'Seleccionar hora salida <i class="fas fa-clock clock-icon"></i>';
        }
    });
}

// Función para mostrar mensajes
function mostrarMensaje(mensaje, tipo) {
    const color = tipo === 'success' ? '#28a745' : '#dc3545';
    const icono = tipo === 'success' ? 'check-circle' : 'exclamation-triangle';

    const mensajeDiv = document.createElement('div');
    mensajeDiv.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                background: ${color};
                color: white;
                padding: 1rem 1.5rem;
                border-radius: 8px;
                z-index: 10001;
                box-shadow: 0 4px 12px rgba(0,0,0,0.15);
                max-width: 300px;
            `;
    mensajeDiv.innerHTML = `
                <i class="fas fa-${icono}"></i> ${mensaje}
            `;

    document.body.appendChild(mensajeDiv);

    // Remover mensaje después de 3 segundos
    setTimeout(() => {
        mensajeDiv.remove();
    }, 3000);
}

// JQuery para selects personalizados y otros eventos
$(document).ready(function () {
    // Convertir todos los selects a dropdown personalizados con la clase form-control al cargar la página
    $('select.form-select, select.form-control').each(function () {
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
                $(mutation.addedNodes).find('select.form-select, select.form-control').each(function () {
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
    });

    // Inicializar GestionPedidos después de convertir los selects personalizados
    if (document.getElementById('pedidosBody')) {
        window.gestionPedidos = new GestionPedidos();
    }
});