using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    public class MenuController : Controller
    {
        [HttpGet]
        public ActionResult menu()
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var resultados = db.sp_consultar_productos().ToList();


                return View(resultados);
            }
        }

        [HttpGet]
        public ActionResult menu_admin()
        {
            using (var db = new BD_CREANDO_RECUERDOSEntities())
            {
                var resultados = db.sp_consultar_productos().ToList();


                return View(resultados);
            }
        }


        [HttpPost]
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





    }
}
