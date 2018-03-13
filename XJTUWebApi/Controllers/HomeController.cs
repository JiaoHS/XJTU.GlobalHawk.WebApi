using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XJTUWebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // return Redirect("/Swagger/ui/index");
            ViewBag.Title = "Home Page";
            //var desc = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
            return View();
        }
    }
}
