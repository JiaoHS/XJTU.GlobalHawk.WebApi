using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using XJTU.Common;
using XJTU.Model;
using XJTU.Service.Contract;

namespace XJTU.Web.Mvc.Controllers
{
    public class HomeController : BaseController<Car>
    {
        private ICarService _carService;
        public HomeController(ICarService carService)
        {
            _carService = carService;
        }


        public ActionResult HomeIndex()
        {
            return Json("ok", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        [EnableCors("", "*", "*")]
        [HttpGet]
        public ActionResult HomeList(string sn_id, int pageIndex, int pageSize, DateTime startTime, DateTime endTime)
        {
            var ht = new Hashtable() { { "sn", sn_id }, { "startIndex", (pageIndex - 1) * pageSize }, { "pageSize", pageSize }, { "startTime", startTime }, { "endTime", endTime } };
            var list = _carService.GetList(ht);
            ht = new Hashtable() { { "sn", sn_id } };
            var count = _carService.GetCarRecordCount(sn_id);
            var json = new { Data = list, totalCount = count };
            return this.LargeJson(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取车辆列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CarNumList(int pageIndex, int pageSize)
        {
            var ht = new Hashtable() { { "startIndex", (pageIndex - 1) * pageSize }, { "pageSize", pageSize } };
            var list = _carService.GetList("Car.GetCarNumList", ht);
            var json = new { Data = list };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CarSample(string sample, string snId)
        {
            var ht = new Hashtable() { { "snId", snId } };
            List<Car> listAll = new List<Car>();
            listAll = _carService.GetList("Car.GetCarNumListById", ht).ToList();
            //object obs = Newtonsoft.Json.JsonConvert.DeserializeObject(sample);
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            List<Behavior> objs = Serializer.Deserialize<List<Behavior>>(sample);
            List<Car> carList = new List<Car>();
            List<Behavior> behavior = new List<Behavior>();
            List<BehaviorList> behaviorList = new List<BehaviorList>();
            if (objs.Count > 0 && objs != null)
            {
                foreach (var item in objs)
                {
                    //1498312800000.ToString().Remove(1498312800000.ToString().Length-3)
                    carList = listAll.Where(a => DateTimeToTimestamp(a.time.AddSeconds(3)) >= Convert.ToInt64(item.Time) && DateTimeToTimestamp(a.time.AddSeconds(-3)) <= Convert.ToInt64(item.Time)).ToList();
                    behaviorList.Add(new BehaviorList() { lable = item.BehaviorId, sample = carList });
                }
            }
            ArrayList col_lst = null;
            ArrayList row_lst = null;
            ArrayList lst = new ArrayList();
            ArrayList labels = new ArrayList();
            // 行
            row_lst = new ArrayList();
            //两两组合
            ArrayList accxyListTrain = new ArrayList();
            ArrayList magxyListTrain = new ArrayList();
            ArrayList lnglatListTrain = new ArrayList();
            ArrayList memsxyListTrain = new ArrayList();

            ArrayList accxyListTest = new ArrayList();
            ArrayList magxyListTest = new ArrayList();
            ArrayList lnglatListTest = new ArrayList();
            ArrayList memsxyListTest = new ArrayList();
            //// 第一列

            //col_lst = new ArrayList();

            //col_lst.Add("a");// 第一行，第一列

            //col_lst.Add("bc");// 第一行，第二列
            //// 第二列

            //col_lst = new ArrayList();

            //col_lst.Add("d");// 第二行，第一列

            //col_lst.Add("ef");// 第二行，第二列
            //row_lst.Add(col_lst);
            int index = 0;
            foreach (var item in behaviorList)
            {
                index++;
                for (int i = 0; i < item.sample.Count; i++)
                {
                    if (index > behaviorList.Count / 4)
                    {
                        col_lst = new ArrayList();
                        //col_lst.Add(item.sample[i].time);
                        col_lst.Add(item.sample[i].accx);
                        col_lst.Add(item.sample[i].accy);

                        accxyListTrain.Add(col_lst);
                        //col_lst.Add(item.sample[i].accz);
                        col_lst = new ArrayList();
                        col_lst.Add(item.sample[i].lat);
                        col_lst.Add(item.sample[i].lng);
                        lnglatListTrain.Add(col_lst);
                        col_lst = new ArrayList();
                        col_lst.Add(item.sample[i].memsx);
                        col_lst.Add(item.sample[i].memsy);
                        //col_lst.Add(item.sample[i].memsz);
                        memsxyListTrain.Add(col_lst);
                        col_lst = new ArrayList();
                        col_lst.Add(item.sample[i].magx);
                        col_lst.Add(item.sample[i].magy);
                        //col_lst.Add(item.sample[i].magz);
                        magxyListTrain.Add(col_lst);

                        //labei一维数组，对应svm的Y
                        labels.Add(Convert.ToInt32(item.lable));
                    }
                    else
                    {
                        col_lst = new ArrayList();
                        //col_lst.Add(item.sample[i].time);
                        col_lst.Add(item.sample[i].accx);
                        col_lst.Add(item.sample[i].accy);

                        accxyListTest.Add(col_lst);
                        //col_lst.Add(item.sample[i].accz);
                        col_lst = new ArrayList();
                        col_lst.Add(item.sample[i].lat);
                        col_lst.Add(item.sample[i].lng);
                        lnglatListTest.Add(col_lst);
                        col_lst = new ArrayList();
                        col_lst.Add(item.sample[i].memsx);
                        col_lst.Add(item.sample[i].memsy);
                        //col_lst.Add(item.sample[i].memsz);
                        memsxyListTest.Add(col_lst);
                        col_lst = new ArrayList();
                        col_lst.Add(item.sample[i].magx);
                        col_lst.Add(item.sample[i].magy);
                        //col_lst.Add(item.sample[i].magz);
                        magxyListTest.Add(col_lst);
                    }
                }
                //sample数组，对应svm的X数组
                //lst.Add(row_lst);

            }
            //string behaviorJson = Serializer.Serialize(row_lst);
            string accxyListJsonTrain = Serializer.Serialize(accxyListTrain);
            string lnglatListJsonTrain = Serializer.Serialize(lnglatListTrain);
            string memsxyListJsonTrain = Serializer.Serialize(memsxyListTrain);
            string magxyListJsonTrain = Serializer.Serialize(magxyListTrain);

            string accxyListJsonTest = Serializer.Serialize(accxyListTest);
            string lnglatListJsonTest = Serializer.Serialize(lnglatListTest);
            string memsxyListJsonTest = Serializer.Serialize(memsxyListTest);
            string magxyListJsonTest = Serializer.Serialize(magxyListTest);
            string labelJson = Serializer.Serialize(labels);
            string name = DateTime.Now.ToString("yyyyMMddhhmmss");
            WriteFile("E:\\sampleAccxyTrain" + name + ".txt", accxyListJsonTrain);
            WriteFile("E:\\samplelnglatTrain" + name + ".txt", lnglatListJsonTrain);
            WriteFile("E:\\samplememsxyTrain" + name + ".txt", memsxyListJsonTrain);
            WriteFile("E:\\sampleAmagxyTrain" + name + ".txt", magxyListJsonTrain);

            WriteFile("E:\\sampleAccxyTest" + name + ".txt", accxyListJsonTest);
            WriteFile("E:\\samplelnglatTest" + name + ".txt", lnglatListJsonTest);
            WriteFile("E:\\samplememsxyTest" + name + ".txt", memsxyListJsonTest);
            WriteFile("E:\\sampleAmagxyTest" + name + ".txt", magxyListJsonTest);
            WriteFile("E:\\label" + name + ".txt", labelJson);
            //FileStream fs = new FileStream("E:\\sample.txt", FileMode.Create);
            ////获得字节数组
            //byte[] data = new UTF8Encoding().GetBytes(behaviorJson);
            ////开始写入
            //fs.Write(data, 0, data.Length);
            ////清空缓冲区、关闭流
            //fs.Flush();
            //fs.Close();
            string temp = string.Empty;
            if (behaviorList.Count > 0)
            {
                temp = "ok";
            }
            else
            {
                temp = "no";
            }
            var json = new { Data = temp };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, dateTime.Kind);
            return Convert.ToInt64((dateTime - start).TotalSeconds);
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
    }
    public class Behavior
    {
        public string Time { get; set; }
        public string BehaviorId { get; set; }
    }
    public class BehaviorList
    {
        public List<Car> sample { get; set; }
        public string lable { get; set; }
    }

}
