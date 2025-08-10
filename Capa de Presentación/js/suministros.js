document.addEventListener("DOMContentLoaded", () => {
  const suministros = [
    {
      nombre: "Jabón Antibacterial",
      marca: "CleanMax",
      presentacion: "500ml",
      proveedor: "Distribuidora La Limpia",
      costo_unidad: 1200.0,
      cantidad: 10,
      costo_total: 12000.0
    },
    {
      nombre: "Desinfectante Multiusos",
      marca: "LimpioPlus",
      presentacion: "1L",
      proveedor: "Químicos del Valle",
      costo_unidad: 1500.0,
      cantidad: 5,
      costo_total: 7500.0
    },
    {
      nombre: "Toallas de Papel",
      marca: "EcoWipe",
      presentacion: "6 rollos",
      proveedor: "Papeles San José",
      costo_unidad: 2100.0,
      cantidad: 7,
      costo_total: 14700.0
    }
  ];

  let indiceEditar = null;

  const tabla = document.getElementById("tabla_suministros");
  const form = document.getElementById("form_suministro");

  // Función para renderizar la tabla
  const renderizarTabla = () => {
    tabla.innerHTML = "";  // Limpiar la tabla

    suministros.forEach((s, i) => {
      const fila = document.createElement("tr");

      // Crear contenido de la fila
      fila.innerHTML = `
        <td>${s.nombre}</td>
        <td>${s.marca}</td>
        <td>${s.presentacion}</td>
        <td>${s.proveedor}</td>
        <td>₡${s.costo_unidad.toFixed(2)}</td>
        <td>${s.cantidad}</td>
        <td>₡${s.costo_total.toFixed(2)}</td>
        <td>
          <div class="d-flex justify-content-start gap-2" style="white-space: nowrap;">
            <!-- Botón Eliminar alineado a la izquierda -->
            <button class="btn btn-danger btn-sm text-white px-2 py-1" onclick="eliminarSuministro(${i})">
              <i class="fas fa-trash-alt me-1"></i>Eliminar
            </button>
            <!-- Botón Editar alineado a la derecha -->
            <button class="btn btn-sm text-white px-2 py-1" style="background-color:#E27AB0; margin-left: auto;" onclick="editarSuministro(${i})">
              <i class="fas fa-edit me-1"></i>Editar
            </button>
          </div>
        </td>
      `;

      // Agregar la fila a la tabla
      tabla.appendChild(fila);
    });
  };

  // Llamar a la función para mostrar la tabla al cargar la página
  renderizarTabla();

  // Manejo del formulario de agregar o editar suministro
  form.addEventListener("submit", e => {
    e.preventDefault();
    const nombre = document.getElementById("nombre").value.trim();
    const marca = document.getElementById("marca").value.trim();
    const presentacion = document.getElementById("presentacion").value.trim();
    const proveedor = document.getElementById("proveedor").value.trim();
    const costo_unidad = parseFloat(document.getElementById("costo_unidad").value);
    const cantidad = parseFloat(document.getElementById("cantidad").value);
    const costo_total = costo_unidad * cantidad;

    if ([costo_unidad, cantidad].some(val => isNaN(val) || val < 0)) {
      return Swal.fire("Error", "Los valores deben ser válidos y no negativos.", "warning");
    }

    if (indiceEditar === null) {
      if (suministros.find(s => s.nombre.toLowerCase() === nombre.toLowerCase())) {
        return Swal.fire("Duplicado", "Este suministro ya existe.", "warning");
      }
      suministros.push({ nombre, marca, presentacion, proveedor, costo_unidad, cantidad, costo_total });
      Swal.fire("¡Agregado!", "Suministro agregado exitosamente.", "success");
    } else {
      suministros[indiceEditar] = { nombre, marca, presentacion, proveedor, costo_unidad, cantidad, costo_total };
      indiceEditar = null;
      form.querySelector("button[type='submit']").textContent = "Agregar";
      Swal.fire("Actualizado", "Suministro actualizado exitosamente.", "success");
    }

    form.reset();
    renderizarTabla();
  });

  // Función para editar un suministro
  window.editarSuministro = index => {
    const s = suministros[index];
    Swal.fire({
      title: `¿Editar "${s.nombre}"?`,
      icon: "question",
      showCancelButton: true,
      confirmButtonText: "Editar",
      cancelButtonText: "Cancelar"
    }).then(res => {
      if (res.isConfirmed) {
        document.getElementById("nombre").value = s.nombre;
        document.getElementById("marca").value = s.marca;
        document.getElementById("presentacion").value = s.presentacion;
        document.getElementById("proveedor").value = s.proveedor;
        document.getElementById("costo_unidad").value = s.costo_unidad;
        document.getElementById("cantidad").value = s.cantidad;
        indiceEditar = index;
        form.querySelector("button[type='submit']").textContent = "Actualizar";
      }
    });
  };

  // Función para eliminar un suministro
  window.eliminarSuministro = index => {
    Swal.fire({
      title: `¿Eliminar "${suministros[index].nombre}"?`,
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí",
      cancelButtonText: "No"
    }).then(res => {
      if (res.isConfirmed) {
        suministros.splice(index, 1);
        renderizarTabla();
        Swal.fire("Eliminado", "Suministro eliminado correctamente.", "success");
      }
    });
  };
});
