document.addEventListener('DOMContentLoaded', function () {
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

    let currentOrder = [];
    let allOrders = [];
    let currentOrdersPage = 1;
    const ordersPerPage = 10;
    let currentRating = 0;

    const mockOrders = [
        {
            orderNumber: 'ORD-2023-0001',
            customerName: 'Juan Pérez',
            customerPhone: '8888-8888',
            items: [
                { name: 'Típico #1', quantity: 2, price: 3995 },
                { name: 'Café Americano', quantity: 1, price: 2100 }
            ],
            total: 10090,
            status: 'pending',
            createdAt: '2023-05-01T10:30:00',
            paymentMethod: 'efectivo',
            takeaway: false
        },
        {
            orderNumber: 'ORD-2023-0002',
            customerName: 'María Rodríguez',
            customerPhone: '7777-7777',
            items: [
                { name: 'Panini Caprese', quantity: 1, price: 5500 },
                { name: 'Jugo de naranja', quantity: 2, price: 1000 }
            ],
            total: 7500,
            status: 'preparation',
            createdAt: '2023-05-01T11:15:00',
            paymentMethod: 'tarjeta',
            takeaway: true
        },
        {
            orderNumber: 'ORD-2023-0003',
            customerName: 'Carlos Sánchez',
            customerPhone: '6666-6666',
            items: [
                { name: 'Crêpe Nutella', quantity: 1, price: 2750 },
                { name: 'Latte', quantity: 1, price: 2700 }
            ],
            total: 5450,
            status: 'ready',
            createdAt: '2023-05-01T11:45:00',
            paymentMethod: 'sinpe',
            takeaway: false
        },
        {
            orderNumber: 'ORD-2023-0004',
            customerName: 'Ana Martínez',
            customerPhone: '5555-5555',
            items: [
                { name: 'Pasta Tornillos', quantity: 1, price: 3950 },
                { name: 'Agua', quantity: 1, price: 1100 }
            ],
            total: 5050,
            status: 'delivered',
            createdAt: '2023-05-01T12:00:00',
            paymentMethod: 'efectivo',
            takeaway: true
        }
    ];

    function toggleOrderPanel() {
        const orderPanel = document.querySelector('.pos-order-panel');
        const toggleBtn = document.getElementById('toggleOrderPanelBtn');

        orderPanel.classList.toggle('collapsed');
        toggleBtn.classList.toggle('collapsed');

        if (orderPanel.classList.contains('collapsed')) {
            toggleBtn.innerHTML = '<i class="fas fa-chevron-left"></i>';
            toggleBtn.style.right = '0';
        } else {
            toggleBtn.innerHTML = '<i class="fas fa-chevron-right"></i>';
            toggleBtn.style.right = '20px';
        }
    }

    function initPOS() {
        loadCategory('desayunos');
        
        document.querySelectorAll('[data-category]').forEach(button => {
            button.addEventListener('click', function() {
                const category = this.getAttribute('data-category');
                loadCategory(category);
                
                document.querySelectorAll('[data-category]').forEach(btn => {
                    btn.classList.remove('active');
                });
                this.classList.add('active');
            });
        });
        
        document.getElementById('clearOrderBtn').addEventListener('click', clearOrder);
        document.getElementById('checkoutBtn').addEventListener('click', openPaymentModal);
        document.getElementById('saveOrderBtn').addEventListener('click', saveOrder);
        
        document.querySelectorAll('.payment-method').forEach(method => {
            method.addEventListener('click', function() {
                const methodName = this.getAttribute('data-method');
                selectPaymentMethod(methodName);
            });
        });
        
        document.getElementById('cashAmount').addEventListener('input', calculateChange);
        document.getElementById('confirmPaymentBtn').addEventListener('click', confirmPayment);
        document.getElementById('newOrderBtn').addEventListener('click', startNewOrder);
        document.getElementById('printReceiptBtn').addEventListener('click', printReceipt);
        document.getElementById('toggleOrderPanelBtn').addEventListener('click', toggleOrderPanel);
        
        document.addEventListener('click', function(e) {
            if (e.target && (e.target.id === 'manageOrdersBtn' || e.target.closest('#manageOrdersBtn'))) {
                e.preventDefault();
                loadOrders();
                const modal = new bootstrap.Modal(document.getElementById('orderStatusModal'));
                modal.show();
            }
        })
        
        document.querySelectorAll('.filter-btn').forEach(btn => {
            btn.addEventListener('click', function() {
                document.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
                this.classList.add('active');
                const status = this.getAttribute('data-status');
                const searchTerm = document.getElementById('orderSearch').value;
                loadOrders(status, searchTerm);
            });
        });
        
        document.getElementById('searchOrderBtn').addEventListener('click', function() {
            const searchTerm = document.getElementById('orderSearch').value;
            const activeFilter = document.querySelector('.filter-btn.active');
            const status = activeFilter ? activeFilter.getAttribute('data-status') : 'all';
            loadOrders(status, searchTerm);
        });
        
        document.getElementById('orderSearch').addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                const searchTerm = this.value;
                const activeFilter = document.querySelector('.filter-btn.active');
                const status = activeFilter ? activeFilter.getAttribute('data-status') : 'all';
                loadOrders(status, searchTerm);
            }
        });
        
        document.querySelectorAll('.rating-stars i').forEach(star => {
            star.addEventListener('click', function() {
                const rating = parseInt(this.getAttribute('data-rating'));
                setRating(rating);
            });
            
            star.addEventListener('mouseover', function() {
                const rating = parseInt(this.getAttribute('data-rating'));
                highlightStars(rating);
            });
            
            star.addEventListener('mouseout', function() {
                highlightStars(currentRating);
            });
        });
        
        document.getElementById('ratingComments').addEventListener('input', function() {
            const remaining = 500 - this.value.length;
            document.getElementById('charsLeft').textContent = remaining;
        });
        
        document.getElementById('submitRatingBtn').addEventListener('click', submitRating);
        
        updateCurrentTime();
        setInterval(updateCurrentTime, 1000);
    }

    function loadOrders(statusFilter = 'all', searchTerm = '') {
        const filteredOrders = mockOrders.filter(order => {
            const matchesStatus = statusFilter === 'all' || order.status === statusFilter;
            const matchesSearch = searchTerm === '' || 
                order.orderNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
                order.customerName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                order.customerPhone.includes(searchTerm);
            return matchesStatus && matchesSearch;
        });

        allOrders = filteredOrders;
        currentOrdersPage = 1;
        renderOrdersTable();
    }

    function renderOrdersTable() {
        const tbody = document.getElementById('ordersTableBody');
        tbody.innerHTML = '';
        
        const startIndex = (currentOrdersPage - 1) * ordersPerPage;
        const endIndex = Math.min(startIndex + ordersPerPage, allOrders.length);
        const ordersToShow = allOrders.slice(startIndex, endIndex);
        
        ordersToShow.forEach(order => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${order.orderNumber}</td>
                <td>${order.customerName}</td>
                <td>${order.customerPhone}</td>
                <td>
                    <ul class="order-products-list">
                        ${order.items.map(item => `<li>${item.quantity}x ${item.name}</li>`).join('')}
                    </ul>
                </td>
                <td>₡${order.total.toLocaleString()}</td>
                <td>
                    <span class="order-status status-${order.status}">
                        ${getStatusText(order.status)}
                    </span>
                </td>
                <td>
                    ${order.status !== 'canceled' && order.status !== 'delivered' ? `
                    <select class="form-select form-select-sm change-status" data-order="${order.orderNumber}">
                        <option value="" disabled selected>Cambiar estado</option>
                        <option value="pending" ${order.status === 'pending' ? 'disabled' : ''}>Pendiente</option>
                        <option value="preparation" ${order.status === 'preparation' ? 'disabled' : ''}>En preparación</option>
                        <option value="ready" ${order.status === 'ready' ? 'disabled' : ''}>Listo</option>
                        <option value="delivered" ${order.status === 'delivered' ? 'disabled' : ''}>Entregado</option>
                        <option value="canceled">Cancelar</option>
                    </select>
                    ` : ''}
                </td>
            `;
            tbody.appendChild(tr);
        });
        
        document.querySelectorAll('.change-status').forEach(select => {
            select.addEventListener('change', function() {
                const orderNumber = this.getAttribute('data-order');
                const newStatus = this.value;
                updateOrderStatus(orderNumber, newStatus);
            });
        });
        
        renderOrdersPagination();
    }

    function renderOrdersPagination() {
        const pagination = document.getElementById('ordersPagination');
        pagination.innerHTML = '';
        
        const totalPages = Math.ceil(allOrders.length / ordersPerPage);
        
        if (totalPages <= 1) return;
        
        const prevLi = document.createElement('li');
        prevLi.className = `page-item ${currentOrdersPage === 1 ? 'disabled' : ''}`;
        prevLi.innerHTML = `<a class="page-link" href="#">Anterior</a>`;
        prevLi.addEventListener('click', (e) => {
            e.preventDefault();
            if (currentOrdersPage > 1) {
                currentOrdersPage--;
                renderOrdersTable();
            }
        });
        pagination.appendChild(prevLi);
        
        for (let i = 1; i <= totalPages; i++) {
            const li = document.createElement('li');
            li.className = `page-item ${i === currentOrdersPage ? 'active' : ''}`;
            li.innerHTML = `<a class="page-link" href="#">${i}</a>`;
            li.addEventListener('click', (e) => {
                e.preventDefault();
                currentOrdersPage = i;
                renderOrdersTable();
            });
            pagination.appendChild(li);
        }
        
        const nextLi = document.createElement('li');
        nextLi.className = `page-item ${currentOrdersPage === totalPages ? 'disabled' : ''}`;
        nextLi.innerHTML = `<a class="page-link" href="#">Siguiente</a>`;
        nextLi.addEventListener('click', (e) => {
            e.preventDefault();
            if (currentOrdersPage < totalPages) {
                currentOrdersPage++;
                renderOrdersTable();
            }
        });
        pagination.appendChild(nextLi);
    }

    function updateOrderStatus(orderNumber, newStatus) {
        const order = mockOrders.find(o => o.orderNumber === orderNumber);
        if (order) {
            // Verifica si la acción es válida
            if (newStatus === 'delivered' && order.status !== 'ready') {
                // Mostrar el mensaje de error si se intenta entregar un pedido que no está listo
                document.getElementById('error-message').style.display = 'block';
                document.getElementById('error-message').textContent = 'El pedido no está listo para ser entregado';
                return;
            } else {
                document.getElementById('error-message').style.display = 'none';
            }
        
            // Actualizar el estado si no hay error
            order.status = newStatus;
            order.statusUpdatedAt = new Date().toISOString();
            
            // Eliminar el alert y solo actualizar la tabla
            renderOrdersTable();
        }
    }
    

    function getStatusText(status) {
        const statusTexts = {
            'pending': 'Pendiente',
            'preparation': 'En preparación',
            'ready': 'Listo',
            'delivered': 'Entregado',
            'canceled': 'Cancelado'
        };
        return statusTexts[status] || status;
    }

    function setRating(rating) {
        currentRating = rating;
        const stars = document.querySelectorAll('.rating-stars i');
        stars.forEach((star, index) => {
            if (index < rating) {
                star.classList.remove('far');
                star.classList.add('fas', 'active');
            } else {
                star.classList.remove('fas', 'active');
                star.classList.add('far');
            }
        });
    }

    function highlightStars(rating) {
        const stars = document.querySelectorAll('.rating-stars i');
        stars.forEach((star, index) => {
            if (index < rating) {
                star.classList.add('hover');
            } else {
                star.classList.remove('hover');
            }
        });
    }

    function submitRating() {
        // Verificar si se ha seleccionado una calificación
        if (currentRating === 0) {
            // Mostrar el mensaje de error si no se selecciona ninguna calificación
            document.getElementById('error-message').style.display = 'block';
            return; // Detener la ejecución si no se selecciona una calificación
        }
    
        // Si hay una calificación seleccionada, ocultar el mensaje de error
        document.getElementById('error-message').style.display = 'none';
    
        // Obtener los comentarios del usuario
        const comments = document.getElementById('ratingComments').value;
    
        // Mostrar en la consola los datos de la valoración (esto puede ser reemplazado por lógica para enviarlo a un servidor)
        console.log('Valoración enviada:', {
            rating: currentRating,
            comments: comments,
            orderNumber: 'ORD-2023-0001'  // Aquí se podría poner un número de pedido dinámico
        });
    
        // Mostrar el mensaje de agradecimiento debajo del formulario de valoración
        const thanksMessage = document.getElementById('thanksMessage');
        thanksMessage.style.display = 'block';  // Mostrar el mensaje
    
        // Opcional: Ocultar el mensaje después de un tiempo
        setTimeout(function() {
            thanksMessage.style.display = 'none'; // Ocultar el mensaje después de 5 segundos
        }, 5000);
    
        // Ocultar el modal de valoración después de enviar la información
        const modal = bootstrap.Modal.getInstance(document.getElementById('ratingModal'));
        modal.hide();
    
        // Reiniciar los campos del formulario
        setRating(0); // Resetear las estrellas
        document.getElementById('ratingComments').value = '';  // Limpiar el campo de comentarios
        document.getElementById('charsLeft').textContent = '500';  // Restablecer el contador de caracteres
    }
    
    
    
    function loadCategory(category) {
        const productGrid = document.getElementById('productGrid');
        productGrid.innerHTML = '';

        const products = menuData[category];

        products.forEach(product => {
            const productCard = document.createElement('div');
            productCard.className = 'col-md-6 col-lg-4';
            productCard.innerHTML = `
                <div class="product-card" data-id="${product.id}">
                    <img src="${product.image}" alt="${product.name}" class="product-img">
                    <div class="product-body">
                        <div class="product-title">${product.name}</div>
                        <div class="product-description small text-muted">${product.description}</div>
                        <div class="product-price">${product.price > 0 ? '₡' + product.price.toLocaleString() : 'Consultar precio'}</div>
                    </div>
                </div>
            `;

            productCard.querySelector('.product-card').addEventListener('click', () => {
                if (product.price > 0) {
                    addToOrder(product);
                } else {
                    openCustomPriceModal(product);
                }
            });

            productGrid.appendChild(productCard);
        });
    }

    function openCustomPriceModal(product) {
        const customPrice = prompt(`Ingrese el precio para ${product.name}:`, "0");

        if (customPrice && !isNaN(customPrice) && parseFloat(customPrice) > 0) {
            addToOrder({
                ...product,
                price: parseFloat(customPrice)
            });
        } else if (customPrice !== null) {
            alert("Por favor ingrese un precio válido");
        }
    }

    function addToOrder(product) {
        const existingItem = currentOrder.find(item => item.id === product.id);

        if (existingItem) {
            existingItem.quantity += 1;
        } else {
            currentOrder.push({
                id: product.id,
                name: product.name,
                price: product.price,
                quantity: 1
            });
        }

        updateOrderDisplay();
    }

    function updateOrderDisplay() {
        const orderItemsContainer = document.getElementById('orderItems');

        if (currentOrder.length === 0) {
            orderItemsContainer.innerHTML = `
                <div class="empty-order">
                    <i class="fas fa-shopping-cart"></i>
                    <p>No hay productos en el pedido</p>
                </div>
            `;
            document.getElementById('checkoutBtn').disabled = true;
        } else {
            let orderHTML = '';
            let subtotal = 0;

            currentOrder.forEach(item => {
                const itemTotal = item.price * item.quantity;
                subtotal += itemTotal;

                orderHTML += `
                    <div class="order-item" data-id="${item.id}">
                        <div class="item-name">${item.name}</div>
                        <div class="item-quantity">
                            <button class="decrease-quantity"><i class="fas fa-minus"></i></button>
                            <input type="number" value="${item.quantity}" min="1" class="quantity-input">
                            <button class="increase-quantity"><i class="fas fa-plus"></i></button>
                        </div>
                        <div class="item-price">₡${itemTotal.toLocaleString()}</div>
                        <div class="item-remove"><i class="fas fa-times"></i></div>
                    </div>
                `;
            });

            orderItemsContainer.innerHTML = orderHTML;

            const tax = subtotal * 0.13;
            const total = subtotal + tax;

            document.getElementById('orderSubtotal').textContent = `₡${subtotal.toLocaleString()}`;
            document.getElementById('orderTax').textContent = `₡${tax.toLocaleString()}`;
            document.getElementById('orderTotal').textContent = `₡${total.toLocaleString()}`;

            document.getElementById('checkoutBtn').disabled = false;

            document.querySelectorAll('.decrease-quantity').forEach(button => {
                button.addEventListener('click', function () {
                    const itemId = parseInt(this.closest('.order-item').getAttribute('data-id'));
                    const item = currentOrder.find(item => item.id === itemId);

                    if (item.quantity > 1) {
                        item.quantity -= 1;
                        updateOrderDisplay();
                    }
                });
            });

            document.querySelectorAll('.increase-quantity').forEach(button => {
                button.addEventListener('click', function () {
                    const itemId = parseInt(this.closest('.order-item').getAttribute('data-id'));
                    const item = currentOrder.find(item => item.id === itemId);
                    item.quantity += 1;
                    updateOrderDisplay();
                });
            });

            document.querySelectorAll('.quantity-input').forEach(input => {
                input.addEventListener('change', function () {
                    const itemId = parseInt(this.closest('.order-item').getAttribute('data-id'));
                    const item = currentOrder.find(item => item.id === itemId);
                    const newQuantity = parseInt(this.value);

                    if (newQuantity > 0) {
                        item.quantity = newQuantity;
                        updateOrderDisplay();
                    } else {
                        this.value = item.quantity;
                    }
                });
            });

            document.querySelectorAll('.item-remove').forEach(button => {
                button.addEventListener('click', function () {
                    const itemId = parseInt(this.closest('.order-item').getAttribute('data-id'));
                    currentOrder = currentOrder.filter(item => item.id !== itemId);
                    updateOrderDisplay();
                });
            });
        }
    }

    function clearOrder() {
        if (currentOrder.length > 0 && confirm('¿Está seguro que desea limpiar el pedido actual?')) {
            currentOrder = [];
            updateOrderDisplay();
        }
    }

    function openPaymentModal() {
        if (currentOrder.length === 0) return;

        const modal = new bootstrap.Modal(document.getElementById('paymentModal'));

        const paymentOrderItems = document.getElementById('paymentOrderItems');
        let itemsHTML = '';
        let subtotal = 0;

        currentOrder.forEach(item => {
            const itemTotal = item.price * item.quantity;
            subtotal += itemTotal;

            itemsHTML += `
                <div class="order-item">
                    <div class="item-name">${item.name} x${item.quantity}</div>
                    <div class="item-price">₡${itemTotal.toLocaleString()}</div>
                </div>
            `;
        });

        paymentOrderItems.innerHTML = itemsHTML;

        const tax = subtotal * 0.13;
        const total = subtotal + tax;

        document.getElementById('paymentSubtotal').textContent = `₡${subtotal.toLocaleString()}`;
        document.getElementById('paymentTax').textContent = `₡${tax.toLocaleString()}`;
        document.getElementById('paymentTotal').textContent = `₡${total.toLocaleString()}`;

        selectPaymentMethod('efectivo');
        document.getElementById('cashAmount').value = '';
        document.getElementById('cashChange').value = '';

        modal.show();
    }

    function selectPaymentMethod(method) {
        document.querySelectorAll('.payment-method').forEach(m => {
            m.classList.remove('active');
        });
        document.querySelector(`[data-method="${method}"]`).classList.add('active');

        document.querySelectorAll('.payment-details').forEach(d => {
            d.style.display = 'none';
        });
        document.getElementById(`${method}Details`).style.display = 'block';
    }

    function calculateChange() {
        const cashAmount = parseFloat(document.getElementById('cashAmount').value) || 0;
        const total = parseFloat(document.getElementById('paymentTotal').textContent.replace('₡', '').replace(',', ''));

        const change = cashAmount - total;
        document.getElementById('cashChange').value = change >= 0 ? `₡${change.toLocaleString()}` : '0';
    }

    function confirmPayment() {
        const paymentMethod = document.querySelector('.payment-method.active').getAttribute('data-method');
        let paymentValid = true;
        
        const total = parseFloat(document.getElementById('paymentTotal').textContent.replace('₡', '').replace(',', ''));
        const cashAmount = parseFloat(document.getElementById('cashAmount').value) || 0;
        
        // Verifica si el monto recibido es menor que el total
        const errorMessage = document.getElementById('error-message');
        
        if (paymentMethod === 'efectivo') {
            if (cashAmount < total) {
                // Si el monto recibido es menor que el total, muestra el mensaje de error
                errorMessage.textContent = 'El monto recibido es menor que el total a pagar';
                errorMessage.style.display = 'block';
                paymentValid = false;
            } else if (cashAmount >= total) {
                // Si el monto recibido es mayor o igual al total, oculta el mensaje de error
                errorMessage.style.display = 'none';
            }
        }
        
        // Verificación adicional para otros métodos de pago
        if (paymentMethod === 'tarjeta') {
            const cardNumber = document.getElementById('cardNumber').value;
            const cardExpiry = document.getElementById('cardExpiry').value;
            const cardCVV = document.getElementById('cardCVV').value;
            
            if (!cardNumber || !cardExpiry || !cardCVV) {
                alert('Por favor complete todos los datos de la tarjeta');
                paymentValid = false;
            }
        } else if (paymentMethod === 'sinpe') {
            const sinpeNumber = document.getElementById('sinpeNumber').value;
            const sinpeConfirmation = document.getElementById('sinpeConfirmation').value;
            
            if (!sinpeNumber || !sinpeConfirmation) {
                alert('Por favor complete todos los datos del SINPE Móvil');
                paymentValid = false;
            }
        }
        
        // Si la validación fue correcta, continúa con el proceso
        if (paymentValid) {
            // Proceder con el pago
            const paymentModal = bootstrap.Modal.getInstance(document.getElementById('paymentModal'));
            paymentModal.hide();
        
            const orderCompleteModal = new bootstrap.Modal(document.getElementById('orderCompleteModal'));
            orderCompleteModal.show();
        
            const orderNumber = `ORD-${new Date().getFullYear()}-${Math.floor(1000 + Math.random() * 9000)}`;
            document.querySelector('.order-number strong').textContent = orderNumber;
        
            console.log('Order completed:', {
                orderNumber,
                items: currentOrder,
                customer: document.getElementById('customerName').value,
                phone: document.getElementById('customerPhone').value,
                takeaway: document.getElementById('takeawayOrder').checked,
                paymentMethod
            });
        }
    }

    // Función para abrir el modal de valoración
    function openRatingModal() {
        const ratingModal = new bootstrap.Modal(document.getElementById('ratingModal'));
        ratingModal.show();
    }

    // Añadir un event listener al botón "Valorar Pedido"
    document.getElementById('rateOrderBtn').addEventListener('click', openRatingModal);

    
    // Agregar un evento de input para la validación dinámica
    document.getElementById('cashAmount').addEventListener('input', function() {
        const paymentMethod = document.querySelector('.payment-method.active').getAttribute('data-method');
        const total = parseFloat(document.getElementById('paymentTotal').textContent.replace('₡', '').replace(',', ''));
        const cashAmount = parseFloat(this.value) || 0;
        const errorMessage = document.getElementById('error-message');
        
        if (paymentMethod === 'efectivo') {
            if (cashAmount < total) {
                // Muestra el mensaje de error si el monto es menor que el total
                errorMessage.textContent = 'El monto recibido es menor que el total a pagar';
                errorMessage.style.display = 'block';
            } else if (cashAmount >= total) {
                // Si el monto es mayor o igual, oculta el mensaje
                errorMessage.style.display = 'none';
            }
        }
    });
    

    function saveOrder() {
        if (currentOrder.length === 0) return;

        if (confirm('¿Guardar este pedido sin procesar pago?')) {
            console.log('Order saved:', {
                items: currentOrder,
                customer: document.getElementById('customerName').value,
                phone: document.getElementById('customerPhone').value,
                takeaway: document.getElementById('takeawayOrder').checked
            });

            alert('Pedido guardado exitosamente');
            clearOrder();
        }
    }

    function startNewOrder() {
        // Cierra el modal de "Pedido Completado"
        const orderCompleteModal = bootstrap.Modal.getInstance(document.getElementById('orderCompleteModal'));
        if (orderCompleteModal) {
            orderCompleteModal.hide();  // Cierra el modal
        }
    
        // Limpia el pedido actual y reinicia los campos
        currentOrder = [];
        updateOrderDisplay();
        document.getElementById('customerName').value = '';
        document.getElementById('customerPhone').value = '';
        document.getElementById('takeawayOrder').checked = false;
    
        // Ocultar el mensaje de error al iniciar un nuevo pedido
        document.getElementById('error-message').style.display = 'none';
    }
    

    function printReceipt() {
        // Muestra el mensaje debajo del botón "Imprimir Factura"
        const printMessage = document.getElementById('printMessage');
        printMessage.style.display = 'block'; // Muestra el mensaje
    
        console.log("Factura enviada a imprimir");
    
        // Ocultar el mensaje después de un tiempo
        setTimeout(function() {
            printMessage.style.display = 'none'; // Oculta el mensaje después de 5 segundos
        }, 5000);
    }
    
    
    function updateCurrentTime() {
        const now = new Date();
        const timeString = now.toLocaleTimeString();
        const dateString = now.toLocaleDateString();
        document.getElementById('currentTime').textContent = `${dateString} ${timeString}`;
    }

    

    initPOS();
});