using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XJTU.Common;
using XJTU.DataAccess;
using XJTU.Model;
using XJTU.Service;
using XJTU.Service.Contract;

namespace XJTUWebApi.Controllers
{
    /// <summary>
    /// 任务控制器
    /// </summary>
    public class TaskController : ApiController
    {
        private ITaskTableService _taskTableService;
        private ISensorService _sensorService;
        private ITaskResultService _taskResultService;
        /// <summary>
        /// 初始化
        /// </summary>
        public TaskController()
        {
            _taskTableService = new TaskTableService(new TaskTableDao());
            _sensorService = new SensorService(new SensorDao());
            _taskResultService = new TaskResultService(new TaskResultDao());
        }
        /// <summary>
        /// 获取所有task列表(分页)
        /// </summary>
        /// <param name="startIndex">第几页</param>
        /// <param name="pageSize">每页大小个数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Task/GetTaskTableList")]
        public IHttpActionResult GetTaskTableInfo(int startIndex, int pageSize)
        {
            var ht = new Hashtable() { { "startIndex", startIndex }, { "pageSize", pageSize } };
            var list = _taskTableService.GetList(ht).ToList();
            return Json<List<TaskTable>>(list);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="jsonModel">拼接成的json串jsonModel: JSON.stringify({ ID: "1",ID: "1"})</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Task/InsertTaskTable")]
        public IHttpActionResult InsertGetTaskTable(string jsonModel)
        {
            TaskTable oData = Newtonsoft.Json.JsonConvert.DeserializeObject<TaskTable>(jsonModel);
            var index = _taskTableService.Add(oData);
            return Ok(index > 0 ? "ok" : "no");
        }
        /// <summary>
        /// 通过id获取Sensor(分页)
        /// </summary>
        /// <param name="startIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Task/GetSensorList")]
        public IHttpActionResult GetSensorById(int startIndex, int pageSize)
        {
            var ht = new Hashtable() { { "startIndex", startIndex }, { "pageSize", pageSize } };
            var list = _sensorService.GetList(ht).ToList();
            return Json<List<Sensor>>(list);
        }
        /// <summary>
        /// 获取任务结果集合
        /// </summary>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="tableIndex">表的索引</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Task/TaskResultList")]
        public IHttpActionResult TaskResultList(int pageIndex, int pageSize, int tableIndex)
        {
            //Task_10_Result
            //先查询 传的参数tableIndex 是否 合法use LFA select Name from sysobjects where xtype='U'
            List<int> list = new List<int>();
            DataTable dt;
            int index = 0;
            try
            {
                dt = SqlHelper.ExecuteDataTable("use LFA select Name from sysobjects where xtype='U'");
                foreach (DataRow dr in dt.Rows)
                {
                    if (Convert.ToString(dr["Name"]).Contains("_"))
                    {
                        list.Add(Convert.ToInt32(Convert.ToString(dr["Name"]).TrimStart("Task_".ToCharArray()).TrimEnd("_Result".ToCharArray())));
                    }
                    else
                    {
                        continue;
                    }
                }
                index = list.OrderByDescending(a => a).ToList()[0];
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
                //log.Error("程序错误：" + ex.Message);
            }
            if (index < tableIndex)
            {
                return Ok("对象名 'Task_" + tableIndex + "_Result' 无效。");
            }
            else
            {
                string tableName = "Task_" + tableIndex.ToString() + "_Result";
                var ht = new Hashtable() { { "startIndex", pageIndex }, { "pageSize", pageSize }, { "tableName", tableName } };
                var listResult = _taskResultService.GetList(ht).ToList();
                return Json<List<TaskResult>>(listResult);
            }
            #region MyRegion

            //JavaScriptSerializer Serializer = new JavaScriptSerializer();
            //List<Rtt> listRtt = new List<Rtt>();
            //List<ViewModel> modelList = new List<ViewModel>();
            //string[] linkArr;
            //string[] linkArr2;
            //if (list.Count > 0)
            //{
            //    for (int i = 0; i < list.Count; i++)
            //    {
            //        linkArr = list[i].Link.Split(',');
            //        listRtt = new List<Rtt>();
            //        if (linkArr.Length > 0)
            //        {
            //            for (int j = 0; j < linkArr.Length; j++)
            //            {
            //                linkArr2 = linkArr[j].Split('#');
            //                listRtt.Add(new Rtt()
            //                {
            //                    Time = list[i].Time,
            //                    Ip = linkArr2[0],
            //                    Lat = linkArr2[1],
            //                    Lnt = linkArr2[2],
            //                    RttTime = linkArr2[3],
            //                    BandWith = linkArr2[4]
            //                });
            //            }
            //        }
            //        modelList.Add(new ViewModel()
            //        {
            //            Count = i,
            //            RttList = listRtt
            //        });
            //    }
            //    List<Rtt> listSample = new List<Rtt>();
            //    if (modelList.Count > 0)
            //    {
            //        string ip = string.Empty;
            //        string rttTime = string.Empty;
            //        DateTime time;
            //        int temp = 0;

            //        ArrayList col_lst = null;
            //        ArrayList ipList = new ArrayList();
            //        List<string> linkSameArr = new List<string>();
            //        List<Rtt> tempList = new List<Rtt>();
            //        do
            //        {
            //            for (int i = 0; i < modelList.Count; i++)
            //            {
            //                //tempList = modelList[i].RttList.Where(a=>a.Ip== linkSameArr[temp].ToString()).ToList();
            //                //if (linkSameArr.Count > 0)
            //                //{
            //                //    for (int n = 0; n < linkSameArr.Count; n++)
            //                //    {

            //                //    }
            //                //    if (modelList[i].RttList.Where(b => b.Ip == linkSameArr[temp - 1].ToString()).ToList().Count < 0)
            //                //    {
            //                //        break;
            //                //    }
            //                //    //modelList = modelList.Where(a => a.RttList.Where(b=>b.Ip== linkSameArr[temp-1].ToString()).ToList()).ToList();
            //                //}
            //                tempList = modelList[i].RttList;
            //                if (tempList.Count > 0)
            //                {
            //                    temp = temp > tempList.Count ? 31 : temp;
            //                    ip = tempList[temp].Ip;
            //                    rttTime = tempList[temp].RttTime;
            //                    time = tempList[temp].Time;

            //                    listSample.Add(new Rtt()
            //                    {
            //                        RttTime = rttTime,
            //                        Time = time,
            //                        Ip = ip
            //                    });
            //                }
            //                if (i == modelList.Count - 1)//循环结束
            //                {
            //                    for (int t = 0; t < 10; t++)//混入假数据
            //                    {
            //                        listSample.Add(new Rtt()
            //                        {
            //                            RttTime = "1000" + t.ToString(),
            //                            Time = DateTime.Now,
            //                            Ip = "192.168.1.1" + t.ToString()
            //                        });
            //                    }

            //                    var listRe = listSample.GroupBy(x => new
            //                    {
            //                        x.Ip,
            //                    }).Select(g => (new { ip = g.Key.Ip, count = g.Count() })).OrderByDescending(g => g.count).ToList();//取个数最多的list
            //                    linkSameArr.Add(listRe[0].ip);//取出数量最多的ip

            //                    listSample = listSample.Where(a => a.Ip == listRe[0].ip).ToList();
            //                    col_lst = new ArrayList();
            //                    ArrayList col_listy = new ArrayList();
            //                    foreach (var item in listSample)//组成2元组：时间和rtt
            //                    {
            //                        //col_lst.Add(item.sample[i].time);
            //                        col_lst = new ArrayList();
            //                        col_lst.Add(DateTimeToTimestamp(item.Time));
            //                        col_lst.Add(item.RttTime);
            //                        ipList.Add(col_lst);

            //                        //col_lst.Add(DateTimeToTimestamp(item.Time));
            //                        //col_listy.Add(item.RttTime);
            //                    }
            //                    //string listJsonx = Serializer.Serialize(col_lst);
            //                    //string listJsony = Serializer.Serialize(col_listy);
            //                    string name = DateTime.Now.ToString("yyyyMMddhhmmss");
            //                    //WriteFile("E:\\ipList " + listSample[0].Ip + "x.txt", listJsonx);
            //                    //WriteFile("E:\\ipList " + listSample[0].Ip + "y.txt", listJsony);

            //                    string listTrain = Serializer.Serialize(ipList);
            //                    WriteFile("E:\\ipList " + name + ".txt", listTrain);
            //                    temp++;
            //                    //保存

            //                    listSample = new List<Rtt>();
            //                }
            //            }
            //        } while (temp < 31);
            //    } 
            #endregion
        }
    }
}
