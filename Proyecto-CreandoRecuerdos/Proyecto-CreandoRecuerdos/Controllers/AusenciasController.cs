using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proyecto_CreandoRecuerdos.Models;
using Proyecto_CreandoRecuerdos.base_de_datos;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    public class AusenciasController : Controller
    {
        // GET: Ausencias
        private BD_CREANDO_RECUERDOSEntities db = new BD_CREANDO_RECUERDOSEntities();

        public ActionResult Index()
        {
            var solicitudes = db.tabla_solicitudes_ausencias.ToList();
            return View("AprobacionRechazoSolicitudAusencia", solicitudes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aprobar(int id)
        {
            var solicitud = db.tabla_solicitudes_ausencias.Find(id);
            if (solicitud != null && solicitud.estado == "Pendiente")
            {
                solicitud.estado = "Aprobado";
                db.SaveChanges();
                TempData["Mensaje"] = "Solicitud aprobada correctamente.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rechazar(int id)
        {
            var solicitud = db.tabla_solicitudes_ausencias.Find(id);
            if (solicitud != null && solicitud.estado == "Pendiente")
            {
                solicitud.estado = "Rechazado";
                db.SaveChanges();
                TempData["Mensaje"] = "Solicitud rechazada correctamente.";
            }
            return RedirectToAction("Index");
        }

        // GET: Ausencias/Solicitar
        public ActionResult Solicitar()
        {
            return View(new SolicitudAusenciaModel());
        }

        // POST: Ausencias/Solicitar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Solicitar(SolicitudAusenciaModel model)
        {
            if (ModelState.IsValid)
            {
                // Aquí deberías obtener el IdUsuario del usuario autenticado
                // model.IdUsuario = ...;

                var entidad = new tabla_solicitudes_ausencias
                {
                    id_usuario = model.IdUsuario,
                    fecha_inicio = model.FechaInicio,
                    fecha_fin = model.FechaFin,
                    tipo = model.Tipo,
                    motivo = model.Motivo,
                    estado = "Pendiente"
                };

                db.tabla_solicitudes_ausencias.Add(entidad);
                db.SaveChanges();

                TempData["Mensaje"] = "Solicitud enviada correctamente.";
                return RedirectToAction("Solicitar");
            }

            return View(model);
        }
    }
}
