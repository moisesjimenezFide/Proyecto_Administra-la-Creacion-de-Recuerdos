// Datos iniciales de prueba
const recetas = [
    {
      nombre: "Panqueques",
      porcion: 4,
      volumen: 200,
      materias: [
        { materia: "Harina", cantidad: 2, unidad: "taza", costo: 300, total: 600 },
        { materia: "Huevos", cantidad: 2, unidad: "unidad", costo: 120, total: 240 }
      ],
      total_receta: 840,
      costo_porcion: 210
    },
    {
      nombre: "Smoothie de Fresa",
      porcion: 2,
      volumen: 300,
      materias: [
        { materia: "Fresa", cantidad: 150, unidad: "g", costo: 6, total: 900 },
        { materia: "Leche", cantidad: 250, unidad: "ml", costo: 2, total: 500 }
      ],
      total_receta: 1400,
      costo_porcion: 700
    },
    {
      nombre: "Café Latte",
      porcion: 1,
      volumen: 250,
      materias: [
        { materia: "Café", cantidad: 20, unidad: "g", costo: 10, total: 200 },
        { materia: "Leche", cantidad: 200, unidad: "ml", costo: 2, total: 400 }
      ],
      total_receta: 600,
      costo_porcion: 600
    }
  ];
  
  // Renderiza las recetas en la tabla principal
