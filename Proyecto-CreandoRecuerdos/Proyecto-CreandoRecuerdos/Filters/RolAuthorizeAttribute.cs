using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RolAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly string[] _rolesPermitidos;

        public RolAuthorizeAttribute(params string[] roles)
        {
            _rolesPermitidos = roles;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var user = filterContext.HttpContext.User as ClaimsPrincipal;
            var wasAuthenticatedSession = filterContext.HttpContext.Session["WasAuthenticated"] as bool?;
            var wasAuthenticatedCookie = filterContext.HttpContext.Request.Cookies["WasAuthenticated"]?.Value == "true";

            // Si no hay usuario autenticado
            if (user == null || !user.Identity.IsAuthenticated)
            {
                if (wasAuthenticatedSession == true || wasAuthenticatedCookie)
                {
                    // Era autenticado y perdió la sesión: inactividad
                    filterContext.Controller.TempData["Message"] = "Tu sesión ha expirado por inactividad. Por favor inicia sesión nuevamente.";
                }
                else
                {
                    // Nunca estuvo autenticado: invitado sin permisos
                    filterContext.Controller.TempData["Message"] = "No tienes permisos para acceder a esta sección.";
                }
                filterContext.Result = new RedirectResult("~/Inicio/AccesoDenegado");
                return;
            }

            var rolClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(rolClaim) || !_rolesPermitidos.Contains(rolClaim))
            {
                filterContext.Controller.TempData["Message"] = "No tienes permisos para acceder a esta sección.";
                filterContext.Result = new RedirectResult("~/Inicio/AccesoDenegado");
                return;
            }
        }
    }
}
