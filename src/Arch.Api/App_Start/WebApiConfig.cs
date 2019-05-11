using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SimpleInjector;

namespace Arch.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            SimpleInjectorInitializer.Initialize(new Container(), config);
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
