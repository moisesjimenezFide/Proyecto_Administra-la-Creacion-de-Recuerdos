document.addEventListener('DOMContentLoaded', function () {
    // Objeto menuData 
    const menuData = {
        desayunos: [
            {
                id: 1,
                name: "Típico #1",
                description: "Gallo pinto, 2 huevos, salchichón, natilla, 2 tortillas, bebida 8 oz (americano, latte o jugo de naranja)",
                price: 3995,
                image: "assets/menu/desayuno1.jpg"
            },
            {
                id: 2,
                name: "Típico #2",
                description: "Gallo pinto, 2 huevos, salchichón, natilla, 2 tortillas, bebida 8 oz (café negro o café con leche)",
                price: 3300,
                image: "assets/menu/desayuno2.jpg"
            },
            {
                id: 3,
                name: "Panino J&Q",
                description: "Panino artesanal con jamón, queso amarillo, lechuga, tomate, bebida 8 oz (americano, latte o jugo de naranja)",
                price: 4850,
                image: "assets/menu/panino-jq.jpg"
            }
        ],
        panini: [
            {
                id: 4,
                name: "Caprese",
                description: "Panino artesanal relleno con queso mozzarella, tomate, albahaca y aceite de albahaca",
                price: 5500,
                image: "assets/menu/panini-caprese.jpg"
            },
            {
                id: 5,
                name: "Panini J&Q",
                description: "Panino artesanal con jamón y queso amarillo, lechuga, tomate y salsa especial",
                price: 5500,
                image: "assets/menu/panini-jq.jpg"
            },
            {
                id: 6,
                name: "Roast beef",
                description: "Panino artesanal relleno con roast beef, queso derretido, lechuga, tomate y salsa especial",
                price: 5500,
                image: "assets/menu/panini-roastbeef.jpg"
            },
            {
                id: 7,
                name: "Pollo",
                description: "Panino artesanal relleno con pollo mechado, queso derretido, lechuga, tomate y salsa especial",
                price: 5500,
                image: "assets/menu/panini-pollo.jpg"
            }
        ],
        crepes: [
            {
                id: 8,
                name: "Pollo salsa blanca",
                description: "Crêpe salada artesanal rellena con pollo en salsa blanca y cubierta con queso",
                price: 4250,
                image: "assets/menu/crepe-pollo.jpg"
            },
            {
                id: 9,
                name: "Caprese",
                description: "Crêpe artesanal de albahaca rellena con queso mozzarella, tomate y albahaca",
                price: 4250,
                image: "assets/menu/crepe-caprese.jpg"
            },
            {
                id: 10,
                name: "Nutella",
                description: "Crêpe dulce artesanal rellena con Nutella y fruta, bañada con topping y decorada con chantilly",
                price: 2750,
                image: "assets/menu/crepe-nutella.jpg"
            },
            {
                id: 11,
                name: "Frita",
                description: "Crêpe dulce artesanal rellena con Nutella y banano, frita y bañada con topping",
                price: 2750,
                image: "assets/menu/crepe-frita.jpg"
            }
        ],
        pasta: [
            {
                id: 12,
                name: "Tornillos",
                description: "Pasta tipo tornillos con opción del día, acompañada de ensalada",
                price: 3950,
                image: "assets/menu/pasta-tornillos.jpg"
            },
            {
                id: 13,
                name: "Plumas",
                description: "Pasta tipo plumas con opción del día, acompañada de ensalada",
                price: 3950,
                image: "assets/menu/pasta-plumas.jpg"
            },
            {
                id: 14,
                name: "Spaghetti",
                description: "Spaghetti con opción del día, acompañada de ensalada",
                price: 3950,
                image: "assets/menu/pasta-spaghetti.jpg"
            }
        ],
        arroz: [
            {
                id: 15,
                name: "Con carne",
                description: "Arroz acompañado de frijoles molidos y carne",
                price: 3950,
                image: "assets/menu/arroz-carne.jpg"
            },
            {
                id: 16,
                name: "Con pollo",
                description: "Arroz acompañado de frijoles molidos y pollo",
                price: 3950,
                image: "assets/menu/arroz-pollo.jpg"
            },
            {
                id: 17,
                name: "Arreglado",
                description: "Arroz acompañado de frijoles molidos y variedad de carnes",
                price: 3950,
                image: "assets/menu/arroz-arreglado.jpg"
            }
        ],
        tentaciones: [
            {
                id: 18,
                name: "Canastas de Patacones",
                description: "3 canastas de patacones rellenas con frijoles molidos, pollo mechado y cubiertas con queso",
                price: 3950,
                image: "assets/menu/patacones.jpg"
            },
            {
                id: 19,
                name: "Enyucados",
                description: "1 enyucado de queso + 1 enyucado de carne, acompañados de ensalada verde y salsa de la casa",
                price: 3950,
                image: "assets/menu/enyucados.jpg"
            },
            {
                id: 20,
                name: "Papas a la francesa",
                description: "Papas a la francesa con salsa rosada",
                price: 1750,
                image: "assets/menu/papas-fritas.jpg"
            }
        ],
        postres: [
            {
                id: 21,
                name: "Postre del día",
                description: "Consulta por nuestra variedad de postres disponibles",
                price: 0,
                image: "assets/menu/postre.jpg"
            },
            {
                id: 22,
                name: "Repostería",
                description: "Selección de repostería artesanal",
                price: 0,
                image: "assets/menu/reposteria.jpg"
            },
            {
                id: 23,
                name: "Gluten Free",
                description: "Postres libres de gluten",
                price: 0,
                image: "assets/menu/glutenfree.jpg"
            }
        ],
        "bebidas-calientes": [
            {
                id: 24,
                name: "Americano",
                description: "Café americano - Grande ₡2.100 / Pequeño ₡1.100",
                price: 2100,
                image: "assets/menu/americano.jpg"
            },
            {
                id: 25,
                name: "Expresso",
                description: "Café expresso - Doble ₡2.000 / Simple ₡1.000",
                price: 2000,
                image: "assets/menu/expresso.jpg"
            },
            {
                id: 26,
                name: "Capuccino",
                description: "Capuccino - Grande ₡3.200 / Pequeño ₡1.800",
                price: 3200,
                image: "assets/menu/capuccino.jpg"
            },
            {
                id: 27,
                name: "Latte",
                description: "Latte - Grande ₡2.700 / Pequeño ₡1.600",
                price: 2700,
                image: "assets/menu/latte.jpg"
            },
            {
                id: 28,
                name: "Capuccino con Sabor",
                description: "Capuccino con Sabor - Grande ₡3.600 / Pequeño ₡2.000",
                price: 3600,
                image: "assets/menu/capuccinosabor.jpg"
            },
            {
                id: 29,
                name: "Café Negro",
                description: "Café Negro - Grande ₡1.000 / Pequeño ₡600",
                price: 1000,
                image: "assets/menu/cafenegro.jpg"
            },
            {
                id: 30,
                name: "Café con Leche",
                description: "Café con Leche - Grande ₡1.500 / Pequeño ₡900",
                price: 1500,
                image: "assets/menu/cafeleche.jpg"
            },
            {
                id: 31,
                name: "Chocolate",
                description: "Chocolate - Grande ₡1.700 / Pequeño ₡1.100",
                price: 1700,
                image: "assets/menu/chocolate.jpg"
            },
            {
                id: 32,
                name: "Té",
                description: "Té - Grande ₡1.700 / Pequeño ₡1.100",
                price: 1700,
                image: "assets/menu/te.jpg"
            },
            {
                id: 33,
                name: "Moka",
                description: "Té - Grande ₡3.600 / Pequeño ₡2.000",
                price: 3600,
                image: "assets/menu/moka.jpg"
            }
        ],
        "bebidas-frias": [
            {
                id: 34,
                name: "Agua",
                description: "Agua (botella 600ml)",
                price: 1100,
                image: "assets/menu/agua.jpg"
            },
            {
                id: 35,
                name: "Gaseosa",
                description: "Gaseosa  (lata 237 ml)",
                price: 775,
                image: "assets/menu/gaseosa.jpg"
            },
            {
                id: 36,
                name: "Bebida refrescante",
                description: "Bebida refrescante de frutas (papaya, piña, sandía)",
                price: 2000,
                image: "assets/menu/refresco-fruta.jpg"
            },
            {
                id: 37,
                name: "Soda italiana",
                description: "Soda italiana",
                price: 2200,
                image: "assets/menu/sodaitaliana.jpg"
            },
            {
                id: 38,
                name: "Latte frío",
                description: "Latte frío",
                price: 3400,
                image: "assets/menu/lattefrio.jpg"
            },
            {
                id: 39,
                name: "Frappé",
                description: "Frappé de café o chocolate",
                price: 3400,
                image: "assets/menu/frappe.jpg"
            },
            {
                id: 40,
                name: "Jugo de naranja",
                description: "Jugo de naranja natural",
                price: 1000,
                image: "assets/menu/jugo-naranja.jpg"
            },
            {
                id: 41,
                name: "Milkshake",
                description: "Milkshake de varios sabores",
                price: 3400,
                image: "assets/menu/milkshake.jpg"
            },
            {
                id: 42,
                name: "Tropical",
                description: "Tropical",
                price: 550,
                image: "assets/menu/tropical.jpg"
            }
        ],
        especiales: [
            {
                id: 43,
                name: "Pedidos especiales",
                description: "Pedido a gusto del cliente - Consultar precio",
                price: 0,
                image: "assets/menu/especiales.jpg"
            }
        ]
    };



    // Elementos del DOM
const productGrid = document.getElementById('productGrid');
const categoryButtons = document.querySelectorAll('[data-category]');

// Cargar categoría inicial
loadCategory('todos');

// Event listeners para los botones de categoría
categoryButtons.forEach(button => {
    button.addEventListener('click', function () {
        categoryButtons.forEach(btn => btn.classList.remove('active'));
        this.classList.add('active');
        const category = this.getAttribute('data-category');
        loadCategory(category);
    });
});

// Función para cargar productos por categoría
function loadCategory(category) {
    productGrid.innerHTML = '';

    let products = [];

    if (category === 'todos') {
        Object.values(menuData).forEach(catProducts => {
            products = products.concat(catProducts);
        });
    } else {
        products = menuData[category] || [];
    }

    if (products.length === 0) {
        productGrid.innerHTML = '<p class="text-center">No hay productos en esta categoría</p>';
        return;
    }

    products.forEach(product => {
        const productCard = document.createElement('div');
        productCard.className = 'col-md-4 mb-4';

        productCard.innerHTML = `
            <div class="card h-100 border-0 shadow-sm product-card">
                <div class="card-img-container">
                    <img src="${product.image}" class="card-img-top" alt="${product.name}">
                </div>
                <div class="card-body">
                    <h5 class="card-title" style="font-family: 'Sono', sans-serif;">${product.name}</h5>
                    <p class="card-text">${product.description}</p>
                    <div class="d-flex justify-content-between align-items-center">
                        <span class="fw-bold dark-violet">₡${product.price.toLocaleString()}</span>
                    </div>
                    <div class="mt-3 d-flex flex-nowrap justify-content-end gap-2">
                        <button class="btn btn-danger btn-sm text-white delete-btn" data-id="${product.id}">
                            <i class="fas fa-trash-alt me-1"></i>Eliminar
                        </button>
                        <button class="btn btn-sm text-white update-btn" data-id="${product.id}" style="background-color: #E27AB0;">
                            <i class="fas fa-edit me-1"></i>Actualizar
                        </button>
                    </div>
                </div>
            </div>
        `;

        productGrid.appendChild(productCard);
    });

    document.querySelectorAll('.delete-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            alert(`Se eliminó el producto`);
        });
    });

    document.querySelectorAll('.update-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            alert(`Se actualizó el producto`);
        });
    });
}

});