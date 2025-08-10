document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("form-precio-final");
    const tabla = document.querySelector("#tabla_precios tbody");
    const busqueda = document.getElementById("busqueda");
    const mensajeNoResultados = document.getElementById("mensaje_no_resultados");
  
    let productos = [
      {
        nombre: "Galleta Artesanal",
        costo: 1200.0,
        utilidad: 40,
        empaque: 100.0,
        iva: 13,
        servicio: 5,
        envio: 3
      },
      {
        nombre: "Pan de Banano",
        costo: 1800.0,
        utilidad: 35,
        empaque: 150.0,
        iva: 13,
        servicio: 5,
        envio: 4
      },
      {
        nombre: "Cupcake Vainilla",
        costo: 800.0,
        utilidad: 45,
        empaque: 80.0,
        iva: 13,
        servicio: 5,
        envio: 2
      }
    ];
  
    let editandoIndex = null;
  
    function calcularPrecio(base, utilidad, iva, servicio, envio, comision = 0) {
      let subtotal = base + (base * (utilidad / 100));
      subtotal += subtotal * (iva / 100);
      subtotal += subtotal * (servicio / 100);
      subtotal += subtotal * (envio / 100);
      subtotal += subtotal * comision;
      return subtotal.toFixed(2);
    }
  
    function renderTabla() {
      const filtro = busqueda.value.toLowerCase();
      tabla.innerHTML = "";
      let hayResultados = false;
  
      productos.forEach((p, i) => {
        if (p.nombre.toLowerCase().includes(filtro)) {
          hayResultados = true;
          const base = p.costo + p.empaque;
          const costoMargen = (p.costo * (p.utilidad / 100)).toFixed(2);
          const canalPropio = calcularPrecio(base, p.utilidad, p.iva, p.servicio, p.envio);
          const uber = calcularPrecio(base, p.utilidad, p.iva, p.servicio, p.envio, 0.3);
          const rappi = calcularPrecio(base, p.utilidad, p.iva, p.servicio, p.envio, 0.25);
          const didi = calcularPrecio(base, p.utilidad, p.iva, p.servicio, p.envio, 0.2);
          const pedidosYa = calcularPrecio(base, p.utilidad, p.iva, p.servicio, p.envio, 0.25);
  
          tabla.innerHTML += `
            <tr>
              <td class="text-start">${p.nombre}</td>
              <td class="text-start">₡${p.costo.toFixed(2)}</td>
              <td class="text-start">${p.utilidad}%</td>
              <td class="text-start">₡${costoMargen}</td>
              <td class="text-start">₡${p.empaque.toFixed(2)}</td>
              <td class="text-start">${p.iva}%</td>
              <td class="text-start">${p.servicio}%</td>
              <td class="text-start">${p.envio}%</td>
              <td class="text-start">₡${canalPropio}</td>
              <td class="text-start">₡${uber}</td>
              <td class="text-start">₡${rappi}</td>
              <td class="text-start">₡${didi}</td>
              <td class="text-start">₡${pedidosYa}</td>
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
      const costo = parseFloat(document.getElementById("costoReceta").value);
      const utilidad = parseFloat(document.getElementById("utilidad").value);
      const empaque = parseFloat(document.getElementById("empaque").value);
      const iva = parseFloat(document.getElementById("iva").value);
      const servicio = parseFloat(document.getElementById("servicio").value);
      const envio = parseFloat(document.getElementById("envio").value);
  
      if (!nombre || isNaN(costo) || isNaN(utilidad) || isNaN(empaque) || isNaN(iva) || isNaN(servicio) || isNaN(envio)) {
        return Swal.fire("Error", "Todos los campos son obligatorios y deben ser válidos.", "warning");
      }
  
      const nuevo = { nombre, costo, utilidad, empaque, iva, servicio, envio };
  
      if (editandoIndex !== null) {
        productos[editandoIndex] = nuevo;
        editandoIndex = null;
        Swal.fire("Actualizado", "Producto modificado correctamente.", "success");
        form.querySelector('button[type="submit"]').textContent = "Guardar";
      } else {
        productos.push(nuevo);
        Swal.fire("Agregado", "Producto agregado exitosamente.", "success");
      }
  
      form.reset();
      renderTabla();
    });
  
    busqueda.addEventListener("input", renderTabla);
    document.getElementById("btn_busqueda").addEventListener("click", renderTabla);
  
    window.eliminar = function (index) {
      Swal.fire({
        title: "¿Eliminar producto?",
        text: `¿Está seguro de eliminar "${productos[index].nombre}"?`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Sí, eliminar",
        cancelButtonText: "Cancelar"
      }).then(result => {
        if (result.isConfirmed) {
          productos.splice(index, 1);
          renderTabla();
          Swal.fire("Eliminado", "Producto eliminado correctamente.", "success");
        }
      });
    };
  
    window.editar = function (index) {
      const p = productos[index];
      document.getElementById("nombre").value = p.nombre;
      document.getElementById("costoReceta").value = p.costo;
      document.getElementById("utilidad").value = p.utilidad;
      document.getElementById("empaque").value = p.empaque;
      document.getElementById("iva").value = p.iva;
      document.getElementById("servicio").value = p.servicio;
      document.getElementById("envio").value = p.envio;
      editandoIndex = index;
      form.querySelector('button[type="submit"]').textContent = "Actualizar";
    };
  
    renderTabla();
  });
  