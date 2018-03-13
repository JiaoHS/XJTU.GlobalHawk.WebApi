using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XJTU.DataAccess;
using XJTU.DataAccess.Contract;
using XJTU.Model;
using XJTU.Service;
using XJTU.Service.Contract;

namespace WebApi.Controllers
{
    public class TestController : ApiController
    {
        private IUserInfoService _userInfoService;
        private ITaskResultService _taskResultService;

        public TestController()
        {
            _userInfoService = new UserInfoService(new UserInfoDao());
            _taskResultService = new TaskResultService(new TaskResultDao());
        }

        //private IUserInfoService _userInfoService;
        //public TestController(IUserInfoService userInfoService)
        //{
        //    _userInfoService = userInfoService;
        //}

        [HttpGet]
        [Route("api/TestApi/CheckUserName")]
        public IHttpActionResult CheckUserName()
        {
            var ht = new Hashtable() { { "startIndex", 1 }, { "pageSize", 15 } };
            var list = _taskResultService.GetList(ht).ToList();
            //var json = list.Count > 0 ? "false" : "true";
            return Json<List<TaskResult>>(list);
        }
    }
}
