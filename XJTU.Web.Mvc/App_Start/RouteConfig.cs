using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Routing;

namespace XJTU.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );   
        }
        public static void Register(HttpConfiguration config)
        {
            //跨域配置
            var allowOrigins = ConfigurationManager.AppSettings["cors_allowOrigins"];
            var cors = new EnableCorsAttribute(allowOrigins, "*", "*");

            //启用跨域　　
            config.EnableCors(cors);
        }
    }
}
