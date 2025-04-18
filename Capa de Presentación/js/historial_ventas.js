let ventas = [
    {
      fecha: "2024-10-01",
      productos: [
        { nombre: "Café Latte", precio: 1500, cantidad: 2, total: 3000 },
        { nombre: "Panini Jamón", precio: 2500, cantidad: 1, total: 2500 }
      ],
      totalVenta: 5500
    },
    {
      fecha: "2024-10-02",
      productos: [
        { nombre: "Smoothie Fresa", precio: 1800, cantidad: 1, total: 1800 },
        { nombre: "Tarta de Queso", precio: 2300, cantidad: 2, total: 4600 }
      ],
      totalVenta: 6400
    },
    {
      fecha: "2024-10-03",
      productos: [
        { nombre: "Capuccino", precio: 1400, cantidad: 1, total: 1400 },
        { nombre: "Croissant", precio: 1700, cantidad: 1, total: 1700 }
      ],
      totalVenta: 3100
    }
  ];
  
  sessionStorage.setItem("ventas", JSON.stringify(ventas));
  
  function renderizarVentas(lista) {
    const tbody = document.getElementById("historialVentas");
    const mensaje = document.getElementById("mensaje_no_resultados");
    tbody.innerHTML = "";
  
    if (lista.length === 0) {
      mensaje.style.display = "block";
      return;
    }
  
    mensaje.style.display = "none";
  
    lista.forEach((venta, index) => {
      const nombres = venta.productos.map(p => p.nombre).join(", ");
      const precios = venta.productos.map(p => `₡${p.precio.toFixed(2)}`).join(", ");
      const cantidades = venta.productos.map(p => p.cantidad).join(", ");
      const totales = venta.productos.map(p => `₡${p.total.toFixed(2)}`).join(", ");
  
      const fila = document.createElement("tr");
      fila.innerHTML = `
        <td>${venta.fecha}</td>
        <td>${nombres}</td>
        <td>${precios}</td>
        <td>${cantidades}</td>
        <td>${totales}</td>
        <td>₡${venta.totalVenta.toFixed(2)}</td>
        <td>
  <div class="d-flex justify-content-center gap-2">
    <button class="btn btn-sm btn-danger text-white" onclick="eliminarVenta(${index})" style="white-space: nowrap; font-size: 0.8rem; padding: 0.3rem 0.6rem;">
      <i class="fas fa-trash-alt me-1"></i>Eliminar
    </button>
    <button class="btn btn-sm text-white" style="background-color: #E27AB0; white-space: nowrap; font-size: 0.8rem; padding: 0.3rem 0.6rem;" onclick="editarVenta(${index})">
      <i class="fas fa-edit me-1"></i>Editar
    </button>
  </div>
</td>

      `;
      tbody.appendChild(fila);
    });
  }
  
  function filtrarVentas() {
    const fecha = document.getElementById("filtroFecha").value;
    const todasLasVentas = JSON.parse(sessionStorage.getItem("ventas")) || [];
    if (!fecha) {
      renderizarVentas(todasLasVentas);
    } else {
      const filtradas = todasLasVentas.filter(v => v.fecha === fecha);
      renderizarVentas(filtradas);
    }
  }
  
  function editarVenta(index) {
    Swal.fire({
      title: 'Funcionalidad no disponible',
      text: 'La edición aún no está habilitada para el historial de ventas.',
      icon: 'info'
    });
  }
  
  function eliminarVenta(index) {
    const ventasActuales = JSON.parse(sessionStorage.getItem("ventas")) || [];
  
    Swal.fire({
      title: '¿Estás seguro?',
      text: 'Esta acción eliminará la venta del historial.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        ventasActuales.splice(index, 1);
        sessionStorage.setItem("ventas", JSON.stringify(ventasActuales));
        renderizarVentas(ventasActuales);
  
        Swal.fire({
          icon: 'success',
          title: 'Eliminado',
          text: 'La venta ha sido eliminada.',
          timer: 1500,
          showConfirmButton: false
        });
      }
    });
  }
  
  document.addEventListener("DOMContentLoaded", () => {
    const guardadas = JSON.parse(sessionStorage.getItem("ventas")) || [];
    renderizarVentas(guardadas);
  });
  