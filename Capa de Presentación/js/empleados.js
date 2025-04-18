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

function validarEmail(email) {
  const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return regex.test(email);
}

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
        <button class="btn btn-sm btn-outline-info me-1" onclick="editarEmpleado(${index})">Editar</button>
        <button class="btn btn-sm btn-outline-danger" onclick="eliminarEmpleado(${index})">Eliminar</button>
      </td>
    `;
    tablaEmpleados.appendChild(fila);
  });
}

function renderTablaVacaciones() {
  tablaVacaciones.innerHTML = "";
  vacaciones.forEach((solicitud) => {
    const fila = document.createElement("tr");
    fila.innerHTML = `
      <td>${solicitud.inicioVacacion} - ${solicitud.finVacacion}</td>
      <td>${solicitud.motivoVacacion}</td>
      <td>${solicitud.estado}</td>
    `;
    tablaVacaciones.appendChild(fila);
  });
}

function renderTablaHorarios() {
  tablaHorarios.innerHTML = "";
  horarios.forEach((horario) => {
    const fila = document.createElement("tr");
    fila.innerHTML = `
      <td>${horario.nombreHorario}</td>
      <td>${horario.entrada}</td>
      <td>${horario.salida}</td>
    `;
    tablaHorarios.appendChild(fila);
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
  if (confirm("¿Deseás eliminar este empleado?")) {
    empleados.splice(index, 1);
    renderTablaEmpleados();
    mostrarMensaje("Empleado eliminado.", "warning");
  }
};

function mostrarMensaje(texto, tipo = "success") {
  mensaje.textContent = texto;
  mensaje.className = `alert alert-${tipo} mt-3`;
  mensaje.style.display = "block";

  setTimeout(() => {
    mensaje.style.display = "none";
  }, 3000);
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

document.addEventListener("DOMContentLoaded", function () {
  cargarDatosEjemplo();
});
