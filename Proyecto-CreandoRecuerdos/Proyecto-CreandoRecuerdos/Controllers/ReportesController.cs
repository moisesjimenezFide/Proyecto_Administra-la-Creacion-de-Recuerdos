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

        public ActionResult ReportesIndex()
        {
            return View("ReportesIndex");
        }

        // ======================= COLORES =======================

        private readonly BaseColor ColorHeader = new BaseColor(181, 72, 133);   // #B54885
        private readonly BaseColor ColorHover = new BaseColor(204, 143, 174);  // #CC8FAE
        private readonly BaseColor ColorBlanco = new BaseColor(255, 255, 255);  // #FFFFFF
        private readonly BaseColor ColorTexto = new BaseColor(44, 44, 44);     // #2C2C2C

        // ======================= FUENTES =======================

        private static readonly string FontPath =
            System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Fonts/DejaVuSans.ttf");

        private static readonly Font FontNormal =
            FontFactory.GetFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 11,
                Font.NORMAL, new BaseColor(44, 44, 44));

        private static readonly Font FontBold =
            FontFactory.GetFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 12,
                Font.BOLD, new BaseColor(255, 255, 255));

        // ======================= CELDA HEADER =======================

        private PdfPCell PdfHeader(string texto)
        {
            return new PdfPCell(new Phrase(texto, FontBold))
            {
                BackgroundColor = ColorHeader,

                // Alineación igual que pdfHtml5
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,

                // Paddings EXACTOS del JS
                PaddingLeft = 4,
                PaddingRight = 4,
                PaddingTop = 6,
                PaddingBottom = 6,

                // Bordes IGUALES al JS
                BorderColor = ColorHover,
                BorderWidth = 1f
            };
        }

        // ======================= CELDA NORMAL =======================

        private PdfPCell PdfCell(string texto, bool sombreado = false)
        {
            return new PdfPCell(new Phrase(texto, FontNormal))
            {
                BackgroundColor = sombreado
                    ? new BaseColor(245, 245, 245)          // gris suave para filas alternadas
                    : ColorBlanco,

                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,

                PaddingTop = 5,
                PaddingBottom = 5,
                PaddingLeft = 4,
                PaddingRight = 4,

                BorderColor = ColorHover,
                BorderWidth = 0.8f
            };
        }

        private void AplicarEstilosExcel(IXLWorksheet ws, int columnas)
        {
            // ================================
            // 1. ENCABEZADO
            // ================================
            var header = ws.Range(1, 1, 1, columnas);

            header.Style.Fill.BackgroundColor = XLColor.FromHtml("#B54885"); // Rosa oscuro
            header.Style.Font.FontColor = XLColor.White;
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Bordes del encabezado (pastel)
            header.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            header.Style.Border.OutsideBorderColor = XLColor.FromHtml("#CC8FAE");
            header.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            header.Style.Border.InsideBorderColor = XLColor.FromHtml("#CC8FAE");


            // ================================
            // 2. CUERPO DE LA TABLA
            // ================================
            var data = ws.Range(2, 1, ws.LastRowUsed().RowNumber(), columnas);

            data.Style.Font.FontColor = XLColor.FromHtml("#2C2C2C"); // Texto gris oscuro

            // Centrado total
            data.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            data.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Bordes pastel
            data.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            data.Style.Border.OutsideBorderColor = XLColor.FromHtml("#CC8FAE");
            data.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            data.Style.Border.InsideBorderColor = XLColor.FromHtml("#CC8FAE");


            // ================================
            // 3. SOMBREADO ALTERNADO (gris suave)
            // ================================
            int lastRow = ws.LastRowUsed().RowNumber();

            for (int row = 2; row <= lastRow; row++)
            {
                if (row % 2 == 1) // filas impares → gris suave
                {
                    ws.Range(row, 1, row, columnas)
                      .Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F2F2"); // Gris pastel
                }
                else
                {
                    ws.Range(row, 1, row, columnas)
                      .Style.Fill.BackgroundColor = XLColor.White;
                }
            }

            // ================================
            // 4. Ajuste de columnas
            // ================================
            ws.Columns().AdjustToContents();
        }

        public ActionResult HistorialVentas(string fechaInicio, string fechaFin)
        {
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
                DateTime? inicio = fechaInicio?.Date;
                DateTime? fin = fechaFin?.Date.AddDays(1).AddTicks(-1);

                var q = from v in db.tabla_ventas
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

                var ventas = q.OrderByDescending(x => x.fecha)
                              .Select(x => new HistorialVentasViewModel
                              {
                                  IdVenta = x.id_venta,
                                  Fecha = x.fecha ?? DateTime.MinValue,
                                  Total = x.total,
                                  Cliente = x.cliente,
                                  Usuario = x.usuario
                              })
                              .ToList();

                if (!ventas.Any())
                    return new HttpStatusCodeResult(204);

                // ===============================================================
                // PDF
                // ===============================================================
                if (formato == "PDF")
                {
                    using (var stream = new MemoryStream())
                    {
                        var doc = new iTextSharp.text.Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, stream).CloseStream = false;
                        doc.Open();

                        // ---- TÍTULO
                        var titulo = new Paragraph("Historial de Ventas",
                            FontFactory.GetFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 20, Font.BOLD, ColorTexto));

                        titulo.Alignment = Element.ALIGN_CENTER;
                        titulo.SpacingAfter = 20;
                        doc.Add(titulo);

                        // ---- TABLA
                        PdfPTable table = new PdfPTable(5);
                        table.WidthPercentage = 100;

                        table.AddCell(PdfHeader("ID Venta"));
                        table.AddCell(PdfHeader("Fecha"));
                        table.AddCell(PdfHeader("Total"));
                        table.AddCell(PdfHeader("Cliente"));
                        table.AddCell(PdfHeader("Vendedor"));

                        var cr = new CultureInfo("es-CR");
                        cr.NumberFormat.CurrencySymbol = "₡";
                        cr.NumberFormat.CurrencyPositivePattern = 0;

                        int idx = 0;
                        foreach (var v in ventas)
                        {
                            bool shade = idx % 2 == 1;

                            table.AddCell(PdfCell(v.IdVenta.ToString(), shade));
                            table.AddCell(PdfCell(v.Fecha.ToString("dd/MM/yyyy"), shade));
                            table.AddCell(PdfCell("₡ " + v.Total.ToString("N2", cr), shade));
                            table.AddCell(PdfCell(v.Cliente, shade));
                            table.AddCell(PdfCell(v.Usuario, shade));

                            idx++;
                        }

                        doc.Add(table);
                        doc.Close();

                        return File(stream.ToArray(), "application/pdf", "HistorialVentas.pdf");
                    }
                }

                // ===============================================================
                // EXCEL
                // ===============================================================
                if (formato == "EXCEL")
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("HistorialVentas");

                        // >>> DECLARAR VARIABLES<<<
                        string titulo = "Historial de Ventas";
                        int columnas = 5;

                        // ====================== Título pastel ======================
                        ws.Cell(1, 1).Value = titulo;
                        ws.Range(1, 1, 1, columnas).Merge();
                        ws.Range(1, 1, 1, columnas).Style.Font.Bold = true;
                        ws.Range(1, 1, 1, columnas).Style.Font.FontSize = 18;
                        ws.Range(1, 1, 1, columnas).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range(1, 1, 1, columnas).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFD6EB");
                        ws.Range(1, 1, 1, columnas).Style.Font.FontColor = XLColor.FromHtml("#2C2C2C");
                        ws.Row(1).Height = 25;

                        // ======= Encabezados en fila 2 =======
                        ws.Cell(2, 1).Value = "ID Venta";
                        ws.Cell(2, 2).Value = "Fecha";
                        ws.Cell(2, 3).Value = "Total";
                        ws.Cell(2, 4).Value = "Cliente";
                        ws.Cell(2, 5).Value = "Vendedor";

                        // ======= Datos desde fila 3 =======
                        for (int i = 0; i < ventas.Count; i++)
                        {
                            int row = i + 3;

                            ws.Cell(row, 1).Value = ventas[i].IdVenta;
                            ws.Cell(row, 2).Value = ventas[i].Fecha.ToString("dd/MM/yyyy");
                            ws.Cell(row, 3).Value = ventas[i].Total;
                            ws.Cell(row, 4).Value = ventas[i].Cliente;
                            ws.Cell(row, 5).Value = ventas[i].Usuario;

                            if (i % 2 == 1)
                                ws.Range(row, 1, row, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#F5F5F5");
                        }

                        // Estilos pastel globales
                        AplicarEstilosExcel(ws, columnas);

                        // Formato moneda
                        ws.Column(3).Style.NumberFormat.Format = "[$₡-es-CR] #,##0.00";

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            return File(
                                stream.ToArray(),
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
                        Usuario = u.correo,
                        NombreCompleto = u.nombre,
                        Rol = u.tabla_roles.nombre,
                        Estado = u.activo == true ? "Activo" : "Inactivo"
                    })
                    .ToList();

                // ========== PDF ==========
                if (formato == "PDF")
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        Document doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, stream).CloseStream = false;
                        doc.Open();

                        var titulo = new Paragraph("Empleados Disponibles",
                            FontFactory.GetFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 20, Font.BOLD, ColorTexto));
                        titulo.Alignment = Element.ALIGN_CENTER;
                        titulo.SpacingAfter = 20;
                        doc.Add(titulo);

                        PdfPTable table = new PdfPTable(4);
                        table.WidthPercentage = 100;

                        table.AddCell(PdfHeader("Nombre"));
                        table.AddCell(PdfHeader("Correo"));
                        table.AddCell(PdfHeader("Rol"));
                        table.AddCell(PdfHeader("Estado"));

                        int idx = 0;
                        foreach (var e in empleados)
                        {
                            bool shade = idx % 2 == 1;

                            table.AddCell(PdfCell(e.NombreCompleto, shade));
                            table.AddCell(PdfCell(e.Usuario, shade));
                            table.AddCell(PdfCell(e.Rol, shade));
                            table.AddCell(PdfCell(e.Estado, shade));

                            idx++;
                        }

                        doc.Add(table);
                        doc.Close();

                        return File(stream.ToArray(), "application/pdf", "EmpleadosDisponibles.pdf");
                    }
                }

                // ========== EXCEL ==========
                else if (formato == "EXCEL")
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("EmpleadosDisponibles");

                        string titulo = "Empleados Disponibles";
                        int columnas = 4;

                        ws.Cell(1, 1).Value = titulo;
                        ws.Range(1, 1, 1, columnas).Merge();
                        ws.Range(1, 1, 1, columnas).Style.Font.Bold = true;
                        ws.Range(1, 1, 1, columnas).Style.Font.FontSize = 18;
                        ws.Range(1, 1, 1, columnas).Style.Font.FontColor = XLColor.FromHtml("#2C2C2C");
                        ws.Range(1, 1, 1, columnas).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range(1, 1, 1, columnas).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFD6EB");
                        ws.Row(1).Height = 25;

                        ws.Cell(2, 1).Value = "Nombre";
                        ws.Cell(2, 2).Value = "Correo";
                        ws.Cell(2, 3).Value = "Rol";
                        ws.Cell(2, 4).Value = "Estado";

                        for (int i = 0; i < empleados.Count; i++)
                        {
                            int row = i + 3;

                            ws.Cell(row, 1).Value = empleados[i].NombreCompleto;
                            ws.Cell(row, 2).Value = empleados[i].Usuario;
                            ws.Cell(row, 3).Value = empleados[i].Rol;
                            ws.Cell(row, 4).Value = empleados[i].Estado;

                            if (i % 2 == 1)
                                ws.Range(row, 1, row, columnas).Style.Fill.BackgroundColor = XLColor.FromHtml("#F5F5F5");
                        }

                        AplicarEstilosExcel(ws, columnas);

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            return File(
                                stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "EmpleadosDisponibles.xlsx"
                            );
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

                var cr = new CultureInfo("es-CR");
                cr.NumberFormat.CurrencySymbol = "₡";

                if (formato == "PDF")
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        Document doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, stream).CloseStream = false;
                        doc.Open();

                        var titulo = new Paragraph("Productos del Menú",
                            FontFactory.GetFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 20, Font.BOLD, ColorTexto));
                        titulo.Alignment = Element.ALIGN_CENTER;
                        titulo.SpacingAfter = 20;
                        doc.Add(titulo);

                        PdfPTable table = new PdfPTable(3);
                        table.WidthPercentage = 100;

                        table.AddCell(PdfHeader("Nombre"));
                        table.AddCell(PdfHeader("Descripción"));
                        table.AddCell(PdfHeader("Precio por Unidad"));

                        int idx = 0;
                        foreach (var p in productos)
                        {
                            bool shade = idx % 2 == 1;

                            table.AddCell(PdfCell(p.Nombre, shade));
                            table.AddCell(PdfCell(p.Descripcion, shade));
                            table.AddCell(PdfCell("₡ " + p.PrecioUnidad.ToString("N2", cr), shade));

                            idx++;
                        }

                        doc.Add(table);
                        doc.Close();

                        return File(stream.ToArray(), "application/pdf", "ProductosDisponibles.pdf");
                    }
                }

                else if (formato == "EXCEL")
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("ProductosDisponibles");

                        string titulo = "Productos del Menú";
                        int columnas = 3;

                        // === TÍTULO PASTEL ===
                        ws.Cell(1, 1).Value = titulo;
                        ws.Range(1, 1, 1, columnas).Merge();
                        ws.Range(1, 1, 1, columnas).Style.Font.Bold = true;
                        ws.Range(1, 1, 1, columnas).Style.Font.FontSize = 18;
                        ws.Range(1, 1, 1, columnas).Style.Font.FontColor = XLColor.FromHtml("#2C2C2C");
                        ws.Range(1, 1, 1, columnas).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range(1, 1, 1, columnas).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFD6EB");
                        ws.Row(1).Height = 25;

                        // Encabezados (fila 2)
                        ws.Cell(2, 1).Value = "Nombre";
                        ws.Cell(2, 2).Value = "Descripción";
                        ws.Cell(2, 3).Value = "Precio por Unidad";

                        for (int i = 0; i < productos.Count; i++)
                        {
                            int row = i + 3;

                            ws.Cell(row, 1).Value = productos[i].Nombre;
                            ws.Cell(row, 2).Value = productos[i].Descripcion;
                            ws.Cell(row, 3).Value = productos[i].PrecioUnidad;

                            if (i % 2 == 1)
                                ws.Range(row, 1, row, columnas).Style.Fill.BackgroundColor = XLColor.FromHtml("#F5F5F5");
                        }

                        AplicarEstilosExcel(ws, columnas);
                        ws.Column(3).Style.NumberFormat.Format = "[$₡-es-CR] #,##0.00";

                        using (MemoryStream stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            return File(
                                stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "ProductosDisponibles.xlsx"
                            );
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

                        // ---- Título pastel
                        var titulo = new Paragraph("Costos Operativos Promedios",
                            FontFactory.GetFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 20, Font.BOLD, ColorTexto));
                        titulo.Alignment = Element.ALIGN_CENTER;
                        titulo.SpacingAfter = 20;
                        doc.Add(titulo);

                        PdfPTable table = new PdfPTable(2);
                        table.WidthPercentage = 100;

                        table.AddCell(PdfHeader("Categoría"));
                        table.AddCell(PdfHeader("Costo Promedio"));

                        var cr = new CultureInfo("es-CR");
                        cr.NumberFormat.CurrencySymbol = "₡";
                        cr.NumberFormat.CurrencyPositivePattern = 0;

                        int idx = 0;
                        Action<string, decimal> AddRow = (categoria, valor) =>
                        {
                            bool shade = idx % 2 == 1;
                            table.AddCell(PdfCell(categoria, shade));
                            table.AddCell(PdfCell("₡ " + valor.ToString("N2", cr), shade));
                            idx++;
                        };

                        AddRow("Recetas", viewModel.PromedioCostosRecetas);
                        AddRow("Empaques", viewModel.PromedioCostosEmpaques);
                        AddRow("Implementos", viewModel.PromedioCostosImplementos);
                        AddRow("Suministros", viewModel.PromedioCostosSuministros);

                        doc.Add(table);
                        doc.Close();

                        return File(stream.ToArray(), "application/pdf", "CostosOperativos.pdf");
                    }
                }

                else if (formato == "EXCEL")
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("CostosOperativos");

                        string titulo = "Costos Operativos Promedios";
                        int columnas = 2;

                        // ====================== TÍTULO PASTEL ======================
                        ws.Cell(1, 1).Value = titulo;
                        ws.Range(1, 1, 1, columnas).Merge();
                        ws.Range(1, 1, 1, columnas).Style.Font.Bold = true;
                        ws.Range(1, 1, 1, columnas).Style.Font.FontSize = 18;
                        ws.Range(1, 1, 1, columnas).Style.Font.FontColor = XLColor.FromHtml("#2C2C2C");
                        ws.Range(1, 1, 1, columnas).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range(1, 1, 1, columnas).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFD6EB");
                        ws.Row(1).Height = 25;

                        // ===== Encabezados en fila 2 =====
                        ws.Cell(2, 1).Value = "Categoría";
                        ws.Cell(2, 2).Value = "Costo Promedio";

                        // ===== Datos desde fila 3 =====
                        ws.Cell(3, 1).Value = "Recetas";
                        ws.Cell(3, 2).Value = viewModel.PromedioCostosRecetas;

                        ws.Cell(4, 1).Value = "Empaques";
                        ws.Cell(4, 2).Value = viewModel.PromedioCostosEmpaques;

                        ws.Cell(5, 1).Value = "Implementos";
                        ws.Cell(5, 2).Value = viewModel.PromedioCostosImplementos;

                        ws.Cell(6, 1).Value = "Suministros";
                        ws.Cell(6, 2).Value = viewModel.PromedioCostosSuministros;

                        // sombreado alternado gris claro
                        for (int r = 3; r <= 6; r++)
                            if (r % 2 == 1)
                                ws.Range(r, 1, r, columnas).Style.Fill.BackgroundColor = XLColor.FromHtml("#F5F5F5");

                        // aplicar estilos globales pastel
                        AplicarEstilosExcel(ws, columnas);

                        // formato moneda CR
                        ws.Column(2).Style.NumberFormat.Format = "[$₡-es-CR] #,##0.00";

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "CostosOperativos.xlsx");
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
                    .OrderByDescending(x => x.Anio)
                    .ThenByDescending(x => x.Mes)
                    .ToList();

                if (formato == "PDF")
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        Document doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, stream).CloseStream = false;
                        doc.Open();

                        var titulo = new Paragraph("Ventas Mensuales",
                            FontFactory.GetFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 20, Font.BOLD, ColorTexto));
                        titulo.Alignment = Element.ALIGN_CENTER;
                        titulo.SpacingAfter = 20;
                        doc.Add(titulo);

                        PdfPTable table = new PdfPTable(3);
                        table.WidthPercentage = 100;

                        table.AddCell(PdfHeader("Año"));
                        table.AddCell(PdfHeader("Mes"));
                        table.AddCell(PdfHeader("Total Ventas"));

                        var cr = new CultureInfo("es-CR");
                        cr.NumberFormat.CurrencySymbol = "₡";

                        int idx = 0;
                        foreach (var item in resumen)
                        {
                            bool shade = idx % 2 == 1;

                            table.AddCell(PdfCell(item.Anio.ToString(), shade));
                            table.AddCell(PdfCell(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Mes), shade));
                            table.AddCell(PdfCell("₡ " + item.Total.ToString("N2", cr), shade));

                            idx++;
                        }

                        doc.Add(table);
                        doc.Close();

                        return File(stream.ToArray(), "application/pdf", "VentasMensuales.pdf");
                    }
                }

                else if (formato == "EXCEL")
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("VentasMensuales");

                        string titulo = "Ventas Mensuales";
                        int columnas = 3;

                        ws.Cell(1, 1).Value = titulo;
                        ws.Range(1, 1, 1, columnas).Merge();
                        ws.Range(1, 1, 1, columnas).Style.Font.Bold = true;
                        ws.Range(1, 1, 1, columnas).Style.Font.FontSize = 18;
                        ws.Range(1, 1, 1, columnas).Style.Font.FontColor = XLColor.FromHtml("#2C2C2C");
                        ws.Range(1, 1, 1, columnas).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFD6EB");
                        ws.Range(1, 1, 1, columnas).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Row(1).Height = 25;

                        // encabezados en fila 2
                        ws.Cell(2, 1).Value = "Año";
                        ws.Cell(2, 2).Value = "Mes";
                        ws.Cell(2, 3).Value = "Total Ventas";

                        for (int i = 0; i < resumen.Count; i++)
                        {
                            int row = i + 3;

                            ws.Cell(row, 1).Value = resumen[i].Anio;
                            ws.Cell(row, 2).Value = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(resumen[i].Mes);
                            ws.Cell(row, 3).Value = resumen[i].Total;

                            if (i % 2 == 1)
                                ws.Range(row, 1, row, columnas).Style.Fill.BackgroundColor = XLColor.FromHtml("#F5F5F5");
                        }

                        AplicarEstilosExcel(ws, columnas);
                        ws.Column(3).Style.NumberFormat.Format = "[$₡-es-CR] #,##0.00";

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            return File(
                                stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "VentasMensuales.xlsx"
                            );
                        }
                    }
                }

                return new HttpStatusCodeResult(400, "Formato no soportado");
            }
        }


        [HttpGet]
        public ActionResult VentasPorDia(string fechaInicio, string fechaFin)
        {
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
