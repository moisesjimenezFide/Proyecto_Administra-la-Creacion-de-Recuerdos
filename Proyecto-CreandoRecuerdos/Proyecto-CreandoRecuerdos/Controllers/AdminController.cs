using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Filters;
using Proyecto_CreandoRecuerdos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    // Evitar el almacenamiento en caché de las vistas
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]

    [RolAuthorize("1")]
    public class AdminController : Controller
    {
        private readonly BaseColor ColorHeader = new BaseColor(181, 72, 133);  // #B54885
        private readonly BaseColor ColorHover = new BaseColor(204, 143, 174);  // #CC8FAE
        private readonly BaseColor ColorAcento = new BaseColor(255, 214, 235); // #FFD6EB
        private readonly BaseColor ColorTexto = new BaseColor(44, 44, 44);     // #2C2C2C
        private readonly BaseColor ColorBlanco = new BaseColor(255, 255, 255); // #FFFFFF

        private static readonly string FontPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Fonts/DejaVuSans.ttf");
        private static readonly Font FontNormal = FontFactory.GetFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 11, Font.NORMAL, new BaseColor(44, 44, 44));
        private static readonly Font FontBold = FontFactory.GetFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 12, Font.BOLD, new BaseColor(255, 255, 255));

        private PdfPCell PdfHeader(string texto)
        {
            return new PdfPCell(new Phrase(texto, FontBold))
            {
                BackgroundColor = ColorHeader,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 6,
                BorderColor = ColorHover
            };
        }

        private PdfPCell PdfCell(string texto)
        {
            return new PdfPCell(new Phrase(texto, FontNormal))
            {
                BackgroundColor = ColorBlanco,
                Padding = 5,
                BorderColor = ColorHover
            };
        }

        private void AplicarEstilosExcel(IXLWorksheet ws, int columnas)
        {
            // Encabezado
            var header = ws.Range(1, 1, 1, columnas);
            header.Style.Fill.BackgroundColor = XLColor.FromHtml("#B54885");  // color-principal-oscuro
            header.Style.Font.FontColor = XLColor.FromHtml("#FFFFFF");        // blanco
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Bordes y filas
            var data = ws.Range(2, 1, ws.LastRowUsed().RowNumber(), columnas);
            data.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            data.Style.Border.BottomBorderColor = XLColor.FromHtml("#CC8FAE"); // color-principal-hover
            data.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");     // blanco

            ws.Columns().AdjustToContents();
        }

        [HttpGet]
        public ActionResult HistorialActividades()
        {

            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                // Usuarios
                ViewBag.Usuarios = new SelectList(
                    context.tabla_usuarios.Where(u => u.activo == true).ToList(),
                    "id_usuario",
                    "nombre");

                // Tablas afectadas
                ViewBag.TablasAfectadas = new SelectList(
                    context.tabla_actividades
                        .Select(a => a.tabla_afectada)
                        .Distinct()
                        .ToList());

                // Tipos de acción
                ViewBag.TiposAccion = new SelectList(
                    context.tabla_actividades
                        .Select(a => a.tipo_accion)
                        .Distinct()
                        .ToList());

                return View();
            }
        }

        [HttpPost]
        public ActionResult FiltrarActividades(FiltroActividadesModel filtros)
        {
            if (Session["Rol"]?.ToString() != "1")
            {
                return Json(new { success = false, message = "No autorizado" });
            }

            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                var actividades = context.sp_obtener_historial_actividades(
                    id_usuario: filtros.IdUsuario,
                    fecha_inicio: filtros.FechaInicio,
                    fecha_fin: filtros.FechaFin,
                    tipo_accion: filtros.TipoAccion,
                    tabla_afectada: filtros.TablaAfectada,
                    id_registro_afectado: filtros.IdRegistroAfectado
                ).ToList();

                var resultado = actividades.Select(a => new ActividadModel
                {
                    IdActividad = a.id_actividad,
                    IdUsuario = a.id_usuario,
                    NombreUsuario = a.nombre_usuario,
                    TipoAccion = a.tipo_accion,
                    TablaAfectada = a.tabla_afectada,
                    IdRegistroAfectado = a.id_registro_afectado,
                    ValoresAnteriores = !string.IsNullOrEmpty(a.valores_anteriores) ?
                        JsonConvert.DeserializeObject<Dictionary<string, object>>(a.valores_anteriores) : null,
                    ValoresNuevos = !string.IsNullOrEmpty(a.valores_nuevos) ?
                        JsonConvert.DeserializeObject<Dictionary<string, object>>(a.valores_nuevos) : null,
                    Descripcion = a.descripcion,
                    FechaHora = a.fecha_hora
                }).ToList();

                return PartialView("_Actividades", resultado);
            }
        }

        [HttpPost]
        public ActionResult ExportarHistorial(FiltroActividadesModel filtros, string formato)
        {
            try
            {

                using (var context = new BD_CREANDO_RECUERDOSEntities())
                {
                    var actividades = context.sp_obtener_historial_actividades(
                        id_usuario: filtros.IdUsuario,
                        fecha_inicio: filtros.FechaInicio,
                        fecha_fin: filtros.FechaFin?.AddDays(1),
                        tipo_accion: filtros.TipoAccion,
                        tabla_afectada: filtros.TablaAfectada,
                        id_registro_afectado: filtros.IdRegistroAfectado
                    ).ToList();

                    if (!actividades.Any())
                    {
                        throw new InvalidOperationException("No hay datos para exportar");
                    }

                    string nombreArchivo = $"HistorialActividades_{DateTime.Now:yyyyMMddHHmmss}";

                    switch (formato?.ToUpper())
                    {
                        case "PDF":
                            return GenerarPDF(actividades, nombreArchivo);
                        case "EXCEL":
                            return GenerarExcel(actividades, nombreArchivo); 
                        default:
                            throw new NotSupportedException("Formato no soportado. Use PDF o Excel.");
                    }
                }
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = ex.Message,
                    });
                }

                return Content($"<script>alert('Error al exportar: {ex.Message}');</script>", "text/html");
            }
        }

        private ActionResult GenerarExcel(List<sp_obtener_historial_actividades_Result> actividades, string nombreArchivo)
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("HistorialActividades");

                // ====================== CONFIG ======================
                string titulo = "Historial de Actividades";
                int columnas = 6;

                // ====================== TÍTULO ======================
                ws.Cell(1, 1).Value = titulo;
                ws.Range(1, 1, 1, columnas).Merge();

                var tituloRango = ws.Range(1, 1, 1, columnas);
                tituloRango.Style.Font.Bold = true;
                tituloRango.Style.Font.FontSize = 18;
                tituloRango.Style.Font.FontColor = XLColor.FromHtml("#2C2C2C");
                tituloRango.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                tituloRango.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                tituloRango.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFD6EB"); // pastel suave
                ws.Row(1).Height = 30;

                // ====================== ENCABEZADOS ======================
                ws.Cell(2, 1).Value = "Fecha y Hora";
                ws.Cell(2, 2).Value = "Usuario";
                ws.Cell(2, 3).Value = "Tipo de Acción";
                ws.Cell(2, 4).Value = "Tabla Afectada";
                ws.Cell(2, 5).Value = "ID Registro";
                ws.Cell(2, 6).Value = "Descripción";

                var header = ws.Range(2, 1, 2, columnas);
                header.Style.Fill.BackgroundColor = XLColor.FromHtml("#B54885"); // rosa oscuro
                header.Style.Font.FontColor = XLColor.White;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                header.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                header.Style.Border.OutsideBorderColor = XLColor.FromHtml("#CC8FAE");
                header.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                header.Style.Border.InsideBorderColor = XLColor.FromHtml("#CC8FAE");

                // ====================== DATOS ======================
                for (int i = 0; i < actividades.Count; i++)
                {
                    int row = i + 3;

                    ws.Cell(row, 1).Value = actividades[i].fecha_hora.ToString("g");
                    ws.Cell(row, 2).Value = actividades[i].nombre_usuario;
                    ws.Cell(row, 3).Value = actividades[i].tipo_accion;
                    ws.Cell(row, 4).Value = actividades[i].tabla_afectada;
                    ws.Cell(row, 5).Value = actividades[i].id_registro_afectado?.ToString() ?? "N/A";
                    ws.Cell(row, 6).Value = actividades[i].descripcion;

                    // SOMBREADO ALTERNADO
                    if (i % 2 == 1)
                        ws.Range(row, 1, row, columnas).Style.Fill.BackgroundColor = XLColor.FromHtml("#F5F5F5");
                }

                // ====================== ESTILOS GENERALES ======================
                var data = ws.Range(2, 1, ws.LastRowUsed().RowNumber(), columnas);

                data.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                data.Style.Border.OutsideBorderColor = XLColor.FromHtml("#CC8FAE");
                data.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                data.Style.Border.InsideBorderColor = XLColor.FromHtml("#CC8FAE");

                data.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                data.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                data.Style.Font.FontColor = XLColor.FromHtml("#2C2C2C");

                // ====================== AJUSTAR COLUMNAS ======================
                ws.Columns().AdjustToContents();

                // ====================== DESCARGA ======================
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"{nombreArchivo}.xlsx"
                    );
                }
            }
        }

        private ActionResult GenerarPDF(List<sp_obtener_historial_actividades_Result> actividades, string nombreArchivo)
        {
            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
                PdfWriter.GetInstance(doc, ms).CloseStream = false;

                doc.Open();

                // ====== TÍTULO ======
                var titulo = new Paragraph("Historial de Actividades",
                    FontFactory.GetFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 20, Font.BOLD, ColorTexto))
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                doc.Add(titulo);

                // ====== TABLA ======
                PdfPTable tabla = new PdfPTable(6)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 10f
                };

                tabla.SetWidths(new float[] { 20, 20, 20, 20, 15, 45 });

                tabla.AddCell(PdfHeader("Fecha/Hora"));
                tabla.AddCell(PdfHeader("Usuario"));
                tabla.AddCell(PdfHeader("Acción"));
                tabla.AddCell(PdfHeader("Tabla"));
                tabla.AddCell(PdfHeader("ID Registro"));
                tabla.AddCell(PdfHeader("Descripción"));

                int index = 0;
                foreach (var act in actividades)
                {
                    bool shade = index % 2 == 1;

                    tabla.AddCell(PdfCell(act.fecha_hora.ToString("g"), shade));
                    tabla.AddCell(PdfCell(act.nombre_usuario, shade));
                    tabla.AddCell(PdfCell(act.tipo_accion, shade));
                    tabla.AddCell(PdfCell(act.tabla_afectada, shade));
                    tabla.AddCell(PdfCell(act.id_registro_afectado?.ToString() ?? "N/A", shade));
                    tabla.AddCell(PdfCell(act.descripcion, shade));

                    index++;
                }

                doc.Add(tabla);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", $"{nombreArchivo}.pdf");
            }
        }
    }
}