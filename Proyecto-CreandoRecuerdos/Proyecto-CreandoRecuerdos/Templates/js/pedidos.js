class SistemaPedidos {
    // ARCHIVO JS HECHO CON LA AYUDA DE IA
    constructor() {
        this.pedidoActual = [];
        this.productosCargados = false;
        this.init();
    }

    init() {
        this.setupGlobalEventListeners(); // Configura todos los eventos delegados
        this.cargarProductos();
    }

    setupGlobalEventListeners() {
        // Evento delegado para toda la página
        document.addEventListener('click', (e) => {
            // Manejar categorías
            if (e.target.closest('#categoryList button[data-category]')) {
                const btn = e.target.closest('button[data-category]');
                const categoriaId = btn.dataset.category;
                this.filtrarProductos(categoriaId);

                // Actualizar botón activo
                document.querySelectorAll('#categoryList button').forEach(b => {
                    b.classList.toggle('active', b === btn);
                });
            }

            // Manejar agregar al carrito
            if (e.target.closest('.add-to-cart')) {
                const btn = e.target.closest('.add-to-cart');
                this.agregarAlCarrito(btn);
            }

            // Manejar botones de cantidad en el carrito
            if (e.target.closest('.aumentar-cantidad')) {
                const itemIndex = e.target.closest('.order-item').dataset.index;
                this.aumentarCantidad(itemIndex);
            }

            if (e.target.closest('.disminuir-cantidad')) {
                const itemIndex = e.target.closest('.order-item').dataset.index;
                this.disminuirCantidad(itemIndex);
            }

            // Manejar eliminar ítem
            if (e.target.closest('.eliminar-item')) {
                const itemIndex = e.target.closest('.order-item').dataset.index;
                this.eliminarItem(itemIndex);
            }

            // Manejar personalización
            if (e.target.closest('.agregar-personalizacion')) {
                const itemIndex = e.target.closest('.order-item').dataset.index;
                this.agregarPersonalizacion(itemIndex);
            }

            // Manejar procesar pago
            if (e.target.closest('#checkoutBtn')) {
                this.procesarPago();
            }

            // Manejar limpiar pedido
            if (e.target.closest('#clearOrderBtn')) {
                this.pedidoActual = [];
                this.actualizarPedido();
            }
        });
    }

    aumentarCantidad(index) {
        if (this.pedidoActual[index]) {
            this.pedidoActual[index].cantidad += 1;
            this.actualizarPedido();
        }
    }

    disminuirCantidad(index) {
        if (this.pedidoActual[index] && this.pedidoActual[index].cantidad > 1) {
            this.pedidoActual[index].cantidad -= 1;
            this.actualizarPedido();
        }
    }

    eliminarItem(index) {
        if (this.pedidoActual[index]) {
            this.pedidoActual.splice(index, 1);
            this.actualizarPedido();
        }
    }

    agregarPersonalizacion(index) {
        Swal.fire({
            title: 'Personalización',
            input: 'text',
            inputLabel: 'Indicaciones especiales (ej: sin cebolla)',
            inputPlaceholder: 'Escribe aquí las personalizaciones...',
            showCancelButton: true,
            inputValidator: (value) => {
                if (value && value.length > 500) {
                    return 'Las personalizaciones no pueden exceder 500 caracteres';
                }
            }
        }).then((result) => {
            if (result.isConfirmed && this.pedidoActual[index]) {

                const productoId = this.pedidoActual[index].id;
                this.pedidoActual = this.pedidoActual.map(item => {
                    if (item.id === productoId) {
                        return { ...item, personalizacion: result.value || '' };
                    }
                    return item;
                });

                this.actualizarPedido();
            }
        });
    }

    cargarProductos() {
        fetch('/Pedidos/ObtenerProductos')
            .then(response => response.json())
            .then(data => {
                if (data && data.length > 0) {
                    window.productosGlobales = data;
                    this.mostrarProductos(data);
                    this.productosCargados = true;
                } else {
                    throw new Error('No se recibieron productos del servidor');
                }
            })
            .catch(error => {
                console.error('Error al cargar productos:', error);
                this.mostrarAlerta('Error al cargar el menú. Por favor recarga la página.', 'danger');
            });
    }

    procesarPago() {
        if (this.pedidoActual.length === 0) {
            this.mostrarAlerta('No hay productos en el pedido', 'warning');
            return;
        }

        const nombreCliente = document.getElementById('customerName').value;
        if (!nombreCliente) {
            this.mostrarAlerta('Por favor ingrese el nombre del cliente', 'warning');
            return;
        }

        const telefono = document.getElementById('customerPhone').value;
        if (telefono && !/^\d{8}$/.test(telefono)) {
            this.mostrarAlerta('El teléfono debe tener 8 dígitos', 'warning');
            return;
        }

        // Calcular totales
        const subtotal = this.pedidoActual.reduce((sum, item) => sum + (item.precio * item.cantidad), 0);
        const impuestos = subtotal * 0.13;
        const total = subtotal + impuestos;

        // Crear objeto del pedido - Asegurando que todos los campos estén presentes
        const pedidoData = {
            NombreCliente: nombreCliente,
            Telefono: telefono || '',
            ParaLlevar: document.getElementById('takeawayOrder').checked ? true : false, // Asegurar valor booleano
            Productos: this.pedidoActual.map(item => ({
                IdProducto: item.id,
                Nombre: item.nombre,
                Cantidad: item.cantidad,
                PrecioUnitario: item.precio,
                Personalizacion: item.personalizacion || '' // Asegurar que siempre haya un valor
            })),
            Subtotal: subtotal,
            Impuestos: impuestos,
            Total: total
        };

        // Mostrar loading
        const btn = document.getElementById('checkoutBtn');
        const originalText = btn.innerHTML;
        btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Procesando...';
        btn.disabled = true;

        // Enviar datos al servidor
        fetch('/Pedidos/ConfirmarPedido', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(pedidoData)
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    window.location.href = data.redirectUrl;
                } else {
                    throw new Error(data.message || 'Error al procesar el pedido');
                }
            })
            .catch(error => {
                this.mostrarAlerta(error.message, 'danger');
            })
            .finally(() => {
                btn.innerHTML = originalText;
                btn.disabled = false;
            });
    }

    filtrarProductos(categoriaId) {
        if (categoriaId === '0') {
            this.mostrarProductos(window.productosGlobales);
            return;
        }

        if (categoriaId === 'recomendados') {
            fetch('/Pedidos/ObtenerProductosRecomendados')
                .then(response => response.json())
                .then(data => this.mostrarProductos(data))
                .catch(error => {
                    console.error('Error:', error);
                    this.mostrarAlerta('Error al cargar productos recomendados', 'danger');
                });
            return;
        }

        fetch(`/Pedidos/ObtenerProductosPorCategoria?idCategoria=${categoriaId}`)
            .then(response => response.json())
            .then(data => this.mostrarProductos(data))
            .catch(error => {
                console.error('Error:', error);
                this.mostrarAlerta('Error al cargar la categoría seleccionada', 'danger');
            });
    }

    mostrarProductos(productos) {
        const grid = document.getElementById('productGrid');
        if (!grid) return;

        grid.innerHTML = productos.map(producto => {
            const precio = producto.precio_por_unidad || 0;
            const precioValido = precio > 0 && precio >= 1;

            return `
        <div class="col-md-4 mb-3 product-item" data-id="${producto.id_producto}">
            <div class="card h-100">
                <img src="/Templates/img/menu/${producto.img_url || 'default.png'}" 
                     class="card-img-top" 
                     alt="${producto.nombre}"
                     onerror="this.src='/Templates/img/menu/default.png'">
                <div class="card-body">
                    <h5 class="card-title">${producto.nombre}</h5>
                    <p class="card-text">${producto.descripcion || 'Sin descripción'}</p>
                    <p class="text-muted">₡${precio.toLocaleString('es-CR') || '0'}</p>
                    ${precioValido ?
                    `<button class="btn btn-sm btn-primary add-to-cart"
                            data-id="${producto.id_producto}"
                            data-name="${producto.nombre}"
                            data-price="${precio}">
                            <i class="fas fa-plus"></i> Agregar
                        </button>` :
                    `<div class="alert alert-warning small mb-0">
                            <i class="fas fa-info-circle"></i> Por favor leer la descripción del producto.
                        </div>`
                }
                </div>
            </div>
        </div>
        `;
        }).join('');
    }

    agregarAlCarrito(btn) {

        if (parseFloat(btn.dataset.price) <= 0) {
            this.mostrarAlerta('Consultar precio de este producto en el lugar o llamándonos al número 8888-8888', 'warning');
            return;
        }

        const producto = {
            id: parseInt(btn.dataset.id),
            nombre: btn.dataset.name,
            precio: parseFloat(btn.dataset.price),
            cantidad: 1,
            personalizacion: '',
            img_url: btn.closest('.product-item').querySelector('img').src
        };

        const itemExistente = this.pedidoActual.find(item => item.id === producto.id && item.personalizacion === producto.personalizacion);

        if (itemExistente) {
            itemExistente.cantidad += 1;
        } else {
            this.pedidoActual.push(producto);
        }

        this.actualizarPedido();
        this.mostrarAlerta(`${producto.nombre} agregado al pedido`, 'success');
    }

    actualizarPedido() {
        const contenedor = document.getElementById('orderItems');
        if (!contenedor) return;

        if (this.pedidoActual.length === 0) {
            contenedor.innerHTML = `
            <div class="empty-order">
                <i class="fas fa-shopping-cart"></i>
                <p>No hay productos en el pedido</p>
            </div>`;
            document.getElementById('checkoutBtn').disabled = true;
            return;
        }

        const subtotal = this.pedidoActual.reduce((sum, item) => sum + (item.precio * item.cantidad), 0);
        const impuestos = subtotal * 0.13;
        const total = subtotal + impuestos;

        contenedor.innerHTML = this.pedidoActual.map((item, index) => `
        <div class="order-item" data-index="${index}">
            <div class="item-name">${item.nombre}</div>
            <div class="item-quantity">
                <button class="btn btn-sm btn-outline-secondary disminuir-cantidad">
                    <i class="fas fa-minus"></i>
                </button>
                <span class="quantity-display">${item.cantidad}</span>
                <button class="btn btn-sm btn-outline-secondary aumentar-cantidad">
                    <i class="fas fa-plus"></i>
                </button>
            </div>
            <div class="item-price">₡${(item.precio * item.cantidad).toLocaleString('es-CR')}</div>
            <div class="item-actions">
                <button class="btn btn-sm btn-outline-primary agregar-personalizacion">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger eliminar-item">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        </div>
    `).join('');

        document.getElementById('orderSubtotal').textContent = `₡${subtotal.toLocaleString('es-CR')}`;
        document.getElementById('orderTax').textContent = `₡${impuestos.toLocaleString('es-CR')}`;
        document.getElementById('orderTotal').textContent = `₡${total.toLocaleString('es-CR')}`;
        document.getElementById('checkoutBtn').disabled = false;
    }

    mostrarAlerta(mensaje, tipo = 'info', duracion = 3000) {
        const iconos = {
            'success': 'success',
            'danger': 'error',
            'warning': 'warning',
            'info': 'info'
        };

        Swal.fire({
            title: mensaje,
            icon: iconos[tipo] || 'info',
            timer: duracion,
            timerProgressBar: true,
            showConfirmButton: false,
            position: 'top-end',
            toast: true
        });
    }

    obtenerIconoAlerta(tipo) {
        const iconos = {
            'success': 'fa-check-circle',
            'danger': 'fa-exclamation-circle',
            'warning': 'fa-exclamation-triangle',
            'info': 'fa-info-circle'
        };
        return iconos[tipo] || 'fa-info-circle';
    }
}

// Inicialización
document.addEventListener('DOMContentLoaded', () => {
    if (document.getElementById('productGrid')) {
        window.sistemaPedidos = new SistemaPedidos();
    }
});