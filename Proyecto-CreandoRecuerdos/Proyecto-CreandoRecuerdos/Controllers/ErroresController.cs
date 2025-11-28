using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    public class ErroresController : Controller
    {
        public ActionResult NoEncontrado404()
        {
            Response.StatusCode = 404;
            return View("NoEncontrado404");
        }

        public ActionResult Error()
        {
            return View("Error");
        }
    }
}