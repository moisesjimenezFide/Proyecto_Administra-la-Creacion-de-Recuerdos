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
        public ActionResult menu_admin(long Id, string Nombre, string Descripcion, string Precio, string ImagenActual, HttpPostedFileBase Imagen)
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

                    int filasAfectadas = db.sp_actualizar_producto(idInt, Nombre, Descripcion, Convert.ToDecimal(Precio), nuevaRutaImagen);

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



    }
}