function mostrar_recetas() {
  const tabla = document.getElementById("tabla_recetas");
  tabla.innerHTML = "";

  recetas.forEach((r, i) => {
    const materias = r.materias.map(m =>
      `${m.materia} (${m.cantidad} ${m.unidad}, ₡${m.total.toFixed(2)})`
    ).join("<br>");

    tabla.innerHTML += `
      <tr>
        <td>${r.nombre}</td>
        <td>${r.porcion}</td>
        <td>${r.volumen}</td>
        <td>${materias}</td>
        <td>₡${r.total_receta.toFixed(2)}</td>
        <td>₡${r.costo_porcion.toFixed(2)}</td>
        <td>
          <div class="d-flex justify-content-center gap-2" style="white-space: nowrap;">
            <button class="btn btn-sm btn-danger text-white" onclick="eliminarReceta(${i})">
              <i class="fas fa-trash-alt me-1"></i>Eliminar
            </button>
            <button class="btn btn-sm text-white" style="background-color: #E27AB0;" onclick="editarReceta(${i})">
              <i class="fas fa-edit me-1"></i>Editar
            </button>
          </div>
        </td>
      </tr>
    `;
  });
}

  
  // Rellena el formulario con los datos de la receta seleccionada
  function editarReceta(index) {
    const receta = recetas[index];
  
    document.getElementById("nombre_receta").value = receta.nombre;
    document.getElementById("porcion").value = receta.porcion;
    document.getElementById("volumen").value = receta.volumen;
  
    const tbody = document.getElementById("tbody_materias");
    tbody.innerHTML = "";
  
    receta.materias.forEach(m => {
      const fila = document.createElement("tr");
      fila.innerHTML = `
        <td><input type="text" class="form-control materia" value="${m.materia}" required></td>
        <td><input type="number" class="form-control cantidad" value="${m.cantidad}" required></td>
        <td><input type="text" class="form-control unidad" value="${m.unidad}" required></td>
        <td><input type="number" class="form-control costo" value="${m.costo}" required></td>
        <td class="total-costo">₡${m.total.toFixed(2)}</td>
        <td><button type="button" class="btn btn-danger btn-sm" onclick="eliminarFila(this)"><i class="fas fa-trash-alt me-1"></i>Eliminar</button></td>
      `;
      tbody.appendChild(fila);
    });
  
    agregarFila();
    calcularTotales();
  }
  
  // Elimina la receta seleccionada
  function eliminarReceta(index) {
    if (confirm(`¿Deseás eliminar la receta "${recetas[index].nombre}"?`)) {
      recetas.splice(index, 1);
      mostrar_recetas();
    }
  }
  
  // Calcula el total de la receta y el costo por porción en base a los datos actuales
  function calcularTotales() {
    const filas = document.querySelectorAll("#tbody_materias tr");
    let total = 0;
  
    filas.forEach(fila => {
      const cantidad = parseFloat(fila.querySelector(".cantidad").value) || 0;
      const costo = parseFloat(fila.querySelector(".costo").value) || 0;
      const subtotal = cantidad * costo;
      fila.querySelector(".total-costo").textContent = `₡${subtotal.toFixed(2)}`;
      total += subtotal;
    });
  
    document.getElementById("costo_total_receta").textContent = `₡${total.toFixed(2)}`;
  
    const porcion = parseFloat(document.getElementById("porcion").value) || 1;
    const porcionCosto = total / porcion;
    document.getElementById("costo_por_porcion").textContent = `₡${porcionCosto.toFixed(2)}`;
  }
  
  // Elimina una fila de materia prima
  function eliminarFila(btn) {
    btn.closest("tr").remove();
    calcularTotales();
  }
  
  // Agrega una fila vacía al formulario para ingreso de materia prima
  function agregarFila() {
    const tbody = document.getElementById("tbody_materias");
    const fila = document.createElement("tr");
    fila.innerHTML = `
      <td><input type="text" class="form-control materia" placeholder="Materia prima" required></td>
      <td><input type="number" class="form-control cantidad" placeholder="Cantidad" required></td>
      <td><input type="text" class="form-control unidad" placeholder="Unidad" required></td>
      <td><input type="number" class="form-control costo" placeholder="Costo" required></td>
      <td class="total-costo">₡0.00</td>
      <td><button type="button" class="btn btn-danger btn-sm" onclick="eliminarFila(this)"><i class="fas fa-trash-alt me-1"></i>Eliminar</button></td>
    `;
    tbody.appendChild(fila);
  }
  
  // Limpia el formulario de materias primas
  function limpiarFormulario() {
    document.getElementById("form_recetas").reset();
    document.getElementById("tbody_materias").innerHTML = "";
    agregarFila();
    calcularTotales();
  }
  
  // Guarda la receta actual y la agrega a la tabla inferior
  function guardarReceta(e) {
    e.preventDefault();
  
    const nombre = document.getElementById("nombre_receta").value;
    const porcion = parseFloat(document.getElementById("porcion").value);
    const volumen = parseFloat(document.getElementById("volumen").value);
  
    const filas = document.querySelectorAll("#tbody_materias tr");
    const materias = [];
    let total_receta = 0;
  
    filas.forEach(fila => {
      const materia = fila.querySelector(".materia").value;
      const cantidad = parseFloat(fila.querySelector(".cantidad").value) || 0;
      const unidad = fila.querySelector(".unidad").value;
      const costo = parseFloat(fila.querySelector(".costo").value) || 0;
      const total = cantidad * costo;
      if (materia && unidad) {
        materias.push({ materia, cantidad, unidad, costo, total });
        total_receta += total;
      }
    });
  
    if (!nombre || porcion <= 0 || volumen <= 0 || materias.length === 0) {
      alert("Por favor completá todos los campos antes de guardar.");
      return;
    }
  
    const nueva = {
      nombre,
      porcion,
      volumen,
      materias,
      total_receta,
      costo_porcion: total_receta / porcion
    };
  
    recetas.push(nueva);
    limpiarFormulario();
    mostrar_recetas();
  }
  
  // Eventos principales
  
  document.addEventListener("DOMContentLoaded", () => {
    mostrar_recetas();
    editarReceta(0);
  
    document.getElementById("form_recetas").addEventListener("submit", guardarReceta);
  
    document.querySelectorAll("button[type='reset']").forEach(btn => {
      btn.addEventListener("click", limpiarFormulario);
    });
  });