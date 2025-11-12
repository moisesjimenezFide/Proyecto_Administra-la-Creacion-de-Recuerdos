using ClosedXML.Excel;
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
                if (Session["Rol"]?.ToString() != "1")
                {
                    throw new UnauthorizedAccessException("No autorizado");
                }

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
                var worksheet = workbook.Worksheets.Add("Historial");

                // Establecer propiedades del documento
                workbook.Properties.Title = "Historial de Actividades";
                workbook.Properties.Author = "Creando Recuerdos";
                workbook.Properties.Company = "Creando Recuerdos";

                // Encabezados
                worksheet.Cell(1, 1).Value = "Fecha y Hora";
                worksheet.Cell(1, 2).Value = "Usuario";
                worksheet.Cell(1, 3).Value = "Tipo de Acción";
                worksheet.Cell(1, 4).Value = "Tabla Afectada";
                worksheet.Cell(1, 5).Value = "ID Registro";
                worksheet.Cell(1, 6).Value = "Descripción";

                // Formato de encabezados
                var headerRange = worksheet.Range("A1:F1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                // Datos
                for (int i = 0; i < actividades.Count; i++)
                {
                    var row = i + 2;
                    worksheet.Cell(row, 1).Value = actividades[i].fecha_hora.ToString("g");
                    worksheet.Cell(row, 2).Value = actividades[i].nombre_usuario;
                    worksheet.Cell(row, 3).Value = actividades[i].tipo_accion;
                    worksheet.Cell(row, 4).Value = actividades[i].tabla_afectada;
                    worksheet.Cell(row, 5).Value = actividades[i].id_registro_afectado?.ToString() ?? "N/A";
                    worksheet.Cell(row, 6).Value = actividades[i].descripcion;

                    if (i % 2 == 0)
                    {
                        var rowRange = worksheet.Range($"A{row}:F{row}");
                        rowRange.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
                    }
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"{nombreArchivo}.xlsx");
                }
            }
        }

        private ActionResult GenerarPDF(List<sp_obtener_historial_actividades_Result> actividades, string nombreArchivo)
        {
            using (var memoryStream = new MemoryStream())
            {
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate());
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);

                document.Open();

                // Agregar título
                var titulo = new iTextSharp.text.Paragraph("Historial de Actividades",
                    iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18));
                titulo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                document.Add(titulo);

                // Crear tabla
                var table = new iTextSharp.text.pdf.PdfPTable(6)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 10f,
                    SpacingAfter = 10f
                };

                // Encabezados
                AddCell(table, "Fecha/Hora", true);
                AddCell(table, "Usuario", true);
                AddCell(table, "Acción", true);
                AddCell(table, "Tabla", true);
                AddCell(table, "ID Registro", true);
                AddCell(table, "Descripción", true);

                // Datos
                foreach (var act in actividades)
                {
                    AddCell(table, act.fecha_hora.ToString("g"));
                    AddCell(table, act.nombre_usuario);
                    AddCell(table, act.tipo_accion);
                    AddCell(table, act.tabla_afectada);
                    AddCell(table, act.id_registro_afectado?.ToString() ?? "N/A");
                    AddCell(table, act.descripcion);
                }

                document.Add(table);
                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", $"{nombreArchivo}.pdf");
            }
        }

        private void AddCell(iTextSharp.text.pdf.PdfPTable table, string text, bool isHeader = false)
        {
            var font = isHeader
                ? iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10)
                : iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);

            var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(text, font))
            {
                HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT,
                Padding = 5f
            };

            if (isHeader)
            {
                cell.BackgroundColor = new iTextSharp.text.BaseColor(220, 220, 220);
            }

            table.AddCell(cell);
        }
    }
}