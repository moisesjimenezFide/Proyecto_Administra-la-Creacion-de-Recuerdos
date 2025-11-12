using System;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Filters   
{
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            response.Cache.SetValidUntilExpires(false);
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetNoStore();
            response.AppendHeader("Pragma", "no-cache");
            response.AppendHeader("Cache-Control", "no-store, must-revalidate, no-cache, max-age=0");
            base.OnActionExecuting(filterContext);
        }
    }
}
