using Newtonsoft.Json;
using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    public class Registro_UsuariosController : Controller
    {

        [HttpGet]
        public ActionResult registro_usuarios()
        {
            return View();
        }

   



        // Craer cuenta simple
        [HttpPost]
        public ActionResult crear_cuenta(UsuarioModel model)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                var result = context.sp_crear_cuenta(model.nombre, model.correo, model.contrasenna);

                if (result > 0)
                {
                    return RedirectToAction("registro_usuarios", "Registro_Usuarios");
                }

                // Si llega aquí es porque hubo un error
                TempData["ErrorMessage"] = "No se pudo completar el registro. Por favor intente nuevamente";
                return RedirectToAction("registro_usuarios", "Registro_Usuarios");
            }
        }
        [HttpPost]
        public ActionResult iniciar_sesion(UsuarioModel model)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                var info = context.sp_autenticar_usuario(model.correo, model.contrasenna).FirstOrDefault();

                if (info != null)
                {
                    if (info.Resultado == 1)
                    {
                        Session["IdUsuario"] = info.UsuarioID;
                        Session["NombreUsuario"] = info.NombreUsuario;
                        Session["Rol"] = info.RolID;
                        return RedirectToAction("inicio", "Inicio");
                    }

                    if (info.Resultado == 0)
                    {
                        ViewBag.Mensaje = "Credenciales incorrectas. Por favor verifique sus datos.";
                        return View(model);
                    }

                    if (info.Resultado == -1)
                    {
                        ViewBag.Mensaje = "Usuario inactivo.";
                        return View(model);
                    }
                }

                // Si info es null
                ViewBag.Mensaje = "No se pudo validar el usuario. Intente de nuevo.";
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult cerrar_sesion()
        {
            Session.Abandon();  // Finaliza la sesión
            return RedirectToAction("registro_usuarios", "Registro_Usuarios");
        }



    }
}
