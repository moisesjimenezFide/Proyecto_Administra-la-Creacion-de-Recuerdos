using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            StripeConfiguration.ApiKey = "sk_test_51RpDQn4CTUg5t4Leo5Mp1midGwRbLfoHhhndiSwNa1ns5mORrHjQxYPWAiJdCi7ePnP8TQm5XI500rQTxcfvITju00UVnxS9z0";
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
