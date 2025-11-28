// HorariosController.cs
using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using Proyecto_CreandoRecuerdos.Models;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    // Evitar el almacenamiento en caché de las vistas
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]

    [RolAuthorize("1")]
    public class HorariosController : Controller
    {
        [HttpGet]
        public ActionResult Horarios()
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                var horarios = context.tabla_horarios
                    .Join(context.tabla_usuarios,
                    h => h.id_usuario,
                    u => u.id_usuario,
                    (h, u) => new HorarioModel
                    {
                        id_horario = h.id_horario,
                        id_usuario = h.id_usuario,
                        dia_semana = h.dia_semana,
                        hora_entrada = h.hora_entrada,
                        hora_salida = h.hora_salida,
                        nombre_usuario = u.nombre
                    })
                    .ToList();

                ViewBag.Empleados = context.tabla_usuarios
                    .Where(u => u.id_rol != 3)
                    .Select(u => new SelectListItem
                    {
                        Value = u.id_usuario.ToString(),
                        Text = u.nombre
                    }).ToList();

                return View(horarios);
            }
        }

        [HttpPost]
        public ActionResult GuardarHorario(int? id_usuario, string[] dias_semana, TimeSpan? hora_entrada, TimeSpan? hora_salida)
        {
            if (id_usuario == null || dias_semana == null || hora_entrada == null || hora_salida == null)
            {
                ModelState.AddModelError("", "Todos los campos son obligatorios.");
                return RedirectToAction("Horarios");
            }

            if (dias_semana == null)
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un día de la semana.");
                // Puedes retornar la vista de horarios, pasando los datos necesarios si lo requieres
                return RedirectToAction("Horarios");
            }

            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                foreach (var dia in dias_semana)
                {
                    var horario = new tabla_horarios
                    {
                        id_usuario = id_usuario.Value,
                        dia_semana = dia,
                        hora_entrada = hora_entrada.Value,
                        hora_salida = hora_salida.Value,
                        activo = true
                    };

                    context.tabla_horarios.Add(horario);
                }

                try
                {
                    context.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException)
                {
                    throw;
                }

                return RedirectToAction("Horarios");
            }
        }

        [HttpPost]
        public ActionResult EliminarHorario(int id)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                var horario = context.tabla_horarios.Find(id);
                if (horario == null) 
                    return View("NoEncontrado404");

                context.tabla_horarios.Remove(horario);
                context.SaveChanges();
                return RedirectToAction("Horarios");
            }
        }
    }
}
