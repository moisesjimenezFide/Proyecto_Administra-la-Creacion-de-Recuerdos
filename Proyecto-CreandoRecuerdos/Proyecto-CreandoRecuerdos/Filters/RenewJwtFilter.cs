using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Proyecto_CreandoRecuerdos.Helpers;

namespace Proyecto_CreandoRecuerdos.Filters
{
    public class RenewJwtFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var authHeader = HttpContext.Current.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();

                try
                {
                    var jwtToken = handler.ReadJwtToken(token);
                    var exp = jwtToken.ValidTo;
                    var tiempoRestante = exp - DateTime.UtcNow;

                    if (tiempoRestante.TotalSeconds < 20) // se renueva si faltan menos de 20 s
                    {
                        var username = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                        var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                        if (!string.IsNullOrEmpty(username))
                        {
                            var newToken = JwtManager.GenerateToken(username, role);
                            HttpContext.Current.Response.Headers.Add("X-Renewed-Token", newToken);
                        }
                    }
                }
                catch
                {
                    // token inválido o expirado; no se renueva
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
