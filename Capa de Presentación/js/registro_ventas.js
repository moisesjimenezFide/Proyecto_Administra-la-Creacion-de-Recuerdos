document.addEventListener("DOMContentLoaded", () => {
    const productos = [
      { id: 1, nombre: "Galleta Artesanal", precio: 1200 },
      { id: 2, nombre: "Pan de Banano", precio: 1800 },
      { id: 3, nombre: "Cupcake Vainilla", precio: 900 },
    ];
  
    let productosSeleccionados = [];
    let totalVenta = 0;
  
    const productoSelect = document.getElementById("producto");
    const cantidadInput = document.getElementById("cantidad");
    const precioUnitarioSpan = document.getElementById("precioUnitario");
    const totalProductoSpan = document.getElementById("totalProducto");
    const tablaProductos = document.getElementById("tablaProductos");
    const totalVentaSpan = document.getElementById("totalVenta");
  
    productos.forEach(producto => {
      const option = document.createElement("option");
      option.value = producto.id;
      option.textContent = producto.nombre;
      productoSelect.appendChild(option);
    });
  
    productoSelect.addEventListener("change", actualizarPrecioYTotal);
    cantidadInput.addEventListener("input", actualizarPrecioYTotal);
  
    function actualizarPrecioYTotal() {
      const productoId = parseInt(productoSelect.value);
      const cantidad = parseInt(cantidadInput.value) || 0;
      const producto = productos.find(p => p.id === productoId);
      if (producto) {
        precioUnitarioSpan.textContent = `₡${producto.precio.toFixed(2)}`;
        totalProductoSpan.textContent = `₡${(producto.precio * cantidad).toFixed(2)}`;
      } else {
        precioUnitarioSpan.textContent = "₡0.00";
        totalProductoSpan.textContent = "₡0.00";
      }
    }
  
    document.getElementById("agregarProducto").addEventListener("click", () => {
      const productoId = parseInt(productoSelect.value);
      const cantidad = parseInt(cantidadInput.value);
      if (!productoId || cantidad <= 0) return;
  
      const producto = productos.find(p => p.id === productoId);
      const total = producto.precio * cantidad;
  
      productosSeleccionados.push({ ...producto, cantidad, total });
      totalVenta += total;
      renderTabla();
    });
  
    document.getElementById("confirmarVenta").addEventListener("click", () => {
      if (productosSeleccionados.length === 0) {
        Swal.fire("Error", "Debe agregar al menos un producto.", "warning");
        return;
      }
      Swal.fire("Venta confirmada", "La venta se registró exitosamente.", "success");
      cancelarVenta();
    });
  
    window.cancelarVenta = function () {
      productosSeleccionados = [];
      totalVenta = 0;
      renderTabla();
      document.getElementById("ventaForm").reset();
      actualizarPrecioYTotal();
    };
  
    window.eliminarProducto = function (index) {
      totalVenta -= productosSeleccionados[index].total;
      productosSeleccionados.splice(index, 1);
      renderTabla();
    };
  
    function renderTabla() {
        tablaProductos.innerHTML = "";
        totalVenta = 0;
      
        productosSeleccionados.forEach((p, i) => {
          totalVenta += parseFloat(p.total) || 0;
      
          tablaProductos.innerHTML += `
            <tr>
              <td>${p.nombre}</td>
              <td>₡${p.precio.toFixed(2)}</td>
              <td>${p.cantidad}</td>
              <td>₡${p.total.toFixed(2)}</td>
              <td>
                <button class="btn btn-danger btn-sm px-2 py-1" onclick="eliminarProducto(${i})">
                  <i class="fas fa-trash-alt"></i> Eliminar
                </button>
              </td>
            </tr>
          `;
        });
      
        totalVentaSpan.textContent = `₡${totalVenta.toFixed(2)}`;
      }
      
  
    // Inicial
    actualizarPrecioYTotal();
  });
  