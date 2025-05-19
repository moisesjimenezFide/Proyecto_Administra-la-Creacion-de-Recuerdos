document.addEventListener("DOMContentLoaded", function () {
  cargarDatos();
  mostrarReporte('ventas'); // Mostrar ventas por defecto

  // Evento para el botón de descargar
  document.getElementById("btn-descargar").addEventListener("click", descargarExcel);
});

function mostrarReporte(nombre) {
  document.querySelectorAll(".reporte").forEach(r => r.style.display = "none");
  document.getElementById("reporte-" + nombre).style.display = "block";
  // Actualizar ID de reporte activo
  document.getElementById("btn-descargar").setAttribute("data-tabla", "tabla-" + nombre);
}

function cargarDatos() {
  const ventas = [
    { mes: "Enero", total: 1245000 },
    { mes: "Febrero", total: 1100000 },
    { mes: "Marzo", total: 1350000 }
  ];

  const empleados = [
    { mes: "Enero", disponibles: 1 },
    { mes: "Febrero", disponibles: 2 },
    { mes: "Marzo", disponibles: 1 }
  ];

  const productos = [
    { mes: "Enero", cantidad: 40 },
    { mes: "Febrero", cantidad: 35 },
    { mes: "Marzo", cantidad: 38 }
  ];

  const costos = [
    { mes: "Enero", promedio: 3200000 },
    { mes: "Febrero", promedio: 3000000 },
    { mes: "Marzo", promedio: 3100000 }
  ];

  llenarTabla("tabla-ventas", ventas, ["mes", "total"], true);
  llenarTabla("tabla-empleados", empleados, ["mes", "disponibles"]);
  llenarTabla("tabla-productos", productos, ["mes", "cantidad"]);
  llenarTabla("tabla-costos", costos, ["mes", "promedio"], true);
}

function llenarTabla(id, datos, campos, esMoneda = false) {
  const tbody = document.getElementById(id);
  tbody.innerHTML = "";

  datos.forEach(item => {
    const fila = document.createElement("tr");
    campos.forEach(campo => {
      const celda = document.createElement("td");
      const valor = item[campo];

      if (typeof valor === "number" && esMoneda) {
        celda.textContent = `₡${valor.toLocaleString()}`;
      } else {
        celda.textContent = valor;
      }

      fila.appendChild(celda);
    });
    tbody.appendChild(fila);
  });
}

function descargarExcel() {
  const tablaId = document.getElementById("btn-descargar").getAttribute("data-tabla");
  const tabla = document.getElementById(tablaId);

  if (!tabla) return;

  // Clonamos toda la tabla (no solo tbody)
  const tablaCompleta = tabla.closest("table").outerHTML;

  // Preparamos el HTML completo
  const contenido = `
    <html xmlns:o="urn:schemas-microsoft-com:office:office"
          xmlns:x="urn:schemas-microsoft-com:office:excel"
          xmlns="http://www.w3.org/TR/REC-html40">
    <head><meta charset="UTF-8"></head>
    <body>${tablaCompleta}</body>
    </html>
  `;

  // BOM para soportar UTF-8 en Excel
  const blob = new Blob(["\uFEFF", contenido], { type: "application/vnd.ms-excel" });
  const url = URL.createObjectURL(blob);
  const enlace = document.createElement("a");
  enlace.href = url;
  enlace.download = `${tablaId}.xls`;
  document.body.appendChild(enlace);
  enlace.click();
  document.body.removeChild(enlace);
  URL.revokeObjectURL(url);
}

