using IBatisNet.Common.Utilities;
using IBatisNet.DataAccess.Configuration;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using XJTU.Web.Mvc;
using XJTU.Web.Mvc.Ioc;

namespace XJTU.Web
{
    public class MvcApplication : System.Web.HttpApplication
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
            HttpConfiguration rou = new HttpConfiguration();
            AreaRegistration.RegisterAllAreas();
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            RouteConfig.Register(rou);
            MvcHandler.DisableMvcResponseHeader = true;

            container = new UnityContainer();
            IControllerFactory controllerFactory = new IocControllerFactory(container);
            RegistManager.RegistAll(container);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);


            builder = new DomDaoManagerBuilder();
            builder.ConfigureAndWatch(daoConfig, new ConfigureHandler(OnConfigChange));
        }
    }
}
