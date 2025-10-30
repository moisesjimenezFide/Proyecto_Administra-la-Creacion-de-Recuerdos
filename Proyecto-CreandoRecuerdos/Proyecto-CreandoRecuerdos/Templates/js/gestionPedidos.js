class GestionPedidos {
    // ARCHIVO JS HECHO CON LA AYUDA DE IA
    constructor() {
        this.filtros = {
            estado: '',
            fechaInicio: '',
            fechaFin: '',
            metodoPago: ''
        };
        this.init();
    }

    init() {
        this.cargarPedidos();
        this.configurarEventos();
        this.configurarEventosDelegados();
    }

    configurarEventos() {
        // Filtros por estado
        document.querySelectorAll('.filter-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                document.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
                btn.classList.add('active');
                this.filtros.estado = btn.dataset.estado;
                this.cargarPedidos();
            });
        });

        // Filtros por fecha y método de pago
        document.getElementById('fechaInicio')?.addEventListener('change', (e) => {
            this.filtros.fechaInicio = e.target.value;
            this.cargarPedidos();
        });

        document.getElementById('fechaFin')?.addEventListener('change', (e) => {
            this.filtros.fechaFin = e.target.value;
            this.cargarPedidos();
        });

        $('#metodoPago').on('change', (e) => {
            this.filtros.metodoPago = e.target.value;
            this.cargarPedidos();
        });

        // Botón refrescar
        document.getElementById('refreshBtn')?.addEventListener('click', () => {
            this.cargarPedidos();
        });
    }

    configurarEventosDelegados() {
        // Delegación de eventos para la tabla dinámica
        document.addEventListener('click', (e) => {
            // Manejar clic en botón de tiempo estimado
            if (e.target.closest('.btn-tiempo')) {
                this.manejarTiempoEstimado(e);
            }

            // Manejar clic en botón de detalle
            if (e.target.closest('.btn-detalle')) {
                this.manejarDetallePedido(e);
            }
        });
    }

    manejarTiempoEstimado(e) {
        const btn = e.target.closest('.btn-tiempo');
        const idPedido = btn.dataset.id;
        const tiempoActual = btn.dataset.tiempo;

        Swal.fire({
            title: 'Modificar Tiempo Estimado',
            html: `
            <div class="mb-3">
                <label for="tiempoEstimado" class="form-label">Nuevo tiempo estimado (minutos)</label>
                <input type="number" id="tiempoEstimado" class="form-control" 
                       value="${tiempoActual}" min="5" max="120" step="5">
            </div>
        `,
            showCancelButton: true,
            confirmButtonText: 'Actualizar',
            cancelButtonText: 'Cancelar',
            focusConfirm: false,
            allowOutsideClick: false,
            preConfirm: () => {
                const nuevoTiempo = document.getElementById('tiempoEstimado').value;
                if (!nuevoTiempo || nuevoTiempo < 5 || nuevoTiempo > 120) {
                    Swal.showValidationMessage('Ingrese un valor entre 5 y 120 minutos');
                    return false;
                }
                return nuevoTiempo;
            }
        }).then((result) => {
            if (result.isConfirmed) {
                this.actualizarTiempoEstimado(idPedido, result.value, btn);
            }
        });
    }

    actualizarTiempoEstimado(idPedido, nuevoTiempo, btn) {
        btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';
        btn.disabled = true;

        fetch('/Pedidos/ActualizarTiempoEstimado', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                idPedido: parseInt(idPedido),
                tiempoEstimado: parseInt(nuevoTiempo)
            })
        })
            .then(response => {
                if (!response.ok) throw new Error('Error en la respuesta del servidor');
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    // Actualizar visualmente en todas las ubicaciones
                    document.querySelectorAll(`[data-id="${idPedido}"] .tiempo-display`).forEach(el => {
                        el.textContent = `${data.nuevoTiempo} min`;
                    });

                    // Actualizar también en el botón
                    btn.dataset.tiempo = nuevoTiempo;

                    // Mostrar notificación
                    Swal.fire({
                        icon: 'success',
                        title: 'Tiempo actualizado',
                        text: data.message,
                        timer: 2000,
                        showConfirmButton: false,
                        position: 'top-end',
                        toast: true
                    });

                    // Forzar recarga de datos después de 1 segundo
                    setTimeout(() => this.cargarPedidos(), 1000);
                } else {
                    throw new Error(data.message || 'Error al actualizar el tiempo');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                Swal.fire({
                    icon: 'success',
                    title: 'Tiempo actualizado',
                    text: data.message,
                    timer: 2000,
                    showConfirmButton: false,
                    position: 'top-end',
                    toast: true
                });
            })
            .finally(() => {
                btn.innerHTML = '<i class="fas fa-clock"></i>';
                btn.disabled = false;
            });
    }

    manejarDetallePedido(e) {
        const idPedido = e.target.closest('.btn-detalle').dataset.id;
        window.location.href = `/Pedidos/DetallePedido?id=${idPedido}`;
    }

    cargarPedidos() {
        const params = new URLSearchParams();
        params.append('estado', this.filtros.estado);
        params.append('fechaInicio', this.filtros.fechaInicio);
        params.append('fechaFin', this.filtros.fechaFin);
        params.append('metodoPago', this.filtros.metodoPago);

        // Mostrar loading
        const tbody = document.getElementById('pedidosBody');
        if (tbody) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="12" class="text-center py-4">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">Cargando...</span>
                        </div>
                        <p class="mt-2">Cargando pedidos...</p>
                    </td>
                </tr>
            `;
        }

        fetch(`/Pedidos/ObtenerPedidos?${params.toString()}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Error en la respuesta del servidor');
                }
                return response.json();
            })
            .then(data => {
                if (data.error) {
                    throw new Error(data.error);
                }
                this.renderizarPedidos(data.pedidos);
            })
            .catch(error => {
                console.error('Error al cargar pedidos:', error);

                if (tbody) {
                    tbody.innerHTML = `
                        <tr>
                            <td colspan="12" class="text-center py-4 text-danger">
                                <i class="fas fa-exclamation-triangle fa-2x mb-2"></i>
                                <p>Error al cargar los pedidos</p>
                                <button class="btn btn-sm btn-primary" onclick="window.location.reload()">
                                    <i class="fas fa-sync-alt me-1"></i> Reintentar
                                </button>
                            </td>
                        </tr>
                    `;
                }

                Swal.fire({
                    icon: 'error',
                    title: 'Error al cargar pedidos',
                    text: error.message,
                    footer: 'Por favor intente nuevamente'
                });
            });
    }

    renderizarPedidos(pedidos) {
        const $tabla = $('#tablaGestionarPedidos');
        const tbody = document.getElementById('pedidosBody');
        if (!tbody) return;

        // Destruye SOLO la instancia de DataTable de esta tabla
        if ($.fn.DataTable.isDataTable($tabla)) {
            $tabla.DataTable().destroy();
            $tabla.closest('.dataTables_wrapper').find('.custom-length-dropdown').remove();
        }

        tbody.innerHTML = pedidos.length === 0
            ? `<tr><td colspan="12" class="text-center py-4"><i class="fas fa-clipboard-list fa-2x mb-2 text-muted"></i><p>No se encontraron pedidos con los filtros seleccionados</p></td></tr>`
            : pedidos.map(pedido => `
    <tr>
        <td>${pedido.numero_pedido}</td>
        <td>${pedido.id_cliente}</td>
        <td>${new Date(pedido.fecha).toLocaleString()}</td>
        <td>${pedido.fecha_fin ? new Date(pedido.fecha_fin).toLocaleString() : 'N/A'}</td>
        <td>${pedido.cantidad_productos} productos</td>
        <td class="text-end">₡${pedido.total.toLocaleString('es-CR')}</td>
        <td>
            <span class="badge ${this.obtenerClaseEstado(pedido.estado)}">
                ${pedido.estado}
            </span>
        </td>
        <td>${pedido.metodo_pago}</td>
        <td>${pedido.para_llevar ? 'Sí' : 'No'}</td>
        <td>
            ${pedido.tiempo_estimado} min
            <button class="btn btn-sm btn-outline-secondary btn-tiempo ms-1" 
                    data-id="${pedido.id_pedido}" 
                    data-tiempo="${pedido.tiempo_estimado}"
                    title="Modificar tiempo estimado">
                <i class="fas fa-clock"></i>
            </button>
        </td>
        <td>
            <div class="d-flex gap-2">
                <button class="btn btn-sm btn-outline-primary btn-detalle" 
                        data-id="${pedido.id_pedido}"
                        title="Ver detalles">
                    <i class="fas fa-eye"></i>
                </button>
            </div>
        </td>
    </tr>
`).join('');

        console.log('Registros tras filtrar:', $('#tablaGestionarPedidos tbody tr').length);

        // Asegura que la clase dataTable esté presente
        $tabla.addClass('dataTable');
        $tabla.removeClass('no-auto-dt');

        window.inicializarDataTables();
    }

    obtenerClaseEstado(estado) {
        const clases = {
            'Pendiente': 'bg-warning text-dark',
            'En preparación': 'bg-info text-white',
            'Listo': 'bg-success text-white',
            'Entregado': 'bg-secondary text-white',
            'Cancelado': 'bg-danger text-white'
        };
        return clases[estado] || 'bg-light text-dark';
    }
}
