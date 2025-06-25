using System.Linq;
using System.Web.Mvc;
using Proyecto_CreandoRecuerdos.Models;
using Proyecto_CreandoRecuerdos.base_de_datos;


namespace Proyecto_CreandoRecuerdos.Controllers
{
    public class AusenciasController : Controller
    {
        // GET: Ausencias
        private BD_CREANDO_RECUERDOSEntities db = new BD_CREANDO_RECUERDOSEntities();

        public ActionResult GestionSolicitudesAusencias()
        {
            var solicitudes = db.tabla_solicitudes_ausencias.ToList();
            return View(solicitudes);
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
            return RedirectToAction("GestionSolicitudesAusencias");
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
            return RedirectToAction("GestionSolicitudesAusencias");
        }

        // GET: Ausencias/SolicitudAusencia
        public ActionResult SolicitudAusencia()
        {
            int idUsuario = (int)Session["IdUsuario"];
            var solicitudes = db.tabla_solicitudes_ausencias
                .Where(s => s.id_usuario == idUsuario)
                .OrderByDescending(s => s.fecha_inicio)
                .ToList();

            ViewBag.Solicitudes = solicitudes;
            return View(new SolicitudAusenciaModel());
        }

        // POST: Ausencias/SolicitudAusencia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SolicitudAusencia(SolicitudAusenciaModel model)
        {
            if (ModelState.IsValid)
            {
                model.IdUsuario = (int)Session["IdUsuario"];

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
                return RedirectToAction("SolicitudAusencia");
            }

            return View(model);
        }
    }
}
