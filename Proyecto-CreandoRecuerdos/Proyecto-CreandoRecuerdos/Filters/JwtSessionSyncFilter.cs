using System;
using System.Security.Claims;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Filters
{
    public class JwtSessionSyncFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = HttpContext.Current.User as ClaimsPrincipal;
            if (user != null && user.Identity.IsAuthenticated)
            {
                var rol = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var nombre = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                HttpContext.Current.Session["Rol"] = rol;
                HttpContext.Current.Session["Usuario"] = nombre;
                HttpContext.Current.Session["WasAuthenticated"] = true;

                // Guardar en cookie
                var wasAuthCookie = new HttpCookie("WasAuthenticated", "true")
                {
                    Expires = DateTime.UtcNow.AddHours(1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                };
                HttpContext.Current.Response.Cookies.Set(wasAuthCookie);
            }
            else
            {
                HttpContext.Current.Session["WasAuthenticated"] = false;

                // Eliminar cookie
                var wasAuthCookie = new HttpCookie("WasAuthenticated", "")
                {
                    Expires = DateTime.UtcNow.AddDays(-1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                };
                HttpContext.Current.Response.Cookies.Set(wasAuthCookie);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}