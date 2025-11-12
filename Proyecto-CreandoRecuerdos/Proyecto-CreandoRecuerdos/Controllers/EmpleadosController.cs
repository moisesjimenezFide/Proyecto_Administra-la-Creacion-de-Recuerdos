using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Filters;
using Proyecto_CreandoRecuerdos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    // Evitar el almacenamiento en caché de las vistas
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]

    [RolAuthorize("1")]
    public class EmpleadosController : Controller
    {
        [HttpGet]
        public ActionResult Empleados()
        {

            List<EmpleadoModel> empleados = new List<EmpleadoModel>();
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                empleados = context.tabla_usuarios
                    .Where(u => u.id_rol != 3)
                    .Select(u => new EmpleadoModel
                    {
                        id_usuario = u.id_usuario,
                        nombre = u.nombre,
                        idRol = u.id_rol,
                        correo = u.correo,
                        activo = u.activo ?? true,
                        fecha_creacion = u.fecha_creacion ?? DateTime.Now
                    })
                    .ToList();
            }
            return View(empleados);
        }

        [HttpGet]
        public ActionResult ObtenerEmpleado(int id)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                var empleado = context.tabla_usuarios
                    .Where(u => u.id_usuario == id && u.id_rol != 3)
                    .Select(u => new EmpleadoModel
                    {
                        id_usuario = u.id_usuario,
                        nombre = u.nombre,
                        idRol = u.id_rol,
                        correo = u.correo,
                        activo = u.activo ?? true
                    })
                    .FirstOrDefault();

                if (empleado == null)
                {
                    return HttpNotFound();
                }

                return Json(empleado, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult RegistrarEmpleado(EmpleadoModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage)
                                                .ToList();
                    TempData["ErrorMessage"] = "Errores de validación: " + string.Join(", ", errors);
                    return RedirectToAction("Empleados");
                }

                using (var context = new BD_CREANDO_RECUERDOSEntities())
                {
                    // Verificar si el rol existe
                    if (!context.tabla_roles.Any(r => r.id_rol == model.idRol))
                    {
                        TempData["ErrorMessage"] = "El rol seleccionado no existe.";
                        return RedirectToAction("Empleados");
                    }

                    // Verificar si el correo ya existe
                    if (context.tabla_usuarios.Any(u => u.correo == model.correo))
                    {
                        TempData["ErrorMessage"] = "El correo electrónico ya está registrado.";
                        return RedirectToAction("Empleados");
                    }

                    var nuevoEmpleado = new tabla_usuarios
                    {
                        nombre = model.nombre,
                        id_rol = model.idRol,
                        correo = model.correo,
                        contrasenna = model.contrasenna ?? "temporal123",
                        activo = model.activo,
                        fecha_creacion = DateTime.Now
                    };

                    context.tabla_usuarios.Add(nuevoEmpleado);
                    context.SaveChanges();

                    TempData["SuccessMessage"] = "Empleado registrado exitosamente.";
                    return RedirectToAction("Empleados");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al registrar empleado: {ex.Message}";
                return RedirectToAction("Empleados");
            }
        }

        [HttpPost]
        public ActionResult ActualizarEmpleado(EmpleadoModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Por favor complete todos los campos requeridos.";
                    return RedirectToAction("Empleados");
                }

                using (var context = new BD_CREANDO_RECUERDOSEntities())
                {
                    var empleado = context.tabla_usuarios.Find(model.id_usuario);

                    if (empleado == null || empleado.id_rol == 3)
                    {
                        TempData["ErrorMessage"] = "Empleado no encontrado o es cliente.";
                        return RedirectToAction("Empleados");
                    }

                    // Verificar si el rol existe
                    if (!context.tabla_roles.Any(r => r.id_rol == model.idRol))
                    {
                        TempData["ErrorMessage"] = "El rol seleccionado no existe.";
                        return RedirectToAction("Empleados");
                    }

                    // Verificar si el correo ya existe 
                    if (context.tabla_usuarios.Any(u => u.correo == model.correo && u.id_usuario != model.id_usuario))
                    {
                        TempData["ErrorMessage"] = "El correo electrónico ya está registrado por otro usuario.";
                        return RedirectToAction("Empleados");
                    }

                    empleado.nombre = model.nombre;
                    empleado.id_rol = model.idRol;
                    empleado.correo = model.correo;
                    empleado.activo = model.activo;

                    if (!string.IsNullOrEmpty(model.contrasenna))
                    {
                        empleado.contrasenna = model.contrasenna;
                    }

                    context.SaveChanges();

                    TempData["SuccessMessage"] = "Empleado actualizado exitosamente.";
                    return RedirectToAction("Empleados");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al actualizar empleado: {ex.Message}";
                return RedirectToAction("Empleados");
            }
        }

        [HttpPost]
        public ActionResult EliminarEmpleado(int id)
        {
            try
            {
                using (var context = new BD_CREANDO_RECUERDOSEntities())
                {
                    var empleado = context.tabla_usuarios.Find(id);

                    if (empleado == null || empleado.id_rol == 3)
                    {
                        TempData["ErrorMessage"] = "Empleado no encontrado o es cliente.";
                        return RedirectToAction("Empleados");
                    }

                    if (context.tabla_ventas.Any(v => v.id_usuario == id))
                    {
                        empleado.activo = false;
                        context.SaveChanges();
                        TempData["SuccessMessage"] = "El empleado tiene registros asociados, se ha marcado como inactivo.";
                    }
                    else
                    {
                        context.tabla_usuarios.Remove(empleado);
                        context.SaveChanges();
                        TempData["SuccessMessage"] = "Empleado eliminado exitosamente.";
                    }

                    return RedirectToAction("Empleados");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar empleado: {ex.Message}";
                return RedirectToAction("Empleados");
            }
        }

        [HttpGet]
        public ActionResult VerificarCorreo(string correo, int idUsuario = 0)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                bool existeCorreo = context.tabla_usuarios
                    .Any(u => u.correo == correo && u.id_usuario != idUsuario);

                return Json(new { isUnique = !existeCorreo }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}