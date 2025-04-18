let empaques = [
    {
      nombre: "Caja de regalo",
      marca: "Festibox",
      presentacion: "Caja mediana",
      proveedor: "Empaques CR",
      costo_unidad: 200,
      cantidad: 10,
      costo_total: 2000
    },
    {
      nombre: "Bolsa de papel",
      marca: "EcoPack",
      presentacion: "25x30cm",
      proveedor: "EcoProveedores",
      costo_unidad: 50,
      cantidad: 100,
      costo_total: 5000
    },
    {
      nombre: "Lazo decorativo",
      marca: "FancyLoops",
      presentacion: "1m",
      proveedor: "Decoraciones S.A.",
      costo_unidad: 75,
      cantidad: 20,
      costo_total: 1500
    }
  ];
  
  const form = document.getElementById("form_empaques");
  const tabla = document.getElementById("tabla_empaques");
  const mensaje = document.getElementById("mensaje_no_resultados");
  let edit_index = -1;
  
  form.addEventListener("submit", function (e) {
    e.preventDefault();
  
    const nombre = document.getElementById("nombre").value.trim();
    const marca = document.getElementById("marca").value.trim();
    const presentacion = document.getElementById("presentacion").value.trim();
    const proveedor = document.getElementById("proveedor").value.trim();
    const costo_unidad = parseFloat(document.getElementById("costo_unidad").value);
    const cantidad = parseFloat(document.getElementById("cantidad").value);
  
    if ([nombre, marca, presentacion, proveedor].includes("") || isNaN(costo_unidad) || isNaN(cantidad)) {
      Swal.fire("Error", "Todos los campos deben completarse correctamente.", "error");
      return;
    }
  
    const costo_total = costo_unidad * cantidad;
  
    const empaque = { nombre, marca, presentacion, proveedor, costo_unidad, cantidad, costo_total };
  
    if (edit_index >= 0) {
      empaques[edit_index] = empaque;
      edit_index = -1;
      Swal.fire("Actualizado", "El empaque fue actualizado.", "success");
    } else {
      empaques.push(empaque);
      Swal.fire("Agregado", "El empaque fue agregado.", "success");
    }
  
    form.reset();
    mostrar_empaques();
  });
  
  form.addEventListener("reset", () => {
    edit_index = -1;
    form.querySelector("button[type='submit']").textContent = "Agregar";
  });
  
  function mostrar_empaques() {
    tabla.innerHTML = "";
    if (empaques.length === 0) {
      mensaje.style.display = "block";
      return;
    }
    mensaje.style.display = "none";
    empaques.forEach((e, i) => {
      tabla.innerHTML += `
  <tr>
    <td>${e.nombre}</td>
    <td>${e.marca}</td>
    <td>${e.presentacion}</td>
    <td>${e.proveedor}</td>
    <td>₡${e.costo_unidad.toFixed(2)}</td>
    <td>${e.cantidad}</td>
    <td>₡${e.costo_total.toFixed(2)}</td>
    <td>
      <div class="d-flex justify-content-center gap-2" style="white-space: nowrap;">
        <button class="btn btn-sm btn-danger text-white" onclick="eliminar_empaque(${i})">
          <i class="fas fa-trash-alt me-1"></i>Eliminar
        </button>
        <button class="btn btn-sm text-white" style="background-color: #E27AB0;" onclick="editar_empaque(${i})">
          <i class="fas fa-edit me-1"></i>Editar
        </button>
      </div>
    </td>
  </tr>
`;

    });
  }
  
  function eliminar_empaque(index) {
    Swal.fire({
      title: `¿Desea eliminar "${empaques[index].nombre}"?`,
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar"
    }).then(result => {
      if (result.isConfirmed) {
        empaques.splice(index, 1);
        mostrar_empaques();
        Swal.fire("Eliminado", "El empaque fue eliminado.", "success");
      }
    });
  }
  
  function editar_empaque(index) {
    const e = empaques[index];
    document.getElementById("nombre").value = e.nombre;
    document.getElementById("marca").value = e.marca;
    document.getElementById("presentacion").value = e.presentacion;
    document.getElementById("proveedor").value = e.proveedor;
    document.getElementById("costo_unidad").value = e.costo_unidad;
    document.getElementById("cantidad").value = e.cantidad;
    form.querySelector("button[type='submit']").textContent = "Actualizar";
    edit_index = index;
  }
  
  document.addEventListener("DOMContentLoaded", mostrar_empaques);