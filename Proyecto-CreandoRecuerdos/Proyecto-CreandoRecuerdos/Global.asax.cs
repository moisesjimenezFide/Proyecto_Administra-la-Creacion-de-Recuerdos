using Proyecto_CreandoRecuerdos.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Proyecto_CreandoRecuerdos
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Forzar TLS 1.2 y TLS 1.3
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | (SecurityProtocolType)12288;
            // Nota: TLS 1.3 no está definido en .NET Framework < 4.8, por eso usamos el valor numérico 12288.

            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = true;

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;

        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var cookie = HttpContext.Current.Request.Cookies["JWT"];
            if (cookie == null) return;

            try
            {
                var principal = JwtManager.ValidateToken(cookie.Value);
                HttpContext.Current.User = principal;
                Thread.CurrentPrincipal = principal;
            }
            catch
            {
                // token inválido o expirado → no se autentica
            }
        }

    }
}
