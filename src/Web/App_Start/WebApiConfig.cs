using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Dimensional.TinyReturns.Web.Controllers;
using Newtonsoft.Json.Serialization;

namespace Dimensional.TinyReturns.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();


            config.Routes.MapHttpRoute(
                name: "PortfolioPerformanceApi",
                routeTemplate: "portfolioDetails/{attribute}/{id}",
                defaults: new { controller="PortfolioDetails", attribute= RouteParameter.Optional, id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().FirstOrDefault();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

        }
    }
}
