using DocumentFormat.OpenXml.EMMA;
using Newtonsoft.Json;
using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Filters;
using Proyecto_CreandoRecuerdos.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    // Evitar el almacenamiento en caché de las vistas
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]

    public class PedidosController : Controller
    {
        // Acción principal que muestra la vista de pedidos con productos y categorías
        [HttpGet]
        public ActionResult Pedidos()
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                // Obtener IDs de productos recomendados
                var productosRecomendadosIds = db.tabla_recomendaciones
                                              .Select(r => r.id_producto)
                                              .ToList();

                // Consultar todos los productos y marcar cuáles son recomendados
                var productos = db.tabla_productos
                    .Select(p => new ProductoModel
                    {
                        id_producto = p.id_producto,
                        nombre = p.nombre,
                        descripcion = p.descripcion,
                        id_categoria = p.id_categoria,
                        precio_por_unidad = p.precio_por_unidad,
                        img_url = p.img_url,
                        EsRecomendado = productosRecomendadosIds.Contains(p.id_producto)
                    })
                    .ToList();

                foreach (var producto in productos)
                {
                    var imgPath = Server.MapPath($"~/Templates/img/menu/{producto.img_url}");
                    if (string.IsNullOrEmpty(producto.img_url) || !System.IO.File.Exists(imgPath))
                    {
                        producto.img_url = "default.png";
                    }
                }

                // Obtener categorías para el filtrado
                var categorias = db.tabla_categorias.ToList();

                // Pasar datos a la vista
                ViewBag.Categorias = categorias;
                ViewBag.ProductosRecomendados = productosRecomendadosIds;
                ViewBag.ProductosJson = JsonConvert.SerializeObject(productos);

                return View(productos);
            }
        }

        // Obtener todos los productos en formato JSON
        [HttpGet]
        public JsonResult ObtenerProductos()
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var productos = db.tabla_productos
                    .Select(p => new {
                        id_producto = p.id_producto,
                        nombre = p.nombre,
                        descripcion = p.descripcion,
                        id_categoria = p.id_categoria,
                        precio_por_unidad = p.precio_por_unidad,
                        img_url = p.img_url
                    })
                    .ToList();

                return Json(productos, JsonRequestBehavior.AllowGet);
            }
        }

        // Obtener productos filtrados por categoría
        [HttpGet]
        public JsonResult ObtenerProductosPorCategoria(int idCategoria)
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var productos = db.tabla_productos
                    .Where(p => p.id_categoria == idCategoria)
                    .Select(p => new {
                        id_producto = p.id_producto,
                        nombre = p.nombre,
                        descripcion = p.descripcion,
                        id_categoria = p.id_categoria,
                        precio_por_unidad = p.precio_por_unidad,
                        img_url = p.img_url
                    })
                    .ToList();

                return Json(productos, JsonRequestBehavior.AllowGet);
            }
        }

        // Cancelar un pedido existente
        [HttpPost]
        public JsonResult CancelarPedido(int idPedido)
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    int idUsuario = GetUserId();
                    // Buscar el pedido del usuario actual
                    var pedido = db.tabla_ventas
                        .Include(v => v.tabla_estados_pedido)
                        .FirstOrDefault(v => v.id_venta == idPedido && v.id_usuario == idUsuario);

                    if (pedido == null)
                        return Json(new { success = false, message = "Pedido no encontrado" });

                    // Validar que el pedido esté en estado cancelable
                    if (!new[] { "Pendiente", "En preparación" }.Contains(pedido.tabla_estados_pedido.nombre))
                        return Json(new { success = false, message = "No se puede cancelar en estado actual" });

                    // Cambiar estado a Cancelado
                    var estadoCancelado = db.tabla_estados_pedido.FirstOrDefault(e => e.nombre == "Cancelado");
                    if (estadoCancelado == null)
                        return Json(new { success = false, message = "Estado no configurado" });

                    pedido.id_estado = estadoCancelado.id_estado;
                    pedido.notificacion = "Pedido cancelado por el cliente";
                    pedido.fecha_actualizacion = DateTime.Now;

                    db.SaveChanges();

                    return Json(new { success = true, message = "Pedido cancelado exitosamente" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // Vista de gestión de pedidos (solo para administradores)
        [HttpGet]
        [RolAuthorize("1")]
        public ActionResult Gestionar()
        {
            int userId = GetUserId();

            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                // Validar que el usuario sea admin
                var user = db.tabla_usuarios.Find(userId);
                if (user == null || (user.id_rol != 1 && user.id_rol != 2))
                {
                    return RedirectToAction("Pedidos", "Pedidos");
                }
            }

            return View();
        }

        // Obtener pedidos con filtros y paginación
        [HttpGet]
        [RolAuthorize("1")]
        public JsonResult ObtenerPedidos(string estado = null, string fechaInicio = null, string fechaFin = null,
    string metodoPago = null)
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    DateTime? fechaDesde = null;
                    DateTime? fechaHasta = null;

                    // Parsear fechas de filtro
                    if (!string.IsNullOrEmpty(fechaInicio))
                        fechaDesde = DateTime.Parse(fechaInicio);

                    if (!string.IsNullOrEmpty(fechaFin))
                        fechaHasta = DateTime.Parse(fechaFin).AddDays(1);

                    // Consulta base con includes
                    var query = db.tabla_ventas
                        .Include(v => v.tabla_clientes)
                        .Include(v => v.tabla_estados_pedido)
                        .Include(v => v.tabla_detalle_venta)
                        .AsQueryable();

                    // Aplicar filtros
                    if (!string.IsNullOrEmpty(estado))
                    {
                        query = query.Where(p => p.tabla_estados_pedido.nombre == estado);
                    }

                    if (fechaDesde.HasValue)
                    {
                        query = query.Where(p => p.fecha >= fechaDesde.Value);
                    }

                    if (fechaHasta.HasValue)
                    {
                        query = query.Where(p => p.fecha < fechaHasta.Value);
                    }

                    if (!string.IsNullOrEmpty(metodoPago))
                    {
                        query = query.Where(p => p.metodo_pago == metodoPago);
                    }

                    // Paginación
                    int totalRegistros = query.Count();

                    var pedidosIds = query
                        .OrderByDescending(p => p.fecha)
                        .Select(p => p.id_venta)
                        .ToList();

                    // Obtener todos los pedidos filtrados y ordenados
                    var pedidos = query
                        .OrderByDescending(p => p.fecha)
                        .Include(p => p.tabla_clientes)
                        .Include(p => p.tabla_estados_pedido)
                        .Include(p => p.tabla_detalle_venta)
                        .AsEnumerable()
                        .Select(p => new {
                            id_pedido = p.id_venta,
                            id_cliente = p.id_cliente,
                            numero_pedido = p.numero_pedido ?? $"ORD-{p.fecha.Value.Year}-{p.id_venta.ToString().PadLeft(4, '0')}",
                            nombre_cliente = (p.id_usuario == null && !string.IsNullOrEmpty(p.nombre_cliente)) ? p.nombre_cliente : p.tabla_clientes.nombre + " " + p.tabla_clientes.apellido,
                            telefono = p.tabla_clientes.telefono ?? "N/A",
                            cantidad_productos = p.tabla_detalle_venta.Count,
                            fecha = p.fecha?.ToString("yyyy-MM-dd HH:mm") ?? "N/A",
                            fecha_fin = (p.tabla_estados_pedido.nombre == "Entregado" && p.fecha_actualizacion != null) ?
                                p.fecha_actualizacion.Value.ToString("yyyy-MM-dd HH:mm") : null,
                            total = p.total,
                            estado = p.tabla_estados_pedido.nombre,
                            metodo_pago = p.metodo_pago ?? "N/A",
                            para_llevar = p.para_llevar,
                            pin = p.pin,
                            tiempo_estimado = p.tiempo_estimado ?? 20,
                            minutos_transcurridos = (int)(DateTime.Now - (p.fecha ?? DateTime.Now)).TotalMinutes
                        })
                        .ToList();

                    return Json(new
                    {
                        pedidos = pedidos,
                        totalRegistros = pedidos.Count
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Obtener pedidos con notificaciones para el usuario actualmente
        [HttpGet]
        public JsonResult ObtenerPedidosConNotificaciones()
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    int userId = GetUserId();
                    var cliente = db.tabla_clientes.FirstOrDefault(c => c.id_usuario == userId);

                    if (cliente == null)
                    {
                        return Json(new { notificaciones = new List<object>() }, JsonRequestBehavior.AllowGet);
                    }

                    // Llamar a stored procedure para obtener notificaciones
                    var notificaciones = db.Database.SqlQuery<NotificacionPedidoModel>(
                        "EXEC sp_obtener_pedidos_con_notificaciones @id_usuario",
                        new SqlParameter("@id_usuario", userId)
                    ).ToList();

                    return Json(new { notificaciones }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Mostrar detalle de un pedido específico
        [HttpGet]
        public ActionResult DetallePedido(int id)
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var pedido = db.tabla_ventas
                    .Include(v => v.tabla_clientes)
                    .Include(v => v.tabla_estados_pedido)
                    .Include(v => v.tabla_detalle_venta.Select(d => d.tabla_productos))
                    .FirstOrDefault(v => v.id_venta == id);

                if (pedido == null)
                {
                    return HttpNotFound();
                }

                // Determinar fecha de finalización si está entregado
                var fechaFin = pedido.id_estado == db.tabla_estados_pedido.First(e => e.nombre == "Entregado").id_estado
                    ? pedido.fecha_actualizacion
                    : (DateTime?)null;

                // Crear modelo para la vista
                var model = new DetallePedidoModel
                {
                    IdPedido = pedido.id_venta,
                    NumeroPedido = pedido.numero_pedido ?? $"ORD-{pedido.fecha.Value.Year}-{pedido.id_venta.ToString("D4")}",
                    NombreCliente = !string.IsNullOrEmpty(pedido.nombre_cliente) ? pedido.nombre_cliente : pedido.tabla_clientes.nombre + " " + pedido.tabla_clientes.apellido,
                    TelefonoCliente = pedido.tabla_clientes.telefono,
                    TelefonoPedido = pedido.telefono,
                    TelefonoSinpe = pedido.telefono_sinpe,
                    MetodoPago = pedido.metodo_pago ?? "No especificado",
                    ParaLlevar = pedido.para_llevar ?? false,
                    Fecha = pedido.fecha.Value,
                    FechaFin = fechaFin,
                    Estado = pedido.tabla_estados_pedido.nombre,
                    Pin = pedido.pin,
                    TiempoEstimado = pedido.tiempo_estimado ?? 20,
                    Subtotal = pedido.tabla_detalle_venta.Sum(d => d.cantidad * d.precio_unitario),
                    Total = pedido.total,
                    Productos = pedido.tabla_detalle_venta.Select(d => new ProductoPedidoModel
                    {
                        IdProducto = d.id_producto,
                        Nombre = d.tabla_productos.nombre,
                        Cantidad = d.cantidad,
                        PrecioUnitario = d.precio_unitario,
                        Personalizacion = !string.IsNullOrEmpty(d.personalizacion) ? d.personalizacion : "Ninguna"
                    }).ToList(),
                    PuedeActualizarEstado = true,
                    EstadosDisponibles = db.tabla_estados_pedido.Select(e => e.nombre).ToList()
                };

                // Ajustar datos para pago con Sinpe
                if (pedido.metodo_pago.Equals("Sinpe", StringComparison.OrdinalIgnoreCase))
                {
                    model.TelefonoPedido = null;
                }

                // Calcular impuestos
                model.Impuestos = model.Total - model.Subtotal;

                return View(model);
            }
        }

        // Actualizar estado de un pedido
        [HttpPost]
        public ActionResult ActualizarEstado(int idPedido, string nuevoEstado)
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    int idUsuario = GetUserId();
                    var pedido = db.tabla_ventas
                        .Include(p => p.tabla_estados_pedido)
                        .FirstOrDefault(p => p.id_venta == idPedido);

                    if (pedido == null)
                    {
                        TempData["ErrorMessage"] = "Pedido no encontrado";
                        return RedirectToAction("DetallePedido", new { id = idPedido });
                    }

                    // Validar nuevo estado
                    var estado = db.tabla_estados_pedido.FirstOrDefault(e => e.nombre == nuevoEstado);
                    if (estado == null)
                    {
                        TempData["ErrorMessage"] = "Estado no válido";
                        return RedirectToAction("DetallePedido", new { id = idPedido });
                    }

                    // Generar mensaje de notificación automático
                    string mensajeNotificacion = GenerarMensajeNotificacion(pedido, nuevoEstado);

                    // Actualizar estado y notificación
                    pedido.id_estado = estado.id_estado;
                    pedido.notificacion = mensajeNotificacion;
                    pedido.fecha_actualizacion = DateTime.Now;

                    db.SaveChanges();

                    // Registrar actividad
                    db.sp_registrar_actividad(
                        idUsuario,
                        "UPDATE",
                        $"Actualizado estado del pedido {idPedido} a {nuevoEstado}"
                    );

                    // Marcar notificación como no leída para el cliente
                    var cliente = db.tabla_clientes.Find(pedido.id_cliente);
                    if (cliente != null && cliente.id_usuario.HasValue)
                    {
                        // Eliminar vistas previas de esta notificación
                        var vistasAnteriores = db.tabla_notificaciones_vistas
                            .Where(n => n.id_pedido == idPedido && n.id_usuario == cliente.id_usuario)
                            .ToList();

                        db.tabla_notificaciones_vistas.RemoveRange(vistasAnteriores);
                        db.SaveChanges();
                    }

                    TempData["SuccessMessage"] = $"Estado actualizado a {nuevoEstado}";
                    return RedirectToAction("DetallePedido", new { id = idPedido });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar el estado: " + ex.Message;
                return RedirectToAction("DetallePedido", new { id = idPedido });
            }
        }

        // Obtener detalle de un pedido en formato JSON
        [HttpGet]
        public JsonResult ObtenerDetallePedido(int idPedido)
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    // Obtener detalles de los productos del pedido
                    var detalles = db.tabla_detalle_venta
                        .Where(d => d.id_venta == idPedido)
                        .Include(d => d.tabla_productos)
                        .Select(d => new {
                            id_producto = d.id_producto,
                            nombre = d.tabla_productos.nombre,
                            descripcion = d.tabla_productos.descripcion,
                            img_url = d.tabla_productos.img_url,
                            cantidad = d.cantidad,
                            precio_unitario = d.precio_unitario,
                            subtotal = d.cantidad * d.precio_unitario,
                            personalizacion = d.personalizacion ?? "Ninguna"
                        })
                        .ToList();

                    // Obtener información general del pedido
                    var pedido = db.tabla_ventas
                        .Where(v => v.id_venta == idPedido)
                        .Include(v => v.tabla_clientes)
                        .Include(v => v.tabla_estados_pedido)
                        .Select(v => new {
                            id_pedido = v.id_venta,
                            numero_pedido = "ORD-" + v.fecha.Value.Year + "-" + v.id_venta.ToString().PadLeft(4, '0'),
                            nombre_cliente = (v.id_usuario == null && !string.IsNullOrEmpty(v.nombre_cliente)) ? v.nombre_cliente : v.tabla_clientes.nombre + " " + v.tabla_clientes.apellido,
                            telefono = v.tabla_clientes.telefono ?? "N/A",
                            fecha = v.fecha,
                            total = v.total,
                            estado = v.tabla_estados_pedido.nombre,
                            metodo_pago = v.metodo_pago ?? "N/A",
                            para_llevar = v.para_llevar,
                            pin = v.pin,
                            tiempo_estimado = v.tiempo_estimado ?? 20
                        })
                        .FirstOrDefault();

                    if (pedido == null)
                    {
                        return Json(new { error = "Pedido no encontrado" }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new
                    {
                        productos = detalles,
                        pedido = pedido
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Actualizar estado de un pedido (versión JSON)
        [HttpPost]
        public JsonResult ActualizarEstadoPedido(int idPedido, string nuevoEstado)
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    int idUsuario = GetUserId();
                    var pedido = db.tabla_ventas
                        .Include(p => p.tabla_estados_pedido)
                        .FirstOrDefault(p => p.id_venta == idPedido);

                    if (pedido == null)
                    {
                        return Json(new { success = false, message = "Pedido no encontrado" });
                    }

                    // Validar nuevo estado
                    var estado = db.tabla_estados_pedido.FirstOrDefault(e => e.nombre == nuevoEstado);
                    if (estado == null)
                    {
                        return Json(new { success = false, message = "Estado no válido" });
                    }

                    // Generar mensaje de notificación automático
                    string mensajeNotificacion = GenerarMensajeNotificacion(pedido, nuevoEstado);

                    // Actualizar pedido
                    pedido.id_estado = estado.id_estado;
                    pedido.notificacion = mensajeNotificacion;
                    pedido.fecha_actualizacion = DateTime.Now;

                    db.SaveChanges();

                    // Registrar actividad
                    db.sp_registrar_actividad(
                        idUsuario,
                        "UPDATE",
                        $"Estado del pedido {idPedido} actualizado a {nuevoEstado}"
                    );

                    // Marcar notificación como no leída para el cliente
                    var cliente = db.tabla_clientes.Find(pedido.id_cliente);
                    if (cliente != null && cliente.id_usuario.HasValue)
                    {
                        // Eliminar vistas previas de esta notificación
                        var vistasAnteriores = db.tabla_notificaciones_vistas
                            .Where(n => n.id_pedido == idPedido && n.id_usuario == cliente.id_usuario)
                            .ToList();

                        db.tabla_notificaciones_vistas.RemoveRange(vistasAnteriores);
                        db.SaveChanges();
                    }

                    return Json(new
                    {
                        success = true,
                        message = $"Estado actualizado a {nuevoEstado}",
                        notificacion = mensajeNotificacion
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // Generar mensaje de notificación según el estado del pedido
        private string GenerarMensajeNotificacion(tabla_ventas pedido, string nuevoEstado)
        {
            switch (nuevoEstado)
            {
                case "Pendiente":
                    return $"Pedido #{pedido.id_venta} recibido. Estamos procesando tu solicitud.";
                case "En preparación":
                    return $"¡Tu pedido #{pedido.id_venta} está en preparación! Tiempo estimado: {pedido.tiempo_estimado ?? 20} minutos";
                case "Listo":
                    return $"¡Tu pedido #{pedido.id_venta} está listo! Por favor preséntate con tu PIN: {pedido.pin}";
                case "Entregado":
                    return $"¡Gracias por tu compra! Esperamos que disfrutes tu pedido #{pedido.id_venta}";
                case "Cancelado":
                    return $"Tu pedido #{pedido.id_venta} ha sido cancelado";
                default:
                    return $"El estado de tu pedido #{pedido.id_venta} ha cambiado a: {nuevoEstado}";
            }
        }

        // Procesar el pago de un pedido
        [HttpPost]
        public JsonResult ProcesarPago(PedidoPagoCompletoModel model)
        {
            try
            {
                var pedidoTemporal = Session["PedidoTemporal"] as PedidoPagoCompletoModel;

                if (pedidoTemporal == null)
                {
                    return Json(new
                    {
                        success = false,
                        showAlert = true,
                        title = "Sesión expirada",
                        message = "La sesión del pedido ha expirado, por favor inicia el pedido nuevamente",
                        icon = "error"
                    });
                }

                // Asegurar que todas las personalizaciones tengan valor
                pedidoTemporal.Productos = pedidoTemporal.Productos.Select(p => new ProductoPedidoModel
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    Cantidad = p.Cantidad,
                    PrecioUnitario = p.PrecioUnitario,
                    Personalizacion = p.Personalizacion ?? string.Empty
                }).ToList();

                // Validar datos según método de pago
                switch (model.MetodoPago?.ToLower())
                {

                    case "sinpe":
                        if (string.IsNullOrEmpty(model.TelefonoSinpe) || !Regex.IsMatch(model.TelefonoSinpe, @"^\d{8}$"))
                        {
                            return Json(new
                            {
                                success = false,
                                showAlert = true,
                                title = "Teléfono inválido",
                                message = "Número SINPE móvil inválido (debe tener 8 dígitos)",
                                icon = "error"
                            });
                        }
                        pedidoTemporal.TelefonoSinpe = model.TelefonoSinpe;
                        break;

                    case "efectivo":

                        break;

                    default:
                        return Json(new
                        {
                            success = false,
                            showAlert = true,
                            title = "Método inválido",
                            message = "Método de pago no válido",
                            icon = "error"
                        });

                }

                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    int? idUsuario = null;
                    if (Session["IdUsuario"] != null)
                    {
                        idUsuario = Convert.ToInt32(Session["IdUsuario"]);
                    }

                    // Validar y truncar teléfono
                    var telefono = pedidoTemporal.Telefono ?? "";
                    if (telefono.Length > 20) telefono = telefono.Substring(0, 20);

                    // Buscar o crear cliente
                    tabla_clientes cliente = null;
                    if (idUsuario != null)
                    {
                        cliente = db.tabla_clientes.FirstOrDefault(c => c.id_usuario == idUsuario);
                        if (cliente == null)
                        {
                            cliente = new tabla_clientes
                            {
                                nombre = pedidoTemporal.NombreCliente,
                                apellido = "",
                                telefono = telefono,
                                id_usuario = idUsuario
                            };
                            db.tabla_clientes.Add(cliente);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        // Para invitados, reutiliza el cliente con id_usuario == null (solo debe haber uno)
                        cliente = db.tabla_clientes.FirstOrDefault(c => c.id_usuario == null);
                        if (cliente == null)
                        {
                            cliente = new tabla_clientes
                            {
                                nombre = pedidoTemporal.NombreCliente,
                                apellido = "",
                                telefono = telefono,
                                id_usuario = null
                            };
                            db.tabla_clientes.Add(cliente);
                            db.SaveChanges();
                        }
                    }

                    // Truncar número de pedido y pin si es necesario
                    var numeroPedido = $"ORD-{DateTime.Now.Year}-{(db.tabla_ventas.Count() + 1).ToString("D4")}";
                    if (numeroPedido.Length > 20) numeroPedido = numeroPedido.Substring(0, 20);
                    var pin = new Random().Next(1000, 9999).ToString();

                    var pedido = new tabla_ventas
                    {
                        id_usuario = idUsuario,
                        id_cliente = cliente.id_cliente,
                        fecha = DateTime.Now,
                        total = pedidoTemporal.Total,
                        id_estado = db.tabla_estados_pedido.First(e => e.nombre == "Pendiente").id_estado,
                        numero_pedido = numeroPedido,
                        telefono = telefono,
                        telefono_sinpe = model.MetodoPago.ToLower() == "sinpe" ? model.TelefonoSinpe : null,
                        metodo_pago = model.MetodoPago,
                        para_llevar = pedidoTemporal.ParaLlevar,
                        pin = pin,
                        tiempo_estimado = CalcularTiempoEstimado(pedidoTemporal.Productos.Count),
                        nombre_cliente = idUsuario == null ? pedidoTemporal.NombreCliente : null
                    };

                    db.tabla_ventas.Add(pedido);
                    db.SaveChanges();

                    // Agregar productos al pedido
                    foreach (var producto in pedidoTemporal.Productos)
                    {
                        var personalizacion = string.IsNullOrEmpty(producto.Personalizacion) ? "" : producto.Personalizacion;
                        if (personalizacion.Length > 500) personalizacion = personalizacion.Substring(0, 500);

                        db.tabla_detalle_venta.Add(new tabla_detalle_venta
                        {
                            id_venta = pedido.id_venta,
                            id_producto = producto.IdProducto,
                            cantidad = producto.Cantidad,
                            precio_unitario = producto.PrecioUnitario,
                            personalizacion = personalizacion
                        });
                    }

                    db.SaveChanges();

                    // Actualizar número de pedido con ID real
                    var numeroPedidoFinal = $"ORD-{pedido.fecha.Value.Year}-{pedido.id_venta.ToString("D4")}";
                    if (numeroPedidoFinal.Length > 20) numeroPedidoFinal = numeroPedidoFinal.Substring(0, 20);
                    pedido.numero_pedido = numeroPedidoFinal;
                    db.SaveChanges();

                    Session.Remove("PedidoTemporal");

                    return Json(new
                    {
                        success = true,
                        message = "Pago procesado exitosamente",
                        numeroPedido = numeroPedidoFinal,
                        pin = pedido.pin,
                        tiempoEstimado = pedido.tiempo_estimado,
                        redirectUrl = Url.Action("ConfirmacionPedido", new { id = pedido.id_venta })
                    });
                }
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                return Json(new
                {
                    success = false,
                    showAlert = true,
                    title = "Error de validación",
                    message = fullErrorMessage,
                    icon = "error"
                });
            }
            catch (Exception ex)
            {
                var inner = ex;
                var sb = new StringBuilder();
                while (inner != null)
                {
                    sb.AppendLine(inner.Message);
                    inner = inner.InnerException;
                }
                return Json(new
                {
                    success = false,
                    showAlert = true,
                    title = "Error",
                    message = "Error al procesar el pago: " + ex.Message,
                    icon = "error"
                });
            }
        }

        // Registrar valoración de un pedido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarValoracion(ValoracionModel model, string pin = null)
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    tabla_ventas pedido = null;

                    if (Session["IdUsuario"] != null)
                    {
                        int userId = GetUserId();
                        var cliente = db.tabla_clientes.FirstOrDefault(c => c.id_usuario == userId);
                        if (cliente == null)
                            return RedirectToAction("MisPedidos");

                        pedido = db.tabla_ventas.FirstOrDefault(v =>
                            v.id_venta == model.IdPedido &&
                            v.id_cliente == cliente.id_cliente &&
                            v.tabla_estados_pedido.nombre == "Entregado");
                    }
                    else if (!string.IsNullOrEmpty(pin))
                    {
                        pedido = db.tabla_ventas.FirstOrDefault(v =>
                            v.id_venta == model.IdPedido &&
                            v.pin == pin &&
                            v.tabla_estados_pedido.nombre == "Entregado");
                    }

                    if (pedido == null || db.tabla_valoraciones.Any(v => v.id_pedido == model.IdPedido))
                        return RedirectToAction("MisPedidos");

                    var valoracion = new tabla_valoraciones
                    {
                        id_pedido = model.IdPedido,
                        calificacion = model.Calificacion,
                        comentarios = model.Comentarios,
                        fecha = DateTime.Now
                    };

                    db.tabla_valoraciones.Add(valoracion);
                    db.SaveChanges();

                    TempData["SweetAlert"] = JsonConvert.SerializeObject(new
                    {
                        title = "¡Gracias!",
                        text = "Apreciamos tu feedback. ¡Tu opinión nos ayuda a mejorar!",
                        icon = "success",
                        timer = 5000,
                        showConfirmButton = false
                    });

                    return RedirectToAction("MisPedidos", new { pin });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al registrar la valoración: " + ex.Message;
                return RedirectToAction("ValorarPedido", new { idPedido = model.IdPedido, pin });
            }
        }

        // Obtener valoración de un pedido
        [HttpGet]
        public JsonResult ObtenerValoracion(int idPedido)
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    var valoracion = db.tabla_valoraciones
                        .Where(v => v.id_pedido == idPedido)
                        .Select(v => new {
                            calificacion = v.calificacion,
                            comentarios = v.comentarios,
                            fecha = v.fecha
                        })
                        .FirstOrDefault();

                    return Json(new
                    {
                        success = true,
                        existe = valoracion != null,
                        valoracion = valoracion
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Actualizar tiempo estimado de entrega
        [HttpPost]
        public JsonResult ActualizarTiempoEstimado(int idPedido, int tiempoEstimado)
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    int idUsuario = GetUserId();
                    var pedido = db.tabla_ventas.Find(idPedido);

                    if (pedido == null)
                    {
                        return Json(new { success = false, message = "Pedido no encontrado" });
                    }

                    // Actualizar tiempo estimado
                    pedido.tiempo_estimado = tiempoEstimado;
                    db.SaveChanges();

                    // Registrar actividad
                    var actividad = new tabla_actividades
                    {
                        id_usuario = idUsuario,
                        tipo_accion = "UPDATE",
                        descripcion = $"Tiempo estimado actualizado para pedido {idPedido} a {tiempoEstimado} minutos",
                        fecha_hora = DateTime.Now,
                        tabla_afectada = "tabla_ventas",
                        id_registro_afectado = idPedido
                    };
                    db.tabla_actividades.Add(actividad);
                    db.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        message = "Tiempo estimado actualizado correctamente",
                        nuevoTiempo = tiempoEstimado
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // Mostrar pedidos del usuario actuales
        [HttpGet]
        public ActionResult MisPedidos(string numeroPedido = null, string pin = null)
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                List<PedidoClienteViewModel> pedidos;

                if (Session["IdUsuario"] != null)
                {
                    int userId = GetUserId();
                    pedidos = db.tabla_ventas
                        .Where(v => v.id_usuario == userId)
                        .OrderByDescending(v => v.fecha)
                        .Select(v => new PedidoClienteViewModel
                        {
                            IdPedido = v.id_venta,
                            NumeroPedido = v.numero_pedido,
                            Fecha = v.fecha.Value,
                            Total = v.total,
                            Estado = v.tabla_estados_pedido.nombre,
                            TiempoEstimado = v.tiempo_estimado ?? 20,
                            Notificacion = v.notificacion,
                            Valorado = db.tabla_valoraciones.Any(val => val.id_pedido == v.id_venta),
                            Pin = v.pin
                        })
                        .ToList();
                }
                else if (!string.IsNullOrEmpty(numeroPedido) && !string.IsNullOrEmpty(pin))
                {
                    pedidos = db.tabla_ventas
                        .Where(v => v.numero_pedido == numeroPedido && v.pin == pin)
                        .OrderByDescending(v => v.fecha)
                        .Select(v => new PedidoClienteViewModel
                        {
                            IdPedido = v.id_venta,
                            NumeroPedido = v.numero_pedido,
                            Fecha = v.fecha.Value,
                            Total = v.total,
                            Estado = v.tabla_estados_pedido.nombre,
                            TiempoEstimado = v.tiempo_estimado ?? 20,
                            Notificacion = v.notificacion,
                            Valorado = db.tabla_valoraciones.Any(val => val.id_pedido == v.id_venta),
                            Pin = v.pin
                        })
                        .ToList();
                }
                else
                {
                    pedidos = new List<PedidoClienteViewModel>();
                }

                ViewBag.EsInvitado = Session["IdUsuario"] == null;
                ViewBag.NumeroPedido = numeroPedido;
                ViewBag.Pin = pin;
                return View(pedidos);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelarPedidoInvitado(int idPedido, string pin)
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var pedido = db.tabla_ventas.FirstOrDefault(v =>
                    v.id_venta == idPedido &&
                    v.pin == pin &&
                    (v.tabla_estados_pedido.nombre == "Pendiente" || v.tabla_estados_pedido.nombre == "En preparación")
                );

                if (pedido == null)
                {
                    TempData["ErrorMessage"] = "No se pudo cancelar el pedido. Verifica los datos.";
                    return RedirectToAction("MisPedidos", new { numeroPedido = (string)null, pin });
                }

                var estadoCancelado = db.tabla_estados_pedido.FirstOrDefault(e => e.nombre == "Cancelado");
                if (estadoCancelado == null)
                {
                    TempData["ErrorMessage"] = "No se pudo cancelar el pedido. Estado no configurado.";
                    return RedirectToAction("MisPedidos", new { numeroPedido = pedido.numero_pedido, pin });
                }

                pedido.id_estado = estadoCancelado.id_estado;
                pedido.notificacion = "Pedido cancelado por el cliente";
                pedido.fecha_actualizacion = DateTime.Now;
                db.SaveChanges();

                TempData["SuccessMessage"] = "Pedido cancelado exitosamente";
                return RedirectToAction("MisPedidos", new { numeroPedido = pedido.numero_pedido, pin });
            }
        }

        // Mostrar formulario de pago
        [HttpGet]
        public ActionResult FormularioPago()
        {
            var pedidoTemporal = Session["PedidoTemporal"] as PedidoPagoCompletoModel;

            if (pedidoTemporal == null)
            {
                return RedirectToAction("Pedidos");
            }

            return View("Pago", pedidoTemporal);
        }

        // Obtener estado de un pedido específico
        [HttpGet]
        public JsonResult ObtenerEstadoPedido(int idPedido)
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var pedido = db.tabla_ventas
                    .Where(v => v.id_venta == idPedido)
                    .Select(v => new {
                        estado = v.tabla_estados_pedido.nombre,
                        fecha = v.fecha,
                        tiempoEstimado = v.tiempo_estimado ?? 20,
                        notificacion = v.notificacion
                    })
                    .FirstOrDefault();

                if (pedido == null)
                {
                    return Json(new { error = "Pedido no encontrado" }, JsonRequestBehavior.AllowGet);
                }

                return Json(pedido, JsonRequestBehavior.AllowGet);
            }
        }

        // Obtener estado de una orden (similar a ObtenerEstadoPedido)
        [HttpPost]
        public JsonResult ObtenerEstadoOrden(int idPedido)
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var estado = db.tabla_ventas
                    .Where(v => v.id_venta == idPedido)
                    .Select(v => new {
                        estado = v.tabla_estados_pedido.nombre,
                        fecha = v.fecha,
                        tiempoEstimado = v.tiempo_estimado
                    })
                    .FirstOrDefault();

                return Json(estado);
            }
        }

        // Confirmar pedido antes de procesar pago
        [HttpPost]
        public JsonResult ConfirmarPedido(PedidosModel model)
        {
            try
            {
                if (model.Productos.Any(p => p.PrecioUnitario <= 0))
                {
                    return Json(new { success = false, message = "No se pueden incluir productos que requieren consultar precio" });
                }
                // Asegurar que las personalizaciones no sean nulas
                if (model.Productos != null)
                {
                    foreach (var producto in model.Productos)
                    {
                        producto.Personalizacion = producto.Personalizacion ?? string.Empty;
                    }
                }

                // Inicializar completamente el objeto pedidoTemporal
                var pedidoTemporal = new PedidoPagoCompletoModel
                {
                    NombreCliente = model.NombreCliente,
                    Telefono = model.Telefono ?? string.Empty,
                    ParaLlevar = model.ParaLlevar,
                    Productos = model.Productos.Select(p => new ProductoPedidoModel
                    {
                        IdProducto = p.IdProducto,
                        Nombre = p.Nombre,
                        Cantidad = p.Cantidad,
                        PrecioUnitario = p.PrecioUnitario,
                        Personalizacion = p.Personalizacion ?? string.Empty
                    }).ToList(),
                    Subtotal = model.Productos.Sum(p => p.Cantidad * p.PrecioUnitario),
                    Impuestos = model.Productos.Sum(p => p.Cantidad * p.PrecioUnitario) * 0.13m,
                    Total = model.Productos.Sum(p => p.Cantidad * p.PrecioUnitario) * 1.13m,
                    Fecha = DateTime.Now
                };

                // Limpiar y recrear la sesión completamente
                Session.Remove("PedidoTemporal");
                Session["PedidoTemporal"] = pedidoTemporal;

                return Json(new { success = true, redirectUrl = Url.Action("FormularioPago") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al procesar el pedido: " + ex.Message });
            }
        }


        // Mostrar confirmación de pedido
        [HttpGet]
        public ActionResult ConfirmacionPedido(int id)
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var pedido = db.tabla_ventas
                    .Include(v => v.tabla_clientes)
                    .Include(v => v.tabla_detalle_venta.Select(d => d.tabla_productos))
                    .FirstOrDefault(v => v.id_venta == id);

                if (pedido == null)
                {
                    return RedirectToAction("Pedidos");
                }

                // Crear modelo para la vista de confirmación
                var model = new ConfirmacionPedidoViewModel
                {
                    IdPedido = pedido.id_venta,
                    NumeroPedido = pedido.numero_pedido ?? $"ORD-{pedido.fecha.Value.Year}-{pedido.id_venta.ToString("D4")}",
                    Pin = pedido.pin,
                    TiempoEstimado = pedido.tiempo_estimado ?? 20,
                    Total = pedido.total,
                    MetodoPago = pedido.metodo_pago,
                    TelefonoSinpe = pedido.telefono,
                    NombreCliente = !string.IsNullOrEmpty(pedido.nombre_cliente) ? pedido.nombre_cliente : pedido.tabla_clientes.nombre + " " + pedido.tabla_clientes.apellido,
                    Productos = pedido.tabla_detalle_venta.Select(d => new ProductoPedidoModel
                    {
                        IdProducto = d.id_producto,
                        Nombre = d.tabla_productos.nombre,
                        Cantidad = d.cantidad,
                        PrecioUnitario = d.precio_unitario,
                        Personalizacion = d.personalizacion
                    }).ToList()
                };

                return View(model);
            }
        }

        // Mostrar formulario para valorar un pedido
        [HttpGet]
        public ActionResult ValorarPedido(int? idPedido, string pin = null)
        {
            if (!idPedido.HasValue)
                return RedirectToAction("MisPedidos");

            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                tabla_ventas pedido = null;

                if (Session["IdUsuario"] != null)
                {
                    int userId = GetUserId();
                    var cliente = db.tabla_clientes.FirstOrDefault(c => c.id_usuario == userId);
                    if (cliente == null)
                        return RedirectToAction("MisPedidos");

                    pedido = db.tabla_ventas.FirstOrDefault(v =>
                        v.id_venta == idPedido.Value &&
                        v.id_cliente == cliente.id_cliente &&
                        v.tabla_estados_pedido.nombre == "Entregado");
                }
                else if (!string.IsNullOrEmpty(pin))
                {
                    pedido = db.tabla_ventas.FirstOrDefault(v =>
                        v.id_venta == idPedido.Value &&
                        v.pin == pin &&
                        v.tabla_estados_pedido.nombre == "Entregado");
                }

                if (pedido == null)
                    return RedirectToAction("MisPedidos");

                if (db.tabla_valoraciones.Any(v => v.id_pedido == idPedido.Value))
                    return RedirectToAction("MisPedidos");

                var model = new ValoracionModel
                {
                    IdPedido = idPedido.Value,
                    NumeroPedido = $"ORD-{pedido.fecha.Value.Year}-{pedido.id_venta.ToString("D4")}"
                };

                ViewBag.Pin = pin;
                return View(model);
            }
        }

        // Obtener productos recomendados
        [HttpGet]
        public JsonResult ObtenerProductosRecomendados()
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    var productosRecomendadosIds = db.tabla_recomendaciones
                                                  .Select(r => r.id_producto)
                                                  .Distinct()
                                                  .ToList();

                    // Obtener detalles de productos recomendados
                    var productos = db.tabla_productos
                        .Where(p => productosRecomendadosIds.Contains(p.id_producto))
                        .Select(p => new {
                            p.id_producto,
                            p.nombre,
                            p.descripcion,
                            p.id_categoria,
                            p.precio_por_unidad,
                            p.img_url,
                            esRecomendado = true
                        })
                        .ToList();

                    return Json(productos, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Notificar cambio de estado a cliente
        [HttpPost]
        public JsonResult NotificarCambioEstado(int idPedido)
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    int idUsuario = GetUserId();
                    var pedido = db.tabla_ventas
                        .Include(p => p.tabla_estados_pedido)
                        .FirstOrDefault(p => p.id_venta == idPedido);

                    if (pedido == null)
                    {
                        return Json(new { success = false, message = "Pedido no encontrado" });
                    }

                    // Generar mensaje de notificación
                    string mensaje = $"Tu pedido #{pedido.id_venta} está ahora {pedido.tabla_estados_pedido.nombre}.";

                    if (pedido.tabla_estados_pedido.nombre == "En preparación" || pedido.tabla_estados_pedido.nombre == "Listo")
                    {
                        mensaje += $" Tiempo estimado: {pedido.tiempo_estimado ?? 20} minutos.";
                    }

                    // Actualizar notificación
                    pedido.notificacion = mensaje;
                    db.SaveChanges();

                    // Registrar actividad
                    db.sp_registrar_actividad(
                        idUsuario,
                        "NOTIFICATION",
                        $"Notificación enviada para pedido {idPedido}: {mensaje}"
                    );

                    return Json(new
                    {
                        success = true,
                        message = "Notificación enviada",
                        notificacion = mensaje
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // Enviar notificación de valoración
        [HttpPost]
        public JsonResult EnviarNotificacionValoracion(int idPedido)
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    // Registrar actividad de valoración
                    db.sp_registrar_actividad(
                        GetUserId(),
                        "NOTIFICATION",
                        $"Notificación de valoración enviada para pedido {idPedido}"
                    );
                }

                return Json(new
                {
                    success = true,
                    message = "¡Gracias por tu valoración! Tu opinión nos ayuda a mejorar."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // Obtener ID de cliente o crear uno nuevo si no existe
        private int ObtenerIdCliente(BD_CREANDO_RECUERDOSEntities db, int idUsuario)
        {
            var cliente = db.tabla_clientes.FirstOrDefault(c => c.id_usuario == idUsuario);

            if (cliente == null)
            {
                var usuario = db.tabla_usuarios.Find(idUsuario);

                // Crear nuevo cliente
                cliente = new tabla_clientes
                {
                    nombre = usuario?.nombre ?? "Cliente",
                    apellido = "",
                    telefono = "",
                    id_usuario = idUsuario
                };

                db.tabla_clientes.Add(cliente);
                db.SaveChanges();

                // Asociar pedidos existentes sin cliente
                var pedidosSinCliente = db.tabla_ventas
                    .Where(v => v.id_usuario == idUsuario && v.id_cliente != cliente.id_cliente)
                    .ToList();

                foreach (var pedido in pedidosSinCliente)
                {
                    pedido.id_cliente = cliente.id_cliente;
                }

                if (pedidosSinCliente.Any())
                {
                    db.SaveChanges();
                }
            }

            return cliente.id_cliente;
        }

        // Obtener ID de usuario de la sesión
        private int GetUserId()
        {
            if (Session["IdUsuario"] != null)
            {
                return Convert.ToInt32(Session["IdUsuario"]);
            }
            // Redirigir al login si no está autenticado
            Response.Redirect("~/Registro_Usuarios/registro_usuarios");
            return -1; // Opcional, nunca se ejecuta si hay redirección

        }

        // Calcular tiempo estimado basado en cantidad de productos
        private int CalcularTiempoEstimado(int cantidadProductos)
        {
            return Math.Min(20 + (cantidadProductos * 5), 60);
        }

        [HttpGet]
        [RolAuthorize("1")]
        public ActionResult RegistroVentas()
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                var usuarios = context.sp_mostrar_ventas_web().ToList();

                return View(usuarios);
            }
        }
    }
}