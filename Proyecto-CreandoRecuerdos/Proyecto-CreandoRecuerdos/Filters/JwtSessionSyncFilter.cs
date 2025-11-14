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

                // Guardar en cookie de estado
                var sessionTimeout = 60; // minutos, igual que Web.config
                var wasAuthCookie = new HttpCookie("WasAuthenticated", "true")
                {
                    Expires = DateTime.UtcNow.AddMinutes(sessionTimeout + 5), // 5 minutos extra
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                };
                HttpContext.Current.Response.Cookies.Set(wasAuthCookie);
                
                // Guardar en cookie persistente
                var lastAuthCookie = new HttpCookie("LastAuthenticated", DateTime.UtcNow.ToString("o"))
                {
                    Expires = DateTime.UtcNow.AddDays(1), // dura 1 día
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                };
                HttpContext.Current.Response.Cookies.Set(lastAuthCookie);
            }
            else
            {
                HttpContext.Current.Session["WasAuthenticated"] = false;

                // Eliminar cookie de estado
                var wasAuthCookie = new HttpCookie("WasAuthenticated", "")
                {
                    Expires = DateTime.UtcNow.AddDays(-1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                };
                HttpContext.Current.Response.Cookies.Set(wasAuthCookie);

                // Eliminar cookie persistente
                var lastAuthCookie = new HttpCookie("LastAuthenticated", "")
                {
                    Expires = DateTime.UtcNow.AddDays(-1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                };
                HttpContext.Current.Response.Cookies.Set(lastAuthCookie);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}