document.addEventListener('DOMContentLoaded', function () {
    const categoryMap = {
        'todos': 'todos',
        'desayunos': 1,
        'panini': 2,
        'crepes': 3,
        'pasta': 4,
        'arroz': 5,
        'tentaciones': 6,
        'postres': 7,
        'bebidas-calientes': 8,
        'bebidas-frias': 9,
        'especiales': 10,
        'recomendaciones': 'recomendaciones'
    };

    const filterButtons = document.querySelectorAll('.d-flex.flex-wrap.justify-content-center.gap-3.mt-4 [data-category]');
    filterButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();

            filterButtons.forEach(btn => btn.classList.remove('active'));
            this.classList.add('active');

            const categoryName = this.dataset.category;
            const categoryId = categoryMap[categoryName];

            filterProducts(categoryId);
        });
    });

    // Mostrar todos los productos inicialmente
    cargarProductos();

    function cargarProductos() {
        fetch('/Menu/ObtenerTodosLosProductos')
            .then(response => response.json())
            .then(productos => {
                const contenedor = document.getElementById('product-container');
                contenedor.innerHTML = '';

                productos.forEach(p => {
                    const div = document.createElement('div');
                    div.className = 'col-md-4 mb-4';
                    div.dataset.category = p.id_categoria;
                    div.dataset.recomendado = p.recomendado;

                    div.innerHTML = `
                        <div class="card h-100">
                            <div class="card-body">
                                <h5 class="card-title">${p.nombre}</h5>
                                <p class="card-text">${p.descripcion}</p>
                                <p class="fw-bold">₡${p.precio_por_unit}</p>
                                <button class="btn btn-sm btn-outline-dark">Editar</button>
                                <button class="btn btn-sm btn-danger">Eliminar</button>
                            </div>
                        </div>
                    `;
                    contenedor.appendChild(div);
                });
            });
    }

    function filterProducts(categoryId) {
        const allProducts = document.querySelectorAll('#product-container > .col-md-4');

        allProducts.forEach(product => {
            const productCategory = product.dataset.category;
            const recomendado = product.dataset.recomendado === "true";

            if (categoryId === 'todos') {
                product.style.display = 'block';
            } else if (categoryId === 'recomendaciones') {
                product.style.display = recomendado ? 'block' : 'none';
            } else {
                product.style.display = productCategory === String(categoryId) ? 'block' : 'none';
            }
        });
    }



    // Crear recomendación
    const crearRecomendacionBtn = document.getElementById('crear_recomendacion');
    if (crearRecomendacionBtn) {
        crearRecomendacionBtn.addEventListener('click', function (e) {
            e.preventDefault();
            abrirFormularioRecomendacion();
        });
    }

    function abrirFormularioRecomendacion() {
        $.ajax({
            url: '/Menu/ObtenerTodosLosProductos',
            type: 'GET',
            success: function (productos) {
                let options = '<option value="">Seleccione un producto</option>';
                productos.forEach(p => {
                    options += `<option value="${p.id_producto}">${p.nombre}</option>`;
                });

                Swal.fire({
                    title: 'Crear Recomendación',
                    html:
                        `<label for="swal-producto">Producto:</label>` +
                        `<select id="swal-producto" class="swal2-select">${options}</select>` +
                        `<label for="swal-motivo" class="mt-2">Motivo:</label>` +
                        `<textarea id="swal-motivo" class="swal2-textarea" placeholder="Ingrese el motivo de la recomendación"></textarea>`,
                    showCancelButton: true,
                    confirmButtonText: 'Guardar',
                    preConfirm: () => {
                        const productoId = document.getElementById('swal-producto').value;
                        const motivo = document.getElementById('swal-motivo').value.trim();

                        if (!productoId || !motivo) {
                            Swal.showValidationMessage('Debe seleccionar un producto y escribir un motivo.');
                            return false;
                        }

                        return { productoId, motivo };
                    }
                }).then((result) => {
                    if (result.isConfirmed) {
                        $.ajax({
                            url: '/Menu/GuardarRecomendacion',
                            type: 'POST',
                            data: result.value,
                            success: function () {
                                Swal.fire('¡Guardado!', 'La recomendación fue registrada.', 'success')
                                    .then(() => location.reload());
                            },
                            error: function () {
                                Swal.fire('Error', 'No se pudo guardar la recomendación.', 'error');
                            }
                        });
                    }
                });
            },
            error: function () {
                Swal.fire('Error', 'No se pudo cargar la lista de productos.', 'error');
            }
        });
    }
});
