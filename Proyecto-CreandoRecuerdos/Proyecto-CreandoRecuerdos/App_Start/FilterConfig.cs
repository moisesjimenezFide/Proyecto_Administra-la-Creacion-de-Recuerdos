using System.Web;
using System.Web.Mvc;
using Proyecto_CreandoRecuerdos.Filters; 

namespace Proyecto_CreandoRecuerdos
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new NoCacheAttribute());
            filters.Add(new JwtSessionSyncFilter());
            filters.Add(new RenewJwtFilter());
            filters.Add(new JwtCookieAuthorizeFilter());
        }
    }
}
