using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Models;
using System;
using System.Linq;
using System.Text;
using System.util;
using System.Web.Mvc;
using Proyecto_CreandoRecuerdos.Filters;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    public class Registro_UsuariosController : Controller
    {
        Utilitarios util = new Utilitarios();

        [HttpGet]
        public ActionResult registro_usuarios()
        {
            return View();
        }

        [HttpGet]
        public ActionResult iniciar_sesion()
        {
            return View();
        }

        // Craer cuenta simple
        [HttpPost]
        public ActionResult crear_cuenta(UsuarioModel model)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                // Ejecutamos el stored procedure (versión que solo retorna valores numéricos)
                var resultado = context.sp_crear_cuenta(
                    model.nombre,
                    model.correo,
                    model.contrasenna
                );

                // Manejamos los diferentes códigos de retorno
                switch (resultado)
                {
                    case 1: // Éxito
                        TempData["SuccessMessage"] = "¡Cuenta creada exitosamente!";
                        return RedirectToAction("inicio", "Inicio");

                    case 0: // Correo ya existe
                        TempData["ErrorMessage"] = "El correo electrónico ya está registrado";
                        return RedirectToAction("registro_usuarios", "Registro_Usuarios");

                    default: // Otros errores
                        TempData["ErrorMessage"] = "Error al crear la cuenta. Por favor intente nuevamente";
                        return RedirectToAction("registro_usuarios", "Registro_Usuarios");
                }
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
                        TempData["ErrorMessage"] = "Credenciales incorrectas. Por favor verifique sus datos.";
                        return RedirectToAction("registro_usuarios", "Registro_Usuarios");
                    }

                    if (info.Resultado == -1)
                    {
                        TempData["ErrorMessage"] = "Usuario inactivo.";
                        return RedirectToAction("registro_usuarios", "Registro_Usuarios");
                    }
                }

                // Si info es null
                TempData["ErrorMessage"] = "No se pudo validar el usuario. Intente de nuevo.";
                return RedirectToAction("registro_usuarios", "Registro_Usuarios");
            }
        }

        [HttpGet]
        public ActionResult cerrar_sesion()
        {
            Session.Abandon();  // Finaliza la sesión
            return RedirectToAction("registro_usuarios", "Registro_Usuarios");
        }

        [AutorizacionFilter(1)]
        //1.Admin
        //2.Trabajador
        //3.Cliente
        //[AutorizacionFilter(1, 2)] 

        [HttpGet]
        public ActionResult gestion_usuarios()
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                var usuarios = context.sp_obtener_usuarios().ToList();

                // Mapear los datos manualmente
                var listaUsuarios = usuarios.Select(u => new UsuarioModel
                {
                    id = u.id_usuario,
                    nombre = u.nombre,
                    id_rol = u.id_rol,
                    correo = u.correo,
                    activo = (bool)u.activo,
                    fecha_creacion = (DateTime)u.fecha_creacion,
                    contrasenna = u.contrasenna
                }).ToList();

                return View(listaUsuarios);
            }
        }


        [HttpPost]
        public ActionResult inactivar_usuarios(int id)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                context.sp_inactivar_usuario(id); 
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult activar_usuarios(int id)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                context.sp_activar_usuario(id);
            }
            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult editar_usuario(int id)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                var usuario = context.sp_obtener_usuarios().FirstOrDefault(u => u.id_usuario == id);
                if (usuario == null)
                    return HttpNotFound();

                // Aquí llamas al procedimiento para obtener roles
                var roles = context.sp_obtener_roles().ToList();

                var model = new UsuarioModel
                {
                    id = usuario.id_usuario,
                    nombre = usuario.nombre,
                    correo = usuario.correo,
                    id_rol = usuario.id_rol,
                    RolesDisponibles = roles.Select(r => new SelectListItem
                    {
                        Value = r.id_rol.ToString(),
                        Text = r.nombre
                    }).ToList()
                };

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult editar_usuario(UsuarioModel model)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                int idUsuario = (int)model.id; // convertir long a int

                context.sp_actualizar_usuario(idUsuario, model.nombre, model.id_rol);
            }
            return RedirectToAction("gestion_usuarios");
        }


        // Metodo para cambiar la contrasenna
        [HttpGet]
        public ActionResult recuperar_contrasenna()
        {
            return View();
        }

        // Metodo para cambiar la contrasenna
        [HttpPost]
        public ActionResult recuperar_contrasenna(UsuarioModel model)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
                {
                    var info = context.tabla_usuarios.Where(x => x.correo == model.correo
                                                       && x.activo == true).FirstOrDefault();
                    if (info != null)
                    {
                        var codigoTemporal = CrearCodigo();
                        info.contrasenna = codigoTemporal;
                        context.SaveChanges();

                        string mensaje = $"Hola {info.nombre}, por favor utilice el siguiente código para ingresar al sistema: {codigoTemporal}";
                        var notificacion = util.EnviarCorreo(info.correo, mensaje, "Acceso a DataGym");

                        if (notificacion)
                            return RedirectToAction("Login", "Usuario");
                    }

                    ViewBag.Mensaje = "Su acceso no se ha podido reestablecer correctamente";
                    return View(model);
                }
        }

        private string CrearCodigo()
        {
            int length = 5;
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }




    }
}
