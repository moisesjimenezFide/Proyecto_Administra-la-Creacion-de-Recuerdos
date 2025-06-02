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

     
    }
}
