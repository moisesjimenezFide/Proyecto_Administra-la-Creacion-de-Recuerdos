using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Filters;
using Proyecto_CreandoRecuerdos.Helpers;
using Proyecto_CreandoRecuerdos.Models;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    // Evitar el almacenamiento en caché de las vistas
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]

    public class Registro_UsuariosController : Controller
    {
        Utilitarios util = new Utilitarios();

        [HttpGet]
        public ActionResult logout()
        {
            // 🔹 Borrar todas las variables de sesión
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

            // 🔹 Eliminar cookie de sesión normal (por compatibilidad)
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                var sessionCookie = new HttpCookie("ASP.NET_SessionId", "")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(sessionCookie);
            }

            // 🔹 Eliminar cookie del JWT
            if (Request.Cookies["JWT"] != null)
            {
                var jwtCookie = new HttpCookie("JWT", "")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(jwtCookie);
            }

            // 🔹 Desactivar caché del navegador
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            // 🔹 Redirigir al login o registro
            return RedirectToAction("iniciar_sesion", "Registro_Usuarios");
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

                if (info == null)
                {
                    TempData["ErrorMessage"] = "No se pudo validar el usuario. Intente de nuevo.";
                    return RedirectToAction("registro_usuarios", "Registro_Usuarios");
                }

                if (info != null)
                {
                    if (info.Resultado == 1)
                    {
                        // 🟢 Crear token JWT con los datos del usuario
                        var token = JwtManager.GenerateToken(
                            info.NombreUsuario,
                            info.RolID.ToString(),
                            1 // minutos de expiración
                        );

                        // 🟢 Guardar el token en una cookie
                        var cookie = new HttpCookie("JWT", token)
                        {
                            HttpOnly = true, // evita acceso por JS
                            Secure = true,   // solo HTTPS
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTime.UtcNow.AddMinutes(1)
                        };
                        Response.Cookies.Add(cookie);

                        // opcionalmente también enviar el header (para AJAX)
                        Response.Headers.Add("X-Renewed-Token", token);


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
        [RolAuthorize("1")]
        public ActionResult gestion_usuarios(string search = null, string fecha = null)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                var usuarios = context.sp_obtener_usuarios().ToList();

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

                // Filtrar por búsqueda si se proporciona
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var filtro = search.ToLower();
                    listaUsuarios = listaUsuarios.Where(u =>
                        u.nombre.ToLower().Contains(filtro) ||
                        u.correo.ToLower().Contains(filtro) ||
                        u.id.ToString().Contains(filtro) ||
                        (u.activo ? "activo" : "inactivo").Contains(filtro)
                    ).ToList();
                }

                // Filtrar por fecha si se proporciona
                if (!string.IsNullOrWhiteSpace(fecha))
                {
                    DateTime fechaBuscada;
                    var formatos = new[] { "yyyy-MM-dd", "dd/MM/yyyy", "dd-MM-yyyy" };
                    if (DateTime.TryParseExact(fecha, formatos, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fechaBuscada))
                    {
                        listaUsuarios = listaUsuarios
                            .Where(u => u.fecha_creacion.Date.Equals(fechaBuscada.Date))
                            .ToList();
                    }
                }

                ViewBag.Search = search;
                ViewBag.Fecha = fecha;
                return View(listaUsuarios);
            }
        }

        [HttpPost]
        [RolAuthorize("1")]
        public ActionResult inactivar_usuarios(int id)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                context.sp_inactivar_usuario(id); 
            }
            return Json(new { success = true });
        }

        [HttpPost]
        [RolAuthorize("1")]
        public ActionResult activar_usuarios(int id)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                context.sp_activar_usuario(id);
            }
            return Json(new { success = true });
        }

        [HttpGet]
        [RolAuthorize("1")]
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
        [RolAuthorize("1")]
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
                        var notificacion = util.EnviarCorreo(info.correo, mensaje, "Acceso a Creando Recuerdos");

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
