using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleInjector;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Arch.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var formatters = config.Formatters;
            formatters.Remove(formatters.XmlFormatter);

            var jsonSettings = formatters.JsonFormatter.SerializerSettings;
            jsonSettings.Formatting = Formatting.Indented;
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

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
