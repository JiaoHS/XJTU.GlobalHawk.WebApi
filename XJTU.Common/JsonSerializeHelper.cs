using System;


namespace XJTU.Common
{
    /// <summary>
    /// 序列化和反序列化的帮助类
    /// </summary>
    public static class JsonSerializeHelper
    {
        #region Json
        public static int MaxJsonLength { get; set; }

        static JsonSerializeHelper()
        {
            MaxJsonLength = 2147483644;
        }

        public static System.Web.Mvc.JsonResult LargeJson(this System.Web.Mvc.Controller controlador, object data)
        {
            return new System.Web.Mvc.JsonResult()
            {
                Data = data,
                MaxJsonLength = MaxJsonLength,
            };
        }
        public static System.Web.Mvc.JsonResult LargeJson(this System.Web.Mvc.Controller controlador, object data, System.Web.Mvc.JsonRequestBehavior behavior)
        {
            return new System.Web.Mvc.JsonResult()
            {
                Data = data,
                JsonRequestBehavior = behavior,
                MaxJsonLength = MaxJsonLength
            };
        }
        //TODO: You can add more overloads, the controller class has 6 overloads
        #endregion
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">要序列化的实体</param>
        /// <returns>序列化后的字符串</returns>
        public static string SerializeToJson(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Object DeserializeFromJson(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeFromJson<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
    }
}
