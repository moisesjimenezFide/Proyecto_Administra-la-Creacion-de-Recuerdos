using System.Linq;
using System.Web.Mvc;
using Proyecto_CreandoRecuerdos.Models;
using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Filters;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    // Evitar el almacenamiento en caché de las vistas
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]

    public class AusenciasController : Controller
    {
        
        // GET: Ausencias
        private BD_CREANDO_RECUERDOSEntities db = new BD_CREANDO_RECUERDOSEntities();

        [RolAuthorize("1")]
        public ActionResult GestionSolicitudesAusencias()
        {
            var solicitudes = db.tabla_solicitudes_ausencias
                .Join(db.tabla_usuarios,
                s => s.id_usuario,
                u => u.id_usuario,
                (s, u) => new SolicitudAusenciaModel
                {
                    IdSolicitud = s.id_solicitud,
                    IdUsuario = s.id_usuario,
                    NombreUsuario = u.nombre,
                    FechaInicio = s.fecha_inicio,
                    FechaFin = s.fecha_fin,
                    Tipo = s.tipo,
                    Motivo = s.motivo,
                    Estado = s.estado
                })
                .ToList();

            return View(solicitudes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RolAuthorize("1")]
        public ActionResult Aprobar(int id)
        {
            var solicitud = db.tabla_solicitudes_ausencias.Find(id);
            if (solicitud != null && solicitud.estado == "Pendiente")
            {
                solicitud.estado = "Aprobado";
                db.SaveChanges();
                TempData["SuccessMessage"] = "Solicitud aprobada correctamente.";
            }
            return RedirectToAction("GestionSolicitudesAusencias");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RolAuthorize("1")]
        public ActionResult Rechazar(int id)
        {
            var solicitud = db.tabla_solicitudes_ausencias.Find(id);
            if (solicitud != null && solicitud.estado == "Pendiente")
            {
                solicitud.estado = "Rechazado";
                db.SaveChanges();
                TempData["SuccessMessage"] = "Solicitud rechazada correctamente.";
            }
            return RedirectToAction("GestionSolicitudesAusencias");
        }

        // GET: Ausencias/SolicitudAusencia
        [RolAuthorize("2")]
        public ActionResult SolicitudAusencia()
        {
            if (Session["IdUsuario"] == null)
            {
                // Redirige a Acceso Denegado
                return RedirectToAction("AccesoDenegado", "Inicio");
            }

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
        [RolAuthorize("2")]
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

                TempData["SuccessMessage"] = "Solicitud enviada correctamente.";
                return RedirectToAction("SolicitudAusencia");
            }

            return View(model);
        }
    }
}
