﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XJTU.Common
{
    public static class SendEmailHelper
    {
        /// <summary>
        /// 发送邮件方法
        /// </summary>
        /// <param name="sendTo">接受者</param>
        /// <param name="content">发送内容</param>
        /// <param name="title">标题</param>
        /// <param name="warnType">发送优先级</param>
        /// <param name="emaliType">邮件类型</param>
        /// <param name="jsonType">返回参数类型</param>
        /// <param name="Errormessage">发送信息</param>
        /// <returns>返回是否成功标识及信息</returns>
        public static bool SendEmail(string sendTo, string content, string title, WarnType warnType, EmaliType emaliType, JsonType jsonType, out string Errormessage)
        {
            object b = null;
            Errormessage = string.Empty;
            string emailContext = content;
            const string pv = "1.0.0";
            var url = System.Configuration.ConfigurationManager.AppSettings["SendEmailUrl"];
            var sign = Common.Md5Helper.Md5(content + pv + "111");
            var data =
                string.Format(
                    "sendTo={0}&content={1}&title={2}&warnType={3}&emaliType={4}&jsonType={5}&sign={6}&pv={7}", sendTo, emailContext, title, (int)warnType, (int)emaliType, (int)jsonType, sign, pv);
            try
            {
                string ret;
                HttpWebAsk.Post(url, data, out ret);
                trade t = Common.JsonSerializeHelper.DeserializeFromJson<trade>(ret);
                if (t.id == 1)
                {
                    b = true;
                    Errormessage = t.comment;
                }
                else
                {
                    b = false;
                    Errormessage = t.comment;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return (bool)b;
        }

    }
    public class trade
    {
        public int id { get; set; }
        public string comment { get; set; }
        public string responseType { get; set; }
        public DateTime tradeTime { get; set; }
    }
    /// <summary>
    /// 发送优先级
    /// </summary>
    public enum WarnType
    {
        [Description("需要等到下午16点统一发")]
        Later,
        [Description("表示立刻发送")]
        RightNow
    }
    /// <summary>
    /// 邮件类型
    /// </summary>
    public enum EmaliType
    {
        [Description("警告类型的邮件")]
        Warning
        ,
        [Description("一般是业务邮")]
        Normal
    }
    /// <summary>
    /// 返回参数类型
    /// </summary>
    public enum JsonType
    {
        [Description("xml格式的内容")]
        Xml
        ,
        [Description(" json格式的内容")]
        Json
    }
}
