using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    // Evitar el almacenamiento en caché de las vistas
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]

    public class InicioController : Controller
    {
        // GET: Home
        public ActionResult inicio()
        {
            
            return View();
        }

        // GET: Pago
        public ActionResult pago()
        {
            return View();
        }

        public ActionResult AccesoDenegado()
        {
            // El mensaje se puede recibir por TempData o ViewBag
            ViewBag.Message = TempData["Message"] as string;
            return View();
        }
    }
}
