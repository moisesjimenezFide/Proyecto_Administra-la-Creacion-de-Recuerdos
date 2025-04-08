document.addEventListener("DOMContentLoaded", function () {
    cargarDatos();
    mostrarReporte('ventas'); // Mostrar ventas por defecto
  });
  
  function mostrarReporte(nombre) {
    document.querySelectorAll(".reporte").forEach(r => r.style.display = "none");
    document.getElementById("reporte-" + nombre).style.display = "block";
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