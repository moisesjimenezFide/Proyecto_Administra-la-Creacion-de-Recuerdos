using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Filters
{
    public class AutorizacionFilter : ActionFilterAttribute
    {
        private readonly int[] _rolesPermitidos;

        // Acepta un array de roles; si no se pasa nada, solo requiere sesión
        public AutorizacionFilter(params int[] rolesPermitidos)
        {
            _rolesPermitidos = rolesPermitidos ?? new int[0];
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var usuario = filterContext.HttpContext.Session["NombreUsuario"] as string;
            var rolUsuario = filterContext.HttpContext.Session["Rol"] != null
                ? Convert.ToInt32(filterContext.HttpContext.Session["Rol"])
                : 0;

            // Si no hay sesión → redirigir al login
            if (string.IsNullOrEmpty(usuario))
            {
                filterContext.Result = new RedirectResult("~/Inicio/AccesoDenegado?expired=true");
                return;
            }

            // Si hay roles definidos y el rol del usuario no está en la lista → acceso denegado
            if (_rolesPermitidos.Length > 0 && !_rolesPermitidos.Contains(rolUsuario))
            {
                filterContext.Result = new RedirectResult("~/Inicio/AccesoDenegado");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
