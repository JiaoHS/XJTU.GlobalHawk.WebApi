using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using System.Web.Mvc;
using XJTU.Common;
using XJTU.Model;
using XJTU.Service.Contract;

namespace XJTU.Web.Mvc.Controllers
{
    public class LoginController : BaseController<UserInfo>
    {
        private IUserInfoService _userInfoService;
        public LoginController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }


        public ActionResult HomeIndex()
        {
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [EnableCors("", "*", "*")]
        [HttpPost]
        public ActionResult Register(string usename, string password, string email)
        {
            int index = _userInfoService.Add(new UserInfo()
            {
                Email = email,
                IsDel = 1,
                PassWord = password,
                Phone = "",
                UserName = usename,
                CreateTime = System.DateTime.Now.ToString("yyyy-mm-dd HH:ss:mm")
            });
            var json = new { Data = index > 0 ? "ok" : "no" };
            return this.LargeJson(json, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 查询用户名是否存在
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckUserName(string username)
            {
            var ht = new Hashtable
               {
                {"username",username}
               };
            var list = _userInfoService.GetList("UserInfo.singleselect", ht);
                var json = list.Count > 0 ? "false" : "true";
            return Content(json);
        }

        [EnableCors("", "*", "*")]
        [HttpPost]
        public ActionResult Login(string usename, string password)
        {
            var ht = new Hashtable
               {
                {"username",usename},
                {"password",password}
               };
            var list = _userInfoService.GetList(ht);
            var json = new { Data = list.Count > 0 ? "ok" : "no" };
            return this.LargeJson(json, JsonRequestBehavior.AllowGet);
        }
    }
}
