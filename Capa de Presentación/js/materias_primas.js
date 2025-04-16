let materias_primas = [
    {
      nombre: "Harina",
      marca: "La Selecta",
      presentacion: "1kg",
      proveedor: "Distribuidora Centroamericana",
      costo: 1200,
      peso: 1000,
      unidad: "g",
      merma: 5
    },
    {
      nombre: "Azúcar",
      marca: "Doña Blanca",
      presentacion: "2kg",
      proveedor: "Azucarera del Sur",
      costo: 1800,
      peso: 2000,
      unidad: "g",
      merma: 3
    },
    {
      nombre: "Leche en Polvo",
      marca: "Dos Pinos",
      presentacion: "500g",
      proveedor: "Dos Pinos S.A.",
      costo: 2500,
      peso: 500,
      unidad: "g",
      merma: 8
    }
  ];
  
  let indiceEditando = null;
  
  function calcularValores(materia) {
    materia.costo_por_gramo = materia.costo / materia.peso;
    materia.merma_total = materia.peso * (materia.merma / 100);
    materia.costo_merma = materia.merma_total * materia.costo_por_gramo;
    materia.total = materia.costo + materia.costo_merma;
    materia.costo_gr_merma = materia.total / (materia.peso - materia.merma_total);
    return materia;
  }
  
  function mostrar_materias_primas() {
    const tabla = document.getElementById("tabla_materias_primas");
    tabla.innerHTML = "";
  
    materias_primas.forEach((m, i) => {
      const materia = calcularValores({ ...m });
      const fila = document.createElement("tr");
  
      fila.innerHTML = `
        <td>${materia.nombre}</td>
        <td>${materia.marca}</td>
        <td>${materia.presentacion}</td>
        <td>${materia.proveedor}</td>
        <td>₡${materia.costo.toFixed(2)}</td>
        <td>${materia.peso}</td>
        <td>${materia.unidad}</td>
        <td>₡${materia.costo_por_gramo.toFixed(4)}</td>
        <td>${materia.merma}%</td>
        <td>${materia.merma_total.toFixed(2)}g</td>
        <td>₡${materia.costo_merma.toFixed(2)}</td>
        <td>₡${materia.total.toFixed(2)}</td>
        <td>₡${materia.costo_gr_merma.toFixed(4)}</td>
        <td>
          <div class="d-flex flex-nowrap justify-content-center gap-2" style="white-space: nowrap;">
            <button class="btn btn-danger btn-sm text-white" onclick="eliminarMateriaPrima(${i})">
              <i class="fas fa-trash-alt me-1"></i>Eliminar
            </button>
            <button class="btn btn-sm text-white" style="background-color: #E27AB0;" onclick="editarMateriaPrima(${i})">
              <i class="fas fa-edit me-1"></i>Editar
            </button>
          </div>
        </td>
      `;
      tabla.appendChild(fila);
    });
  }
  
  document.getElementById("form_materias_primas").addEventListener("submit", function (e) {
    e.preventDefault();
  
    const nombre = document.getElementById("nombre").value.trim();
    const marca = document.getElementById("marca").value.trim();
    const presentacion = document.getElementById("presentacion").value.trim();
    const proveedor = document.getElementById("proveedor").value.trim();
    const costo = parseFloat(document.getElementById("costo").value);
    const peso = parseFloat(document.getElementById("peso").value);
    const unidad = document.getElementById("unidad").value.trim();
    const merma = parseFloat(document.getElementById("merma").value);
  
    if (!nombre || !marca || !presentacion || !proveedor || isNaN(costo) || isNaN(peso) || isNaN(merma)) {
      return Swal.fire("Error", "Todos los campos son obligatorios.", "warning");
    }
  
    const nuevaMateria = calcularValores({
      nombre,
      marca,
      presentacion,
      proveedor,
      costo,
      peso,
      unidad,
      merma
    });
  
    if (indiceEditando !== null) {
      materias_primas[indiceEditando] = nuevaMateria;
      indiceEditando = null;
      Swal.fire("Actualizado", "Materia prima actualizada exitosamente.", "success");
    } else {
      materias_primas.push(nuevaMateria);
      Swal.fire("Agregado", "Materia prima agregada exitosamente.", "success");
    }
  
    e.target.reset();
    document.querySelector('button[type="submit"]').textContent = "Guardar";
    mostrar_materias_primas();
  });
  
  function editarMateriaPrima(index) {
    const m = materias_primas[index];
    document.getElementById("nombre").value = m.nombre;
    document.getElementById("marca").value = m.marca;
    document.getElementById("presentacion").value = m.presentacion;
    document.getElementById("proveedor").value = m.proveedor;
    document.getElementById("costo").value = m.costo;
    document.getElementById("peso").value = m.peso;
    document.getElementById("unidad").value = m.unidad;
    document.getElementById("merma").value = m.merma;
  
    indiceEditando = index;
    document.querySelector('button[type="submit"]').textContent = "Actualizar";
  }
  
  function eliminarMateriaPrima(index) {
    Swal.fire({
      title: `¿Eliminar "${materias_primas[index].nombre}"?`,
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar"
    }).then((result) => {
      if (result.isConfirmed) {
        materias_primas.splice(index, 1);
        mostrar_materias_primas();
        Swal.fire("Eliminado", "Materia prima eliminada correctamente.", "success");
      }
    });
  }
  
  function buscarMaterias() {
    const valor = document.getElementById("busqueda").value.toLowerCase();
    const resultados = materias_primas.filter(m =>
      `${m.nombre} ${m.marca} ${m.presentacion} ${m.proveedor}`.toLowerCase().includes(valor)
    );
  
    const tabla = document.getElementById("tabla_materias_primas");
    const mensaje = document.getElementById("mensaje_no_resultados");
    tabla.innerHTML = "";
  
    if (resultados.length === 0) {
      mensaje.style.display = "block";
    } else {
      mensaje.style.display = "none";
      resultados.forEach((m, i) => {
        const materia = calcularValores({ ...m });
        const fila = document.createElement("tr");
        fila.innerHTML = `
          <td>${materia.nombre}</td>
          <td>${materia.marca}</td>
          <td>${materia.presentacion}</td>
          <td>${materia.proveedor}</td>
          <td>₡${materia.costo.toFixed(2)}</td>
          <td>${materia.peso}</td>
          <td>${materia.unidad}</td>
          <td>₡${materia.costo_por_gramo.toFixed(4)}</td>
          <td>${materia.merma}%</td>
          <td>${materia.merma_total.toFixed(2)}g</td>
          <td>₡${materia.costo_merma.toFixed(2)}</td>
          <td>₡${materia.total.toFixed(2)}</td>
          <td>₡${materia.costo_gr_merma.toFixed(4)}</td>
          <td>
            <div class="d-flex flex-nowrap justify-content-center gap-2" style="white-space: nowrap;">
              <button class="btn btn-danger btn-sm text-white" onclick="eliminarMateriaPrima(${i})">
                <i class="fas fa-trash-alt me-1"></i>Eliminar
              </button>
              <button class="btn btn-sm text-white" style="background-color: #E27AB0;" onclick="editarMateriaPrima(${i})">
                <i class="fas fa-edit me-1"></i>Editar
              </button>
            </div>
          </td>
        `;
        tabla.appendChild(fila);
      });
    }
  }

  // Función para llenar las listas de Marca y Proveedor
