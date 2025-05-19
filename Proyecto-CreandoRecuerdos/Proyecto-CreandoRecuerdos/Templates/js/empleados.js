let empleados = [];
let vacaciones = [];
let horarios = [];
let editando = false;
let indiceEditar = null;

const formEmpleado = document.getElementById("form-empleado");
const formVacaciones = document.getElementById("form-vacaciones");
const formHorarios = document.getElementById("form-horarios");

const tablaEmpleados = document.getElementById("tabla-empleados").querySelector("tbody");
const tablaVacaciones = document.querySelector("#seccion-vacaciones table tbody");
const tablaHorarios = document.querySelector("#seccion-horarios table tbody");

const mensaje = document.getElementById("mensaje");

// ----- EMPLEADOS -----
formEmpleado.addEventListener("submit", function (e) {
  e.preventDefault();

  const nombre = document.getElementById("nombre").value.trim();
  const apellido = document.getElementById("apellido").value.trim();
  const correo = document.getElementById("correo").value.trim().toLowerCase();
  const rol = document.getElementById("rol").value;
  const estado = document.getElementById("estado").value;

  if (!nombre || !apellido || !correo || !rol || !estado) {
    mostrarMensaje("Todos los campos son obligatorios.", "danger");
    return;
  }

  if (!validarEmail(correo)) {
    mostrarMensaje("El correo no tiene un formato válido.", "danger");
    return;
  }

  const yaExiste = empleados.some((emp, i) => emp.correo === correo && i !== indiceEditar);
  if (yaExiste) {
    mostrarMensaje("Ya existe un empleado con ese correo.", "danger");
    return;
  }

  const empleado = { nombre, apellido, correo, rol, estado };

  if (editando) {
    empleados[indiceEditar] = empleado;
    editando = false;
    indiceEditar = null;
    mostrarMensaje("Empleado editado correctamente.", "info");
  } else {
    empleados.push(empleado);
    mostrarMensaje("Empleado registrado con éxito.", "success");
  }

  formEmpleado.reset();
  renderTablaEmpleados();
});

function renderTablaEmpleados() {
  tablaEmpleados.innerHTML = "";
  empleados.forEach((emp, index) => {
    const fila = document.createElement("tr");
    fila.innerHTML = `
      <td>${emp.nombre} ${emp.apellido}</td>
      <td>${emp.correo}</td>
      <td>${emp.rol}</td>
      <td>${emp.estado}</td>
      <td>
        <div class="d-flex justify-content-center gap-2">
          <!-- Botón Eliminar con tamaño pequeño -->
          <button class="btn btn-sm btn-danger px-2 py-1" onclick="eliminarEmpleado(${index})">
            <i class="fas fa-trash-alt me-1"></i>Eliminar
          </button>
          <!-- Botón Editar con tamaño pequeño -->
          <button class="btn btn-sm text-white px-2 py-1" style="background-color: #E27AB0;" onclick="editarEmpleado(${index})">
            <i class="fas fa-edit me-1"></i>Editar
          </button>
        </div>
      </td>
    `;
    tablaEmpleados.appendChild(fila);
  });
}

window.editarEmpleado = function (index) {
  const emp = empleados[index];
  document.getElementById("nombre").value = emp.nombre;
  document.getElementById("apellido").value = emp.apellido;
  document.getElementById("correo").value = emp.correo;
  document.getElementById("rol").value = emp.rol;
  document.getElementById("estado").value = emp.estado;

  editando = true;
  indiceEditar = index;
};

window.eliminarEmpleado = function (index) {
  Swal.fire({
    title: `¿Deseás eliminar a ${empleados[index].nombre}?`,
    icon: "warning",
    showCancelButton: true,
    confirmButtonText: "Sí, eliminar",
    cancelButtonText: "Cancelar"
  }).then(result => {
    if (result.isConfirmed) {
      empleados.splice(index, 1);
      renderTablaEmpleados();
      mostrarMensaje("Empleado eliminado.", "warning");
    }
  });
};

// ----- VACACIONES -----
formVacaciones.addEventListener("submit", function (e) {
  e.preventDefault();

  const inicioVacacion = document.getElementById("inicioVacacion").value.trim();
  const finVacacion = document.getElementById("finVacacion").value.trim();
  const motivoVacacion = document.getElementById("motivoVacacion").value.trim();

  if (!inicioVacacion || !finVacacion || !motivoVacacion) {
    mostrarMensaje("Todos los campos son obligatorios.", "danger");
    return;
  }

  const solicitud = { inicioVacacion, finVacacion, motivoVacacion, estado: "Pendiente" };

  vacaciones.push(solicitud);
  mostrarMensaje("Solicitud de vacaciones enviada.", "success");
  formVacaciones.reset();
  renderTablaVacaciones();
});

function renderTablaVacaciones() {
  tablaVacaciones.innerHTML = "";
  vacaciones.forEach((solicitud, index) => {
    const fila = document.createElement("tr");
    fila.innerHTML = `
      <td>${solicitud.inicioVacacion} - ${solicitud.finVacacion}</td>
      <td>${solicitud.motivoVacacion}</td>
      <td>${solicitud.estado}</td>
      <td>
        <div class="d-flex justify-content-center gap-2" style="flex-wrap: nowrap; white-space: nowrap;">
          <button class="btn btn-sm btn-danger px-2 py-1" onclick="eliminarVacacion(${index})">
            <i class="fas fa-trash-alt me-1"></i>Eliminar
          </button>
          <button class="btn btn-sm text-white px-2 py-1" style="background-color: #E27AB0;" onclick="editarVacacion(${index})">
            <i class="fas fa-edit me-1"></i>Editar
          </button>
        </div>
      </td>
    `;
    tablaVacaciones.appendChild(fila);
  });
}


