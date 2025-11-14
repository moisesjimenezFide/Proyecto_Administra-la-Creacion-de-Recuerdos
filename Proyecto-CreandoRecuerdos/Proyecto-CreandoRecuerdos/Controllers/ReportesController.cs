using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Filters;
using Proyecto_CreandoRecuerdos.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;


namespace Proyecto_CreandoRecuerdos.Controllers
{
    // Evitar el almacenamiento en caché de las vistas
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]

    [RolAuthorize("1")]

    public class ReportesController : Controller
    {
        private bool UsuarioEsAdmin()
        {
            return Session["Rol"] != null && Convert.ToInt32(Session["Rol"]) == 1;
        }

        public ActionResult ReportesIndex()
        {
            if (!UsuarioEsAdmin())
                return RedirectToAction("iniciar_sesion", "Registro_Usuarios");

            return View("ReportesIndex");
        }

        public ActionResult HistorialVentas(string fechaInicio, string fechaFin)
        {
            if (!UsuarioEsAdmin())
                return RedirectToAction("iniciar_sesion", "Registro_Usuarios");

            DateTime? inicio = null;
            DateTime? fin = null;

            var formatos = new[] { "dd-MM-yyyy", "yyyy-MM-dd" };
            var cultura = System.Globalization.CultureInfo.InvariantCulture;

            if (!string.IsNullOrWhiteSpace(fechaInicio) &&
                DateTime.TryParseExact(fechaInicio, formatos, cultura, System.Globalization.DateTimeStyles.None, out var fInicio))
            {
                inicio = fInicio.Date;
                ViewBag.FechaInicio = fInicio.ToString("dd-MM-yyyy");
            }
            else
            {
                ViewBag.FechaInicio = "";
            }

            if (!string.IsNullOrWhiteSpace(fechaFin) &&
                DateTime.TryParseExact(fechaFin, formatos, cultura, System.Globalization.DateTimeStyles.None, out var fFin))
            {
                fin = fFin.Date.AddDays(1).AddTicks(-1);
                ViewBag.FechaFin = fFin.ToString("dd-MM-yyyy");
            }
            else
            {
                ViewBag.FechaFin = "";
            }

            if (!inicio.HasValue || !fin.HasValue)
                return View("HistorialVentas", new List<HistorialVentasViewModel>());

            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var q = from v in db.tabla_ventas
                        join u in db.tabla_usuarios on v.id_usuario equals u.id_usuario into usuarioJoin
                        from uj in usuarioJoin.DefaultIfEmpty()
                        join c in db.tabla_clientes on v.id_cliente equals c.id_cliente into clienteJoin
                        from cj in clienteJoin.DefaultIfEmpty()
                        where v.fecha >= inicio && v.fecha <= fin
                        orderby v.fecha ascending
                        select new HistorialVentasViewModel
                        {
                            IdVenta = v.id_venta,
                            Fecha = v.fecha ?? DateTime.MinValue,
                            Total = v.total,
                            Cliente = cj != null ? (cj.nombre + " " + cj.apellido) : "Consumidor Final",
                            Usuario = uj != null ? uj.nombre : "Sin usuario"
                        };

                var lista = q.ToList();

                foreach (var item in lista)
                {
                    if (item.Fecha != DateTime.MinValue)
                        item.Fecha = DateTime.ParseExact(item.Fecha.ToString("dd-MM-yyyy"), "dd-MM-yyyy", cultura);
                }

                return View("HistorialVentas", lista);
            }
        }


        public ActionResult ExportarHistorialVentas(string formato, DateTime? fechaInicio, DateTime? fechaFin)
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                DateTime? inicio = null;
                DateTime? fin = null;

                if (fechaInicio.HasValue && fechaInicio.Value.Year > 1)
                    inicio = fechaInicio.Value.Date;

                if (fechaFin.HasValue && fechaFin.Value.Year > 1)
                    fin = fechaFin.Value.Date.AddDays(1).AddTicks(-1);

                var q =
                    from v in db.tabla_ventas
                    join u in db.tabla_usuarios on v.id_usuario equals u.id_usuario into usuarioJoin
                    from uj in usuarioJoin.DefaultIfEmpty()
                    join c in db.tabla_clientes on v.id_cliente equals c.id_cliente into clienteJoin
                    from cj in clienteJoin.DefaultIfEmpty()
                    select new
                    {
                        v.id_venta,
                        v.fecha,
                        v.total,
                        cliente = cj != null ? (cj.nombre + " " + cj.apellido) : "Consumidor Final",
                        usuario = uj != null ? uj.nombre : "Sin usuario"
                    };

                if (inicio.HasValue) q = q.Where(x => x.fecha >= inicio.Value);
                if (fin.HasValue) q = q.Where(x => x.fecha <= fin.Value);

                q = q.OrderByDescending(x => x.fecha);

                var ventas = q.Select(x => new HistorialVentasViewModel
                {
                    IdVenta = x.id_venta,
                    Fecha = x.fecha ?? DateTime.MinValue,
                    Total = x.total,
                    Cliente = x.cliente,
                    Usuario = x.usuario
                }).ToList();

                if (string.Equals(formato, "PDF", StringComparison.OrdinalIgnoreCase))
                {
                    using (var stream = new MemoryStream())
                    {
                        var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
                        PdfWriter.GetInstance(doc, stream).CloseStream = false;
                        doc.Open();

                        doc.Add(new Paragraph("Historial de Ventas"));
                        doc.Add(new Paragraph(" "));

                        var table = new PdfPTable(5);
                        table.AddCell("ID Venta");
                        table.AddCell("Fecha");
                        table.AddCell("Total");
                        table.AddCell("Cliente");
                        table.AddCell("Vendedor");

                        // --- SOLO CURRENCY (CR) ---
                        var cr = new System.Globalization.CultureInfo("es-CR");
                        cr.NumberFormat.CurrencySymbol = "₡";
                        cr.NumberFormat.CurrencyPositivePattern = 0; // símbolo antes del número

                        foreach (var v in ventas)
                        {
                            table.AddCell(v.IdVenta.ToString());
                            table.AddCell(v.Fecha.ToString("dd/MM/yyyy"));
                            table.AddCell(v.Total.ToString("C", cr)); // ₡ y formato CR
                            table.AddCell(v.Cliente);
                            table.AddCell(v.Usuario);
                        }

                        doc.Add(table);
                        doc.Close();

                        var pdfBytes = stream.ToArray();
                        return File(pdfBytes, "application/pdf", "HistorialVentas.pdf");
                    }
                }
                else if (string.Equals(formato, "EXCEL", StringComparison.OrdinalIgnoreCase))
                {
                    using (var workbook = new ClosedXML.Excel.XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("HistorialVentas");
                        ws.Cell(1, 1).Value = "ID Venta";
                        ws.Cell(1, 2).Value = "Fecha";
                        ws.Cell(1, 3).Value = "Total";
                        ws.Cell(1, 4).Value = "Cliente";
                        ws.Cell(1, 5).Value = "Vendedor";

                        for (int i = 0; i < ventas.Count; i++)
                        {
                            ws.Cell(i + 2, 1).Value = ventas[i].IdVenta;
                            ws.Cell(i + 2, 2).Value = ventas[i].Fecha.ToString("dd/MM/yyyy");
                            ws.Cell(i + 2, 3).Value = ventas[i].Total;        // número
                            ws.Cell(i + 2, 4).Value = ventas[i].Cliente;
                            ws.Cell(i + 2, 5).Value = ventas[i].Usuario;
                        }

                        // --- SOLO CURRENCY (CR) ---
                        ws.Column(3).Style.NumberFormat.Format = "[$₡-es-CR] #,##0.00";

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var content = stream.ToArray();
                            return File(
                                content,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "HistorialVentas.xlsx"
                            );
                        }
                    }
                }

                return new HttpStatusCodeResult(400, "Formato no soportado");
            }
        }

        public ActionResult VentasMensuales(int? anio)
        {
            if (!UsuarioEsAdmin())
                return RedirectToAction("iniciar_sesion", "Registro_Usuarios");

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

            if (!UsuarioEsAdmin())
                return RedirectToAction("iniciar_sesion", "Registro_Usuarios");

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
                return RedirectToAction("iniciar_sesion", "Registro_Usuarios");

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

        [HttpGet]
        public ActionResult VentasPorDia(string fechaInicio, string fechaFin)
        {
            if (!UsuarioEsAdmin()) return new HttpStatusCodeResult(401);

            DateTime? inicio = null;
            DateTime? fin = null;
            string formato = "dd-MM-yyyy";

            if (!string.IsNullOrWhiteSpace(fechaInicio) &&
                DateTime.TryParseExact(fechaInicio, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fInicio))
                inicio = fInicio.Date;

            if (!string.IsNullOrWhiteSpace(fechaFin) &&
                DateTime.TryParseExact(fechaFin, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fFin))
                fin = fFin.Date.AddDays(1).AddTicks(-1);

            if (!inicio.HasValue || !fin.HasValue)
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);

            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var rows = (from v in db.tabla_ventas
                            where v.fecha >= inicio && v.fecha <= fin
                            group v by DbFunctions.TruncateTime(v.fecha) into g
                            orderby g.Key
                            select new
                            {
                                d = g.Key,
                                total = g.Sum(x => (decimal?)x.total) ?? 0m
                            }).ToList();

                var data = rows.Select(x => new
                {
                    label = x.d.HasValue ? x.d.Value.ToString("dd-MM-yyyy") : "",
                    total = x.total
                });

                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult VentasPorMes(string fechaInicio, string fechaFin)
        {
            if (!UsuarioEsAdmin()) return new HttpStatusCodeResult(401);

            DateTime? inicio = null;
            DateTime? fin = null;
            string formato = "dd-MM-yyyy";

            if (!string.IsNullOrWhiteSpace(fechaInicio) &&
                DateTime.TryParseExact(fechaInicio, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fInicio))
                inicio = fInicio.Date;

            if (!string.IsNullOrWhiteSpace(fechaFin) &&
                DateTime.TryParseExact(fechaFin, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fFin))
                fin = fFin.Date.AddDays(1).AddTicks(-1);

            if (!inicio.HasValue || !fin.HasValue)
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);

            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var rows = (from v in db.tabla_ventas
                            where v.fecha >= inicio && v.fecha <= fin
                            group v by new { v.fecha.Value.Year, v.fecha.Value.Month } into g
                            orderby g.Key.Year, g.Key.Month
                            select new
                            {
                                y = g.Key.Year,
                                m = g.Key.Month,
                                total = g.Sum(x => (decimal?)x.total) ?? 0m
                            }).ToList();

                var data = rows.Select(x => new
                {
                    label = $"{x.y:D4}-{x.m:D2}",
                    total = x.total
                });

                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }



    }
}
