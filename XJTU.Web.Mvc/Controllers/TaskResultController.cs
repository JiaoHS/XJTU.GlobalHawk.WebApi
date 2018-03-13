using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using XJTU.Model;
using XJTU.Service.Contract;

namespace XJTU.Web.Mvc.Controllers
{
    public class TaskResultController : BaseController<TaskResult>
    {
        private ITaskResultService _taskResultService;
        public TaskResultController(ITaskResultService taskResultService)
        {
            _taskResultService = taskResultService;
        }


        public ActionResult HomeIndex()
        {
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult TaskResultList(int pageIndex, int pageSize)
        {
            //Task_10_Result
            var ht = new Hashtable() { { "startIndex", pageIndex }, { "pageSize", pageSize } };
            var list = _taskResultService.GetList(ht);
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            List<Rtt> listRtt = new List<Rtt>();
            List<ViewModel> modelList = new List<ViewModel>();
            string[] linkArr;
            string[] linkArr2;
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    linkArr = list[i].Link.Split(',');
                    listRtt = new List<Rtt>();
                    if (linkArr.Length > 0)
                    {
                        for (int j = 0; j < linkArr.Length; j++)
                        {
                            linkArr2 = linkArr[j].Split('#');
                            listRtt.Add(new Rtt()
                            {
                                Time = list[i].Time,
                                Ip = linkArr2[0],
                                Lat = linkArr2[1],
                                Lnt = linkArr2[2],
                                RttTime = linkArr2[3],
                                BandWith = linkArr2[4]
                            });
                        }
                    }
                    modelList.Add(new ViewModel()
                    {
                        Count = i,
                        RttList = listRtt
                    });
                }
                List<Rtt> listSample = new List<Rtt>();
                if (modelList.Count > 0)
                {
                    string ip = string.Empty;
                    string rttTime = string.Empty;
                    DateTime time;
                    int temp = 0;

                    ArrayList col_lst = null;
                    ArrayList ipList = new ArrayList();
                    List<string> linkSameArr = new List<string>();
                    List<Rtt> tempList = new List<Rtt>();
                    do
                    {
                        for (int i = 0; i < modelList.Count; i++)
                        {
                            //tempList = modelList[i].RttList.Where(a=>a.Ip== linkSameArr[temp].ToString()).ToList();
                            //if (linkSameArr.Count > 0)
                            //{
                            //    for (int n = 0; n < linkSameArr.Count; n++)
                            //    {

                            //    }
                            //    if (modelList[i].RttList.Where(b => b.Ip == linkSameArr[temp - 1].ToString()).ToList().Count < 0)
                            //    {
                            //        break;
                            //    }
                            //    //modelList = modelList.Where(a => a.RttList.Where(b=>b.Ip== linkSameArr[temp-1].ToString()).ToList()).ToList();
                            //}
                            tempList = modelList[i].RttList;
                            if (tempList.Count > 0)
                            {
                                temp = temp > tempList.Count ? 31 : temp;
                                ip = tempList[temp].Ip;
                                rttTime = tempList[temp].RttTime;
                                time = tempList[temp].Time;

                                listSample.Add(new Rtt()
                                {
                                    RttTime = rttTime,
                                    Time = time,
                                    Ip = ip
                                });
                            }
                            if (i == modelList.Count - 1)//循环结束
                            {
                                for (int t = 0; t < 10; t++)//混入假数据
                                {
                                    listSample.Add(new Rtt()
                                    {
                                        RttTime = "1000" + t.ToString(),
                                        Time = DateTime.Now,
                                        Ip = "192.168.1.1" + t.ToString()
                                    });
                                }

                                var listRe = listSample.GroupBy(x => new
                                {
                                    x.Ip,
                                }).Select(g => (new { ip = g.Key.Ip, count = g.Count() })).OrderByDescending(g => g.count).ToList();//取个数最多的list
                                linkSameArr.Add(listRe[0].ip);//取出数量最多的ip

                                listSample = listSample.Where(a => a.Ip == listRe[0].ip).ToList();
                                col_lst = new ArrayList();
                                ArrayList col_listy = new ArrayList();
                                foreach (var item in listSample)//组成2元组：时间和rtt
                                {
                                    //col_lst.Add(item.sample[i].time);
                                    col_lst = new ArrayList();
                                    col_lst.Add(DateTimeToTimestamp(item.Time));
                                    col_lst.Add(item.RttTime);
                                    ipList.Add(col_lst);

                                    //col_lst.Add(DateTimeToTimestamp(item.Time));
                                    //col_listy.Add(item.RttTime);
                                }
                                //string listJsonx = Serializer.Serialize(col_lst);
                                //string listJsony = Serializer.Serialize(col_listy);
                                string name = DateTime.Now.ToString("yyyyMMddhhmmss");
                                //WriteFile("E:\\ipList " + listSample[0].Ip + "x.txt", listJsonx);
                                //WriteFile("E:\\ipList " + listSample[0].Ip + "y.txt", listJsony);

                                string listTrain = Serializer.Serialize(ipList);
                                WriteFile("E:\\ipList " + name + ".txt", listTrain);
                                temp++;
                                //保存

                                listSample = new List<Rtt>();
                            }
                        }
                    } while (temp < 31);
                }
            }
            var json = new { Data = modelList };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public static long DateTimeToTimestamp(DateTime date)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            return (long)(date - startTime).TotalMilliseconds;
        }
        public static void WriteFile(string path, string strJson)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            //获得字节数组
            byte[] data = new UTF8Encoding().GetBytes(strJson);
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }
        public class ViewModel
        {
            public int Count { get; set; }
            public List<Rtt> RttList { get; set; }
        }
        public class Rtt
        {
            public DateTime Time { get; set; }
            public string RttTime { get; set; }

            public string Ip { get; set; }
            public string Lat { get; set; }
            public string Lnt { get; set; }
            public string BandWith { get; set; }
            public int Count { get; set; }
        }
    }
}