window.editarVacacion = function (index) {
  const solicitud = vacaciones[index];
  document.getElementById("inicioVacacion").value = solicitud.inicioVacacion;
  document.getElementById("finVacacion").value = solicitud.finVacacion;
  document.getElementById("motivoVacacion").value = solicitud.motivoVacacion;

  vacaciones.splice(index, 1);
  renderTablaVacaciones();
};

window.eliminarVacacion = function (index) {
  Swal.fire({
    title: `¿Deseás eliminar esta solicitud?`,
    icon: "warning",
    showCancelButton: true,
    confirmButtonText: "Sí, eliminar",
    cancelButtonText: "Cancelar"
  }).then(result => {
    if (result.isConfirmed) {
      vacaciones.splice(index, 1);
      renderTablaVacaciones();
      mostrarMensaje("Solicitud eliminada.", "warning");
    }
  });
};

// ----- HORARIOS -----
formHorarios.addEventListener("submit", function (e) {
  e.preventDefault();

  const nombreHorario = document.getElementById("nombreHorario").value.trim();
  const entrada = document.getElementById("entrada").value.trim();
  const salida = document.getElementById("salida").value.trim();

  if (!nombreHorario || !entrada || !salida) {
    mostrarMensaje("Todos los campos son obligatorios.", "danger");
    return;
  }

  const horario = { nombreHorario, entrada, salida };

  horarios.push(horario);
  mostrarMensaje("Horario registrado con éxito.", "success");
  formHorarios.reset();
  renderTablaHorarios();
});

function renderTablaEmpleados() {
  tablaEmpleados.innerHTML = "";
  empleados.forEach((emp, index) => {
    const fila = document.createElement("tr");
    fila.innerHTML = `
      <td>${emp.nombre} ${emp.apellido}</td>
      <td>${emp.correo}</td>
      <td>${emp.rol}</td>
      <td>${emp.estado}</td>
      <td>
        <div class="d-flex justify-content-center gap-2" style="flex-wrap: nowrap; white-space: nowrap;">
          <!-- Botón Eliminar con tamaño pequeño -->
          <button class="btn btn-sm btn-danger px-2 py-1" onclick="eliminarEmpleado(${index})">
            <i class="fas fa-trash-alt me-1"></i>Eliminar
          </button>
          <!-- Botón Editar con tamaño pequeño -->
          <button class="btn btn-sm text-white px-2 py-1" style="background-color: #E27AB0;" onclick="editarEmpleado(${index})">
            <i class="fas fa-edit me-1"></i>Editar
          </button>
        </div>
      </td>
    `;
    tablaEmpleados.appendChild(fila);
  });
}

window.editarHorario = function (index) {
  const horario = horarios[index];
  document.getElementById("nombreHorario").value = horario.nombreHorario;
  document.getElementById("entrada").value = horario.entrada;
  document.getElementById("salida").value = horario.salida;

  horarios.splice(index, 1);
  renderTablaHorarios();
};

window.eliminarHorario = function (index) {
  Swal.fire({
    title: `¿Eliminar horario de ${horarios[index].nombreHorario}?`,
    icon: "warning",
    showCancelButton: true,
    confirmButtonText: "Sí, eliminar",
    cancelButtonText: "Cancelar"
  }).then(result => {
    if (result.isConfirmed) {
      horarios.splice(index, 1);
      renderTablaHorarios();
      mostrarMensaje("Horario eliminado.", "warning");
    }
  });
};

// ----- UTILITARIOS -----
function validarEmail(email) {
  const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return regex.test(email);
}

function mostrarMensaje(texto, tipo = "success") {
  mensaje.textContent = texto;
  mensaje.className = `alert alert-${tipo} mt-3`;
  mensaje.style.display = "block";
  setTimeout(() => (mensaje.style.display = "none"), 3000);
}

function cargarDatosEjemplo() {
  empleados = [
    { nombre: "Juan", apellido: "Pérez", correo: "juan@example.com", rol: "Gerente", estado: "Activo" },
    { nombre: "Ana", apellido: "López", correo: "ana@example.com", rol: "Cajero", estado: "Inactivo" }
  ];
  renderTablaEmpleados();

  vacaciones = [
    { inicioVacacion: "2023-05-01", finVacacion: "2023-05-10", motivoVacacion: "Vacaciones anuales", estado: "Aprobada" },
    { inicioVacacion: "2023-06-01", finVacacion: "2023-06-05", motivoVacacion: "Permiso personal", estado: "Pendiente" }
  ];
  renderTablaVacaciones();

  horarios = [
    { nombreHorario: "Juan Pérez", entrada: "08:00", salida: "17:00" },
    { nombreHorario: "Ana López", entrada: "09:00", salida: "18:00" }
  ];
  renderTablaHorarios();
}

document.addEventListener("DOMContentLoaded", cargarDatosEjemplo);
