using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Models;
using System;
using System.Linq;
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

        // Crear cuenta simple
        [HttpPost]
        public ActionResult crear_cuenta(UsuarioModel model)
        {
            try
            {
                using (var context = new BD_CREANDO_RECUERDOSEntities())
                {
                    var result = context.sp_crear_cuenta(model.nombre, model.correo, model.contrasenna, model.telefono);

                    if (result > 0)
                    {
                        TempData["SuccessMessage"] = "Registro exitoso. Por favor inicie sesión.";
                        return RedirectToAction("registro_usuarios");
                    }

                    TempData["ErrorMessage"] = "No se pudo completar el registro. Por favor intente nuevamente";
                    return RedirectToAction("registro_usuarios");
                }
            }
            catch (Exception ex)
            {
                // Log the error
                TempData["ErrorMessage"] = "Ocurrió un error inesperado durante el registro.";
                return RedirectToAction("registro_usuarios");
            }
        }

        [HttpPost]
        public ActionResult iniciar_sesion(UsuarioModel model)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                var info = context.sp_autenticar_usuario(model.correo, model.contrasenna).FirstOrDefault();

                if (info != null && info.Resultado == 1 && info.UsuarioID.HasValue)
                {
                    // Configurar sesión
                    Session["UserId"] = info.UsuarioID.Value;
                    Session["NombreUsuario"] = info.NombreUsuario;
                    Session["Rol"] = info.RolID;

                    // Manejo especial para clientes
                    if (info.RolID == 3) // ID para clientes
                    {
                        int clienteId = CrearOActualizarCliente(context, info.UsuarioID, info.NombreUsuario);
                        Session["ClienteId"] = clienteId;
                    }

                    return RedirectToAction("inicio", "Inicio");
                }

                TempData["ErrorMessage"] = "No se pudo validar el usuario. Intente de nuevo.";
                return RedirectToAction("registro_usuarios");
            }
        }


        private int CrearOActualizarCliente(BD_CREANDO_RECUERDOSEntities context, int? usuarioId, string nombreUsuario)
        {
            if (!usuarioId.HasValue)
            {
                throw new ArgumentException("El ID de usuario no puede ser nulo");
            }

            var cliente = context.tabla_clientes.FirstOrDefault(c => c.id_usuario == usuarioId.Value);

            if (cliente == null)
            {
                cliente = new tabla_clientes
                {
                    nombre = nombreUsuario,
                    apellido = "",
                    telefono = "",
                    id_usuario = usuarioId.Value
                };
                context.tabla_clientes.Add(cliente);
                context.SaveChanges();
            }

            var pedidosIncorrectos = context.tabla_ventas
                .Where(v => v.id_usuario == usuarioId.Value && v.id_cliente != cliente.id_cliente)
                .ToList();

            foreach (var pedido in pedidosIncorrectos)
            {
                pedido.id_cliente = cliente.id_cliente;
            }

            if (pedidosIncorrectos.Any())
            {
                context.SaveChanges();
            }

            return cliente.id_cliente;
        }

        [HttpGet]
        public ActionResult cerrar_sesion()
        {
            Session.Abandon();  // Finaliza la sesión
            return RedirectToAction("registro_usuarios");
        }

        [HttpGet]
        public ActionResult gestion_usuarios()
        {
            if (Session["Rol"]?.ToString() != "1") // Solo admin puede acceder
            {
                return RedirectToAction("inicio", "Inicio");
            }

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
                    contrasenna = u.contrasenna,
                    id_cliente = context.tabla_clientes.FirstOrDefault(c => c.id_usuario == u.id_usuario)?.id_cliente
                }).ToList();

                return View(listaUsuarios);
            }
        }

        [HttpPost]
        public ActionResult inactivar_usuarios(int id)
        {
            if (Session["Rol"]?.ToString() != "1")
            {
                return Json(new { success = false, message = "No autorizado" });
            }

            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                context.sp_inactivar_usuario(id);
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult activar_usuarios(int id)
        {
            if (Session["Rol"]?.ToString() != "1")
            {
                return Json(new { success = false, message = "No autorizado" });
            }

            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                context.sp_activar_usuario(id);
            }
            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult editar_usuario(int id)
        {
            if (Session["Rol"]?.ToString() != "1")
            {
                return RedirectToAction("inicio", "Inicio");
            }

            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                var usuario = context.sp_obtener_usuarios().FirstOrDefault(u => u.id_usuario == id);
                if (usuario == null)
                    return HttpNotFound();

                var roles = context.sp_obtener_roles().ToList();
                var cliente = context.tabla_clientes.FirstOrDefault(c => c.id_usuario == id);

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
                    }).ToList(),
                    id_cliente = cliente?.id_cliente,
                    apellido = cliente?.apellido,
                    telefono = cliente?.telefono
                };

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult editar_usuario(UsuarioModel model)
        {
            if (Session["Rol"]?.ToString() != "1")
            {
                return RedirectToAction("inicio", "Inicio");
            }

            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                // Actualizar usuario
                context.sp_actualizar_usuario((int)model.id, model.nombre, model.id_rol);

                // Si es cliente, actualizar información del cliente
                if (model.id_rol == 3 && model.id_cliente.HasValue)
                {
                    var cliente = context.tabla_clientes.Find(model.id_cliente.Value);
                    if (cliente != null)
                    {
                        cliente.nombre = model.nombre;
                        cliente.apellido = model.apellido ?? string.Empty;
                        cliente.telefono = model.telefono;
                        context.SaveChanges();
                    }
                }
            }
            return RedirectToAction("gestion_usuarios");
        }
    }
}