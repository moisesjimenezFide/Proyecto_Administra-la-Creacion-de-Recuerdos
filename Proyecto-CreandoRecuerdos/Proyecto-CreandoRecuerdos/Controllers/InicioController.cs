using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    public class InicioController : Controller
    {
        // GET: Home
        public ActionResult inicio()
        {
            
            return View();
        }

        // GET: Home
        public ActionResult pago()
        {
            return View();
        }

        public ActionResult AccesoDenegado()
        {
            if (Request.QueryString["expired"] == "true")
            {
                ViewBag.Mensaje = "Tu sesión ha expirado por inactividad. Por favor inicia sesión nuevamente.";
            }
            return View(); 
        }

    }
}
