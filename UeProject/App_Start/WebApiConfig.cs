using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace UeProject
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes


            //config.Routes.MapHttpRoute(
            //    name: "CryptoConvertFromTo",
            //    routeTemplate: "api/crypto/convert/{amount}/{from}/{to}",
            //    defaults:
            //    new
            //    {
            //        controller = "Cryptocurrency",
            //        action = "Convert"
            //    }
            //);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MapHttpAttributeRoutes();
        }
    }
}
