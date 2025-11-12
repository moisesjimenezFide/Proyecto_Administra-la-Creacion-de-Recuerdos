using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Proyecto_CreandoRecuerdos.Helpers;

namespace Proyecto_CreandoRecuerdos.Filters
{
    public class JwtCookieAuthorizeFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = HttpContext.Current.Request.Cookies["JWT"];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                try
                {
                    var principal = JwtManager.ValidateToken(cookie.Value);
                    if (principal != null)
                    {
                        HttpContext.Current.User = principal;// reconstruir identidad
                        System.Threading.Thread.CurrentPrincipal = principal;

                        var handler = new JwtSecurityTokenHandler();
                        var token = handler.ReadJwtToken(cookie.Value);
                        var exp = token.ValidTo;
                        var tiempoRestante = exp - DateTime.UtcNow;

                        // 🔁 renovar si faltan menos de 20 s
                        if (tiempoRestante.TotalSeconds < 20)
                        {
                            var username = principal.Identity.Name;
                            var role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                            var newToken = JwtManager.GenerateToken(username, role, 1);
                            var newCookie = new HttpCookie("JWT", newToken)
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Lax,
                                Expires = DateTime.UtcNow.AddMinutes(1)
                            };
                            HttpContext.Current.Response.Cookies.Add(newCookie);
                        }
                    }
                }
                catch
                {
                    // token inválido o expirado → borrar cookie
                    var expired = new HttpCookie("JWT", "")
                    {
                        Expires = DateTime.Now.AddDays(-1)
                    };
                    HttpContext.Current.Response.Cookies.Add(expired);
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
