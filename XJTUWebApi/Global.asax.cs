using IBatisNet.Common.Utilities;
using IBatisNet.DataAccess.Configuration;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using XJTUWebApi.Ioc;

namespace XJTUWebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static IUnityContainer container;
        public static IUnityContainer Container
        {
            get { return container; }
        }

        private string daoConfig = "dao.config";
        private DomDaoManagerBuilder builder;

        private void OnConfigChange(object obj)
        {
            if (builder == null)
                builder = new DomDaoManagerBuilder();
            builder.Configure(daoConfig);
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            container = new UnityContainer();
            IControllerFactory controllerFactory = new IocControllerFactory(container);
            RegistManager.RegistAll(container);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);

            builder = new DomDaoManagerBuilder();
            builder.ConfigureAndWatch(daoConfig, new ConfigureHandler(OnConfigChange));
        }
    }
    public class CustomApiExplorer : ApiExplorer
    {
        public CustomApiExplorer(HttpConfiguration configuration) : base(configuration)
        {
        }

        public override bool ShouldExploreAction(string actionVariableValue, HttpActionDescriptor actionDescriptor, IHttpRoute route)
        {
            return base.ShouldExploreAction(actionVariableValue, actionDescriptor, route);
        }

        public override bool ShouldExploreController(string controllerVariableValue, HttpControllerDescriptor controllerDescriptor, IHttpRoute route)
        {
            return base.ShouldExploreController(controllerVariableValue, controllerDescriptor, route);
        }
    }
}
