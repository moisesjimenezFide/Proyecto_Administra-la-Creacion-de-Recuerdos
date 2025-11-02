using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
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

            // Si no hay usuario autenticado
            if (user == null || !user.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/Inicio/AccesoDenegado");
                return;
            }

            // Obtener el rol del JWT
            var rolClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Si el rol no está permitido
            if (rolClaim == null || !_rolesPermitidos.Contains(rolClaim))
            {
                filterContext.Result = new RedirectResult("~/Inicio/AccesoDenegado");
                return;
            }
        }
    }
}
