document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("form_productos_preparados");
    const tabla = document.querySelector("#tabla_productos_preparados");
    const busqueda = document.getElementById("busqueda");
    const mensajeNoResultados = document.getElementById("mensaje_no_resultados");
  
    let productos_preparados = [
      {
        nombre: "Pan Casero",
        marca: "Tradición",
        presentacion: "500g",
        proveedor: "Panadería La Fina",
        costo_unidad: 1200,
        cantidad: 5
      },
      {
        nombre: "Galletas Artesanales",
        marca: "Dulce Hogar",
        presentacion: "Caja 12 unidades",
        proveedor: "Distribuidora Gourmet",
        costo_unidad: 2500,
        cantidad: 3
      },
      {
        nombre: "Tarta de Frutas",
        marca: "La Delicia",
        presentacion: "1 unidad",
        proveedor: "Frutarte",
        costo_unidad: 4000,
        cantidad: 2
      }
    ];
  
    let edit_index = null;
  
    function renderTabla() {
      const filtro = busqueda.value.toLowerCase();
      tabla.innerHTML = "";
      let hayResultados = false;
  
      productos_preparados.forEach((p, i) => {
        const texto = `${p.nombre} ${p.marca} ${p.presentacion} ${p.proveedor}`.toLowerCase();
  
        if (texto.includes(filtro)) {
          hayResultados = true;
          const costo_total = p.costo_unidad * p.cantidad;
  
          tabla.innerHTML += `
            <tr>
              <td class="text-start">${p.nombre}</td>
              <td class="text-start">${p.marca}</td>
              <td class="text-start">${p.presentacion}</td>
              <td class="text-start">${p.proveedor}</td>
              <td class="text-start">₡${p.costo_unidad.toFixed(2)}</td>
              <td class="text-start">${p.cantidad}</td>
              <td class="text-start">₡${costo_total.toFixed(2)}</td>
              <td>
                <div class="d-flex flex-nowrap justify-content-start gap-2" style="white-space: nowrap;">
                  <button class="btn btn-danger btn-sm text-white px-2 py-1" onclick="eliminar(${i})">
                    <i class="fas fa-trash-alt me-1"></i>Eliminar
                  </button>
                  <button class="btn btn-sm text-white px-2 py-1" style="background-color: #E27AB0;" onclick="editar(${i})">
                    <i class="fas fa-edit me-1"></i>Editar
                  </button>
                </div>
              </td>
            </tr>
          `;
        }
      });
  
      mensajeNoResultados.style.display = hayResultados ? "none" : "block";
    }
  
    form.addEventListener("submit", function (e) {
      e.preventDefault();
  
      const nombre = document.getElementById("nombre").value.trim();
      const marca = document.getElementById("marca").value.trim();
      const presentacion = document.getElementById("presentacion").value.trim();
      const proveedor = document.getElementById("proveedor").value.trim();
      const costo_unidad = parseFloat(document.getElementById("costo_unidad").value);
      const cantidad = parseFloat(document.getElementById("cantidad").value);
  
      if (!nombre || !marca || !presentacion || !proveedor || isNaN(costo_unidad) || isNaN(cantidad)) {
        return Swal.fire("Error", "Todos los campos son obligatorios y deben ser válidos.", "warning");
      }
  
      const nuevo = { nombre, marca, presentacion, proveedor, costo_unidad, cantidad };
  
      if (edit_index !== null) {
        productos_preparados[edit_index] = nuevo;
        edit_index = null;
        Swal.fire("Actualizado", "Producto preparado actualizado correctamente.", "success");
        form.querySelector('button[type="submit"]').textContent = "Agregar";
      } else {
        productos_preparados.push(nuevo);
        Swal.fire("Agregado", "Producto preparado agregado exitosamente.", "success");
      }
  
      form.reset();
      renderTabla();
    });
  
    busqueda.addEventListener("input", renderTabla);
    document.getElementById("btn_buscar").addEventListener("click", renderTabla);
  
    window.eliminar = function (index) {
      Swal.fire({
        title: "¿Eliminar producto?",
        text: `¿Está seguro de eliminar "${productos_preparados[index].nombre}"?`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Sí, eliminar",
        cancelButtonText: "Cancelar"
      }).then(result => {
        if (result.isConfirmed) {
          productos_preparados.splice(index, 1);
          renderTabla();
          Swal.fire("Eliminado", "Producto eliminado correctamente.", "success");
        }
      });
    };
  
    window.editar = function (index) {
      const p = productos_preparados[index];
      document.getElementById("nombre").value = p.nombre;
      document.getElementById("marca").value = p.marca;
      document.getElementById("presentacion").value = p.presentacion;
      document.getElementById("proveedor").value = p.proveedor;
      document.getElementById("costo_unidad").value = p.costo_unidad;
      document.getElementById("cantidad").value = p.cantidad;
      edit_index = index;
      form.querySelector('button[type="submit"]').textContent = "Actualizar";
    };
  
    renderTabla();
  });
  