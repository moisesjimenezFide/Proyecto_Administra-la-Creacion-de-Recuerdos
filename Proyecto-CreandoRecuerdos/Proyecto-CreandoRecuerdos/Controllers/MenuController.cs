using Proyecto_CreandoRecuerdos.base_de_datos;
using System;
using System.Collections.Generic;
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

    }
}