function llenarListas() {
  const marcasSelect = document.getElementById("marca");
  const proveedoresSelect = document.getElementById("proveedor");

  // Limpiar las opciones existentes
  marcasSelect.innerHTML = '<option value="" disabled selected>Seleccione una marca</option>';
  proveedoresSelect.innerHTML = '<option value="" disabled selected>Seleccione un proveedor</option>';

  // Llenar las listas con las marcas y proveedores disponibles en materias_primas
  const marcas = new Set(materias_primas.map(m => m.marca));
  const proveedores = new Set(materias_primas.map(m => m.proveedor));

  marcas.forEach(marca => {
      const option = document.createElement("option");
      option.value = marca;
      option.textContent = marca;
      marcasSelect.appendChild(option);
  });

  proveedores.forEach(proveedor => {
      const option = document.createElement("option");
      option.value = proveedor;
      option.textContent = proveedor;
      proveedoresSelect.appendChild(option);
  });
}

// Llamar a la función al cargar la página
document.addEventListener("DOMContentLoaded", function() {
  llenarListas();
});

  
  document.getElementById("busqueda").addEventListener("input", buscarMaterias);
  document.getElementById("btn_buscar").addEventListener("click", buscarMaterias);
  document.addEventListener("DOMContentLoaded", mostrar_materias_primas);
  