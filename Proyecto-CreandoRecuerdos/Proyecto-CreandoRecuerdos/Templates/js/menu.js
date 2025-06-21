document.addEventListener('DOMContentLoaded', function () {
    // Mapeo de nombres de categoría a IDs (ajusta según tu base de datos)
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
        'especiales': 10
    };

    // Seleccionar solo los botones de filtro (no los productos)
    const filterButtons = document.querySelectorAll('.d-flex.flex-wrap.justify-content-center.gap-3.mt-4 [data-category]');

    filterButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            // Prevenir comportamiento por defecto
            e.preventDefault();
            e.stopPropagation();

            // Remover clase 'active' de todos los botones
            filterButtons.forEach(btn => btn.classList.remove('active'));

            // Agregar clase 'active' al botón clickeado
            this.classList.add('active');

            // Obtener la categoría seleccionada
            const categoryName = this.dataset.category;
            const categoryId = categoryMap[categoryName];

            // Filtrar productos
            filterProducts(categoryId);
        });
    });

    // Función para filtrar los productos
    function filterProducts(categoryId) {
        const allProducts = document.querySelectorAll('#product-container > .col-md-4');

        allProducts.forEach(product => {
            if (categoryId === 'todos' || product.dataset.category == categoryId) {
                product.style.display = 'block';
                product.classList.add('animate__animated', 'animate__fadeIn');
            } else {
                product.style.display = 'none';
                product.classList.remove('animate__animated', 'animate__fadeIn');
            }
        });
    }
});