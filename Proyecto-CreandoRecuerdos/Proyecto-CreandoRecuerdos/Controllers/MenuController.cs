using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Filters;
using Proyecto_CreandoRecuerdos.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    // Evitar el almacenamiento en caché de las vistas
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]

    public class MenuController : Controller
    {
        [HttpGet]
        public ActionResult menu()
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var resultados = db.sp_consultar_productos().ToList();

                ViewBag.ProductosRecomendados = db.tabla_recomendaciones
                                                  .Select(r => r.id_producto)
                                                  .ToList();

                return View(resultados);
            }
        }

        [HttpGet]
        [RolAuthorize("1")]
        public ActionResult menu_admin()
        {

            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var resultados = db.sp_consultar_productos().Select(p => new ProductoModel
                {
                    id_producto = p.id_producto,
                    nombre = p.nombre,
                    descripcion = p.descripcion,
                    id_categoria = p.id_categoria,
                    precio_por_unidad = p.precio_por_unidad,
                    img_url = p.img_url,
                    EsRecomendado = db.tabla_recomendaciones.Any(r => r.id_producto == p.id_producto)
                })
            .ToList();

                // Agregá esta línea para enviar los productos recomendados a la vista
                ViewBag.ProductosRecomendados = db.tabla_recomendaciones
                                                  .Select(r => r.id_producto)
                                                  .ToList();

                return View(resultados);
            }
        }

        [HttpGet]
        public JsonResult ObtenerProductosDisponibles()
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                // Obtener TODOS los productos del menú, sin filtrar por recomendaciones
                var productosDisponibles = db.tabla_productos
                    .Select(p => new {
                        p.id_producto,
                        p.nombre,
                        p.descripcion,
                        p.id_categoria,
                        p.precio_por_unidad,
                        p.img_url
                    })
                    .ToList();

                // Crear una nueva lista con la imagen corregida si es necesario
                var productosConImagen = productosDisponibles.Select(producto =>
                {
                    var imgPath = Server.MapPath($"~/Templates/img/menu/{producto.img_url}");
                    var imgUrlFinal = (string.IsNullOrEmpty(producto.img_url) || !System.IO.File.Exists(imgPath))
                        ? "default.png"
                        : producto.img_url;

                    return new
                    {
                        producto.id_producto,
                        producto.nombre,
                        producto.descripcion,
                        producto.id_categoria,
                        producto.precio_por_unidad,
                        img_url = imgUrlFinal
                    };
                }).ToList();


                return Json(productosConImagen, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [RolAuthorize("1")]
        public ActionResult menu_admin(long Id,string Nombre,string Descripcion,string Precio,string ImagenActual,int IdCategoria, HttpPostedFileBase Imagen)
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    string nuevaRutaImagen;

                    if (Imagen != null && Imagen.ContentLength > 0)
                    {
                        var fileName = Id + ".jpg";
                        var path = Path.Combine(Server.MapPath("~/Templates/img/menu/"), fileName);
                        Imagen.SaveAs(path);
                        nuevaRutaImagen = fileName;
                    }
                    else
                    {
                        nuevaRutaImagen = ImagenActual;
                    }

                    int idInt = Convert.ToInt32(Id);
                    decimal precioDecimal = Convert.ToDecimal(Precio); 

                    int filasAfectadas = db.sp_actualizar_producto(
                        idInt,
                        Nombre,
                        Descripcion,
                        precioDecimal,
                        nuevaRutaImagen,
                        IdCategoria 
                    );

                    if (filasAfectadas > 0)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        ViewBag.Mensaje = "No se pudo actualizar el producto.";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Ocurrió un error: " + ex.Message);
            }
        }


        [HttpPost]
        [RolAuthorize("1")]
        public ActionResult crear_producto(string Nombre, string Descripcion, string Precio, int IdCategoria, HttpPostedFileBase Imagen)
        {
            try
            {
                if (Imagen == null || Imagen.ContentLength == 0)
                    return Json(new { success = false, message = "La imagen es obligatoria." });

                if (!decimal.TryParse(Precio, out decimal precioDecimal))
                    return Json(new { success = false, message = "El precio no es válido." });

                var ext = Path.GetExtension(Imagen.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                    return Json(new { success = false, message = "Solo se permiten imágenes JPG o PNG." });

                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    string rutaTemporal = "Recuerdos.png";

                    int nuevoId = db.sp_crear_producto(
                        Nombre,
                        Descripcion,
                        precioDecimal,
                        IdCategoria,
                        rutaTemporal
                    ).FirstOrDefault() ?? 0;

                    if (nuevoId <= 0)
                        return Json(new { success = false, message = "No se pudo crear el producto." });

                    var fileName = nuevoId + ".jpg";
                    var path = Path.Combine(Server.MapPath("~/Templates/img/menu/"), fileName);
                    Imagen.SaveAs(path);

                    int filasAfectadas = db.sp_actualizar_producto(
                        nuevoId,
                        Nombre,
                        Descripcion,
                        precioDecimal,
                        fileName,
                        IdCategoria
                    );

                    if (filasAfectadas > 0)
                        return Json(new { success = true });

                    return Json(new { success = false, message = "Error al actualizar la imagen del producto." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error: " + ex.Message });
            }
        }

        [HttpPost]
        [RolAuthorize("1")]
        public ActionResult eliminar_producto(int id)
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    int filasAfectadas = db.sp_eliminar_producto(id);

                    if (filasAfectadas > 0)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "No se pudo eliminar el producto." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error: " + ex.Message });
            }
        }

        [HttpPost]
        [RolAuthorize("1")]
        public ActionResult GuardarRecomendacion(int productoId, string motivo)
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    db.sp_insertar_recomendacion(productoId, motivo);
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
       
        [RolAuthorize("1")]
        public ActionResult Recomendaciones()
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var idsRecomendados = db.tabla_recomendaciones.Select(r => r.id_producto).ToList();

                var productos = db.tabla_productos
                    .Where(p => idsRecomendados.Contains(p.id_producto))
                    .Select(p => new ProductoModel
                    {
                        id_producto = p.id_producto,
                        nombre = p.nombre,
                        descripcion = p.descripcion,
                        id_categoria = p.id_categoria,
                        precio_por_unidad = p.precio_por_unidad,
                        img_url = p.img_url,
                        EsRecomendado = true
                    })
                    .ToList();

                ViewBag.ProductosRecomendados = idsRecomendados;
                return View("menu_admin", productos);
            }
        }

        [HttpGet]
        public JsonResult RecomendacionesAjax()
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var ids = db.tabla_recomendaciones.Select(r => r.id_producto).ToList();
                var productos = db.tabla_productos
                                  .Where(p => ids.Contains(p.id_producto))
                                  .Select(p => new {
                                      p.id_producto,
                                      p.nombre
                                  }).ToList();
                return Json(productos, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [RolAuthorize("1")]
        public ActionResult EliminarRecomendacion(int id)
        {
            try
            {
                using (var db = new BD_CREANDO_RECUERDOSEntities())
                {
                    var recomendacion = db.tabla_recomendaciones.FirstOrDefault(r => r.id_producto == id);
                    if (recomendacion != null)
                    {
                        db.tabla_recomendaciones.Remove(recomendacion);
                        db.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false, message = "No se encontró la recomendación." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

    }
}
