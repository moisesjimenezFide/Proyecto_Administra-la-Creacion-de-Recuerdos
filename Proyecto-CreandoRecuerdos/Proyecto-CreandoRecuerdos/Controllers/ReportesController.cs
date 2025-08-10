using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.ViewModels;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    public class ReportesController : Controller
    {
        private bool UsuarioEsAdmin()
        {
            return Session["Rol"] != null && (int)Session["Rol"] == 1;
        }

        public ActionResult Index()
        {
            if (!UsuarioEsAdmin())
                return RedirectToAction("registro_usuarios", "Registro_Usuarios");

            return View();
        }

        public ActionResult HistorialVentas(string fechaInicio, string fechaFin)
        {
            if (!UsuarioEsAdmin())
                return RedirectToAction("registro_usuarios", "Registro_Usuarios");

            DateTime fechaInicioFiltrada = DateTime.MinValue;
            DateTime fechaFinFiltrada = DateTime.MaxValue;

            if (DateTime.TryParseExact(fechaInicio, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var fInicio))
                fechaInicioFiltrada = fInicio.Date;

            if (DateTime.TryParseExact(fechaFin, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var fFin))
                fechaFinFiltrada = fFin.Date.AddDays(1).AddTicks(-1);

            ViewBag.FechaInicio = fInicio.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fFin.ToString("yyyy-MM-dd");

            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var ventas = (from v in db.tabla_ventas
                              join u in db.tabla_usuarios on v.id_usuario equals u.id_usuario
                              join c in db.tabla_clientes on v.id_cliente equals c.id_cliente into clienteJoin
                              from cj in clienteJoin.DefaultIfEmpty()
                              where v.fecha >= fechaInicioFiltrada && v.fecha <= fechaFinFiltrada
                              select new HistorialVentasViewModel
                              {
                                  IdVenta = v.id_venta,
                                  Fecha = v.fecha ?? DateTime.MinValue,
                                  Total = v.total,
                                  Cliente = cj != null ? cj.nombre + " " + cj.apellido : "Consumidor Final",
                                  Usuario = u.nombre
                              }).ToList();

                return View("HistorialVentas", ventas);
            }
        }



        public ActionResult ExportarHistorialVentas(string formato, DateTime? fechaInicio, DateTime? fechaFin)
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var fechaInicioSinHora = fechaInicio?.Date;
                var fechaFinSinHora = fechaFin?.Date.AddDays(1).AddTicks(-1);

                var ventas = (from v in db.tabla_ventas
                              join u in db.tabla_usuarios on v.id_usuario equals u.id_usuario
                              join c in db.tabla_clientes on v.id_cliente equals c.id_cliente into clienteJoin
                              from cj in clienteJoin.DefaultIfEmpty()
                              where (!fechaInicio.HasValue || v.fecha >= fechaInicioSinHora) &&
                                    (!fechaFin.HasValue || v.fecha <= fechaFinSinHora)
                              select new HistorialVentasViewModel
                              {
                                  IdVenta = v.id_venta,
                                  Fecha = v.fecha ?? DateTime.MinValue,
                                  Total = v.total,
                                  Cliente = cj != null ? cj.nombre + " " + cj.apellido : "Consumidor Final",
                                  Usuario = u.nombre
                              }).ToList();

                if (formato == "PDF")
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        Document doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, stream).CloseStream = false;
                        doc.Open();

                        doc.Add(new Paragraph("Historial de Ventas"));
                        doc.Add(new Paragraph(" "));

                        PdfPTable table = new PdfPTable(5);
                        table.AddCell("ID Venta");
                        table.AddCell("Fecha");
                        table.AddCell("Total");
                        table.AddCell("Cliente");
                        table.AddCell("Vendedor");

                        foreach (var venta in ventas)
                        {
                            table.AddCell(venta.IdVenta.ToString());
                            table.AddCell(venta.Fecha.ToString("dd/MM/yyyy"));
                            table.AddCell(venta.Total.ToString("C"));
                            table.AddCell(venta.Cliente);
                            table.AddCell(venta.Usuario);
                        }

                        doc.Add(table);
                        doc.Close();

                        byte[] pdfBytes = stream.ToArray();
                        return File(pdfBytes, "application/pdf", "HistorialVentas.pdf");
                    }
                }
                else if (formato == "EXCEL")
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("HistorialVentas");
                        worksheet.Cell(1, 1).Value = "ID Venta";
                        worksheet.Cell(1, 2).Value = "Fecha";
                        worksheet.Cell(1, 3).Value = "Total";
                        worksheet.Cell(1, 4).Value = "Cliente";
                        worksheet.Cell(1, 5).Value = "Vendedor";

                        for (int i = 0; i < ventas.Count; i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = ventas[i].IdVenta;
                            worksheet.Cell(i + 2, 2).Value = ventas[i].Fecha.ToString("dd/MM/yyyy");
                            worksheet.Cell(i + 2, 3).Value = ventas[i].Total;
                            worksheet.Cell(i + 2, 4).Value = ventas[i].Cliente;
                            worksheet.Cell(i + 2, 5).Value = ventas[i].Usuario;
                        }

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            byte[] content = stream.ToArray();
                            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "HistorialVentas.xlsx");
                        }
                    }
                }

                return new HttpStatusCodeResult(400, "Formato no soportado");
            }
        }

        public ActionResult VentasMensuales(int? anio)
        {
            if (!UsuarioEsAdmin())
                return RedirectToAction("registro_usuarios", "Registro_Usuarios");

            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var resumen = db.tabla_ventas
                    .Where(v => !anio.HasValue || v.fecha.Value.Year == anio.Value)
                    .GroupBy(v => new { v.fecha.Value.Year, v.fecha.Value.Month })
                    .Select(g => new VentasMensualesViewModel
                    {
                        Anio = g.Key.Year,
                        Mes = g.Key.Month,
                        Total = g.Sum(v => v.total)
                    })
                    .OrderByDescending(g => g.Anio)
                    .ThenByDescending(g => g.Mes)
                    .ToList();

                return View("VentasMensuales", resumen);
            }
        }

        public ActionResult EmpleadosDisponibles()
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var empleados = db.tabla_usuarios
                    .Where(u => u.id_rol == 2 && u.activo == true)
                    .Select(u => new EmpleadosDisponiblesViewModel
                    {
                        IdUsuario = u.id_usuario,
                        Usuario = u.nombre,
                        Correo = u.correo,
                        NombreCompleto = u.nombre,
                        Rol = u.tabla_roles.nombre,
                        Estado = u.activo == true ? "Activo" : "Inactivo"
                    })
                    .ToList();

                return View(empleados);
            }
        }

        public ActionResult ProductosDisponibles()
        {
            if (!UsuarioEsAdmin())
                return RedirectToAction("registro_usuarios", "Registro_Usuarios");

            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var productos = db.tabla_productos
                    .Select(p => new ProductosDisponiblesViewModel
                    {
                        IdProducto = p.id_producto,
                        Nombre = p.nombre,
                        Descripcion = p.descripcion,
                        PrecioUnidad = p.precio_por_unidad
                    }).ToList();

                return View("ProductosDisponibles", productos);
            }
        }

        public ActionResult CostosOperativos()
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var viewModel = new CostosOperativosMensualesViewModel
                {
                    PromedioCostosRecetas = db.tabla_costos_recetas.Any() ? db.tabla_costos_recetas.Average(r => (decimal?)r.costo_por_porcion) ?? 0 : 0,
                    PromedioCostosEmpaques = db.tabla_empaques_decoraciones.Any() ? db.tabla_empaques_decoraciones.Average(e => (decimal?)e.costo) ?? 0 : 0,
                    PromedioCostosImplementos = db.tabla_implementos.Any() ? db.tabla_implementos.Average(i => (decimal?)i.costo) ?? 0 : 0,
                    PromedioCostosSuministros = db.tabla_suministros.Any() ? db.tabla_suministros.Average(s => (decimal?)s.costo) ?? 0 : 0
                };

                return View(viewModel);
            }
        }

        public ActionResult ExportarEmpleados(string formato)
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var empleados = db.tabla_usuarios
                    .Where(u => u.id_rol == 2 && u.activo == true)
                    .Select(u => new EmpleadosDisponiblesViewModel
                    {
                        Usuario = u.nombre,
                        NombreCompleto = u.nombre,
                        Rol = u.tabla_roles.nombre,
                        Estado = u.activo == true ? "Activo" : "Inactivo"
                    })
                    .ToList();

                if (formato == "PDF")
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        Document doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, stream).CloseStream = false;
                        doc.Open();

                        doc.Add(new Paragraph("Empleados Disponibles"));
                        doc.Add(new Paragraph(" "));

                        PdfPTable table = new PdfPTable(4);
                        table.AddCell("Nombre");
                        table.AddCell("Correo");
                        table.AddCell("Rol");
                        table.AddCell("Estado");

                        foreach (var e in empleados)
                        {
                            table.AddCell(e.NombreCompleto);
                            table.AddCell(e.Usuario);
                            table.AddCell(e.Rol);
                            table.AddCell(e.Estado);
                        }

                        doc.Add(table);
                        doc.Close();

                        byte[] pdfBytes = stream.ToArray();
                        return File(pdfBytes, "application/pdf", "EmpleadosDisponibles.pdf");
                    }
                }
                else if (formato == "EXCEL")
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("EmpleadosDisponibles");
                        worksheet.Cell(1, 1).Value = "Nombre";
                        worksheet.Cell(1, 2).Value = "Correo";
                        worksheet.Cell(1, 3).Value = "Rol";
                        worksheet.Cell(1, 4).Value = "Estado";

                        for (int i = 0; i < empleados.Count; i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = empleados[i].NombreCompleto;
                            worksheet.Cell(i + 2, 2).Value = empleados[i].Usuario;
                            worksheet.Cell(i + 2, 3).Value = empleados[i].Rol;
                            worksheet.Cell(i + 2, 4).Value = empleados[i].Estado;
                        }

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            byte[] content = stream.ToArray();
                            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmpleadosDisponibles.xlsx");
                        }
                    }
                }

                return new HttpStatusCodeResult(400, "Formato no soportado");
            }
        }
        public ActionResult ExportarProductos(string formato)
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var productos = db.tabla_productos
                    .Select(p => new ProductosDisponiblesViewModel
                    {
                        Nombre = p.nombre,
                        Descripcion = p.descripcion,
                        PrecioUnidad = p.precio_por_unidad
                    })
                    .ToList();

                if (formato == "PDF")
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        Document doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, stream).CloseStream = false;
                        doc.Open();

                        doc.Add(new Paragraph("Productos del Menú"));
                        doc.Add(new Paragraph(" "));

                        PdfPTable table = new PdfPTable(3);
                        table.AddCell("Nombre");
                        table.AddCell("Descripción");
                        table.AddCell("Precio por Unidad");

                        foreach (var p in productos)
                        {
                            table.AddCell(p.Nombre);
                            table.AddCell(p.Descripcion);
                            table.AddCell(p.PrecioUnidad.ToString("C"));
                        }

                        doc.Add(table);
                        doc.Close();

                        byte[] pdfBytes = stream.ToArray();
                        return File(pdfBytes, "application/pdf", "ProductosDisponibles.pdf");
                    }
                }
                else if (formato == "EXCEL")
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("ProductosDisponibles");
                        worksheet.Cell(1, 1).Value = "Nombre";
                        worksheet.Cell(1, 2).Value = "Descripción";
                        worksheet.Cell(1, 3).Value = "Precio por Unidad";

                        for (int i = 0; i < productos.Count; i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = productos[i].Nombre;
                            worksheet.Cell(i + 2, 2).Value = productos[i].Descripcion;
                            worksheet.Cell(i + 2, 3).Value = productos[i].PrecioUnidad;
                        }

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            byte[] content = stream.ToArray();
                            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProductosDisponibles.xlsx");
                        }
                    }
                }

                return new HttpStatusCodeResult(400, "Formato no soportado");
            }
        }

        public ActionResult ExportarCostosOperativos(string formato)
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var viewModel = new CostosOperativosMensualesViewModel
                {
                    PromedioCostosRecetas = db.tabla_costos_recetas.Any() ? db.tabla_costos_recetas.Average(r => (decimal?)r.costo_por_porcion) ?? 0 : 0,
                    PromedioCostosEmpaques = db.tabla_empaques_decoraciones.Any() ? db.tabla_empaques_decoraciones.Average(e => (decimal?)e.costo) ?? 0 : 0,
                    PromedioCostosImplementos = db.tabla_implementos.Any() ? db.tabla_implementos.Average(i => (decimal?)i.costo) ?? 0 : 0,
                    PromedioCostosSuministros = db.tabla_suministros.Any() ? db.tabla_suministros.Average(s => (decimal?)s.costo) ?? 0 : 0
                };

                if (formato == "PDF")
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        Document doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, stream).CloseStream = false;
                        doc.Open();

                        doc.Add(new Paragraph("Costos Operativos Promedios"));
                        doc.Add(new Paragraph(" "));

                        PdfPTable table = new PdfPTable(2);
                        table.AddCell("Categoría");
                        table.AddCell("Costo Promedio");

                        table.AddCell("Recetas");
                        table.AddCell(viewModel.PromedioCostosRecetas.ToString("C"));

                        table.AddCell("Empaques");
                        table.AddCell(viewModel.PromedioCostosEmpaques.ToString("C"));

                        table.AddCell("Implementos");
                        table.AddCell(viewModel.PromedioCostosImplementos.ToString("C"));

                        table.AddCell("Suministros");
                        table.AddCell(viewModel.PromedioCostosSuministros.ToString("C"));

                        doc.Add(table);
                        doc.Close();

                        byte[] pdfBytes = stream.ToArray();
                        return File(pdfBytes, "application/pdf", "CostosOperativos.pdf");
                    }
                }
                else if (formato == "EXCEL")
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("CostosOperativos");
                        worksheet.Cell(1, 1).Value = "Categoría";
                        worksheet.Cell(1, 2).Value = "Costo Promedio";

                        worksheet.Cell(2, 1).Value = "Recetas";
                        worksheet.Cell(2, 2).Value = viewModel.PromedioCostosRecetas;

                        worksheet.Cell(3, 1).Value = "Empaques";
                        worksheet.Cell(3, 2).Value = viewModel.PromedioCostosEmpaques;

                        worksheet.Cell(4, 1).Value = "Implementos";
                        worksheet.Cell(4, 2).Value = viewModel.PromedioCostosImplementos;

                        worksheet.Cell(5, 1).Value = "Suministros";
                        worksheet.Cell(5, 2).Value = viewModel.PromedioCostosSuministros;

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            byte[] content = stream.ToArray();
                            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CostosOperativos.xlsx");
                        }
                    }
                }

                return new HttpStatusCodeResult(400, "Formato no soportado");
            }
        }

        public ActionResult ExportarVentasMensuales(string formato, int? anio)
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var resumen = db.tabla_ventas
                    .Where(v => !anio.HasValue || v.fecha.Value.Year == anio.Value)
                    .GroupBy(v => new { v.fecha.Value.Year, v.fecha.Value.Month })
                    .Select(g => new VentasMensualesViewModel
                    {
                        Anio = g.Key.Year,
                        Mes = g.Key.Month,
                        Total = g.Sum(v => v.total)
                    })
                    .OrderByDescending(g => g.Anio)
                    .ThenByDescending(g => g.Mes)
                    .ToList();

                if (formato == "PDF")
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        Document doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, stream).CloseStream = false;
                        doc.Open();

                        doc.Add(new Paragraph("Ventas Mensuales"));
                        doc.Add(new Paragraph(" "));

                        PdfPTable table = new PdfPTable(3);
                        table.AddCell("Año");
                        table.AddCell("Mes");
                        table.AddCell("Total Ventas");

                        foreach (var item in resumen)
                        {
                            table.AddCell(item.Anio.ToString());
                            table.AddCell(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Mes));
                            table.AddCell(item.Total.ToString("C"));
                        }

                        doc.Add(table);
                        doc.Close();

                        byte[] pdfBytes = stream.ToArray();
                        return File(pdfBytes, "application/pdf", "VentasMensuales.pdf");
                    }
                }
                else if (formato == "EXCEL")
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("VentasMensuales");
                        worksheet.Cell(1, 1).Value = "Año";
                        worksheet.Cell(1, 2).Value = "Mes";
                        worksheet.Cell(1, 3).Value = "Total Ventas";

                        for (int i = 0; i < resumen.Count; i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = resumen[i].Anio;
                            worksheet.Cell(i + 2, 2).Value = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(resumen[i].Mes);
                            worksheet.Cell(i + 2, 3).Value = resumen[i].Total;
                        }

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            byte[] content = stream.ToArray();
                            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "VentasMensuales.xlsx");
                        }
                    }
                }

                return new HttpStatusCodeResult(400, "Formato no soportado");
            }
        }

    }
}
