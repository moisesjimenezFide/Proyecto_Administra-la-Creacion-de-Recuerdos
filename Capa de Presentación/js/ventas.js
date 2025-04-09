document.addEventListener('DOMContentLoaded', function () {
    const ventas = [
        {
            id: 1, fecha: "2025-04-01", productos: [
                { id: 1, nombre: "Producto A", cantidad: 2, precio: 100 },
                { id: 2, nombre: "Producto B", cantidad: 1, precio: 200 }
            ], total: 400
        },

        {
            id: 2, fecha: "2025-04-02", productos: [
                { id: 3, nombre: "Producto C", cantidad: 3, precio: 150 }
            ], total: 450
        },

        {
            id: 3, fecha: "2025-04-03", productos: [
                { id: 4, nombre: "Producto D", cantidad: 5, precio: 250 },
                { id: 1, nombre: "Producto A", cantidad: 1, precio: 100 }
            ], total: 1350
        },

        {
            id: 4, fecha: "2025-04-04", productos: [
                { id: 2, nombre: "Producto B", cantidad: 2, precio: 200 }
            ], total: 400
        },

        {
            id: 5, fecha: "2025-04-05", productos: [
                { id: 5, nombre: "Producto E", cantidad: 4, precio: 300 },
                { id: 3, nombre: "Producto C", cantidad: 2, precio: 150 }
            ], total: 1200
        },

        {
            id: 6, fecha: "2025-04-06", productos: [
                { id: 6, nombre: "Producto F", cantidad: 1, precio: 500 },
                { id: 4, nombre: "Producto D", cantidad: 3, precio: 250 }
            ], total: 1750
        },

        {
            id: 7, fecha: "2025-04-07", productos: [
                { id: 7, nombre: "Producto G", cantidad: 2, precio: 400 }
            ], total: 800
        },

        {
            id: 8, fecha: "2025-04-08", productos: [
                { id: 8, nombre: "Producto H", cantidad: 1, precio: 600 },
                { id: 9, nombre: "Producto I", cantidad: 1, precio: 500 }
            ], total: 1100
        },

        {
            id: 9, fecha: "2025-04-09", productos: [
                { id: 10, nombre: "Producto J", cantidad: 2, precio: 350 },
                { id: 5, nombre: "Producto E", cantidad: 1, precio: 300 }
            ], total: 1000
        },

        {
            id: 10, fecha: "2025-04-10", productos: [
                { id: 6, nombre: "Producto F", cantidad: 2, precio: 500 },
                { id: 7, nombre: "Producto G", cantidad: 1, precio: 400 }
            ], total: 1400
        }
    ];
});
