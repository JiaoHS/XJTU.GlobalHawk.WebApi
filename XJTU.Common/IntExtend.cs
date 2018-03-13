using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XJTU.Common
{
    public static class IntExtend
    {
        #region UnitTime 转换
        //private static DateTime BaseTime = new DateTime(1970, 1, 1);

        /// <summary>   
        /// 将unixtime转换为.NET的DateTime   
        /// </summary>   
        /// <param name="timeStamp">秒数</param>   
        /// <returns>转换后的时间</returns>   
        public static DateTime GetDateTimeByUnixTime(this int timeStamp)
        {
            return GetDateTimeByUnixTime(timeStamp.GetLong());
        }
        /// <summary>   
        /// 将unixtime转换为.NET的DateTime   
        /// </summary>   
        /// <param name="timeStamp">秒数</param>   
        /// <returns>转换后的时间</returns>   
        public static DateTime GetDateTimeByUnixTime(this int? timeStamp)
        {
            return GetDateTimeByUnixTime(timeStamp.GetLong());
        }

        /// <summary>   
        /// 将unixtime转换为.NET的DateTime   
        /// </summary>   
        /// <param name="timeStamp">秒数</param>   
        /// <returns>转换后的时间</returns>   
        public static DateTime GetDateTimeByUnixTime(this long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>   
        /// 将.NET的DateTime转换为unix time   
        /// </summary>   
        /// <param name="dateTime">待转换的时间</param>   
        /// <returns>转换后的unix time</returns>   
        public static long GetUnixTime(this DateTime dateTime)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (long)(dateTime - startTime).TotalSeconds;
        }
        #endregion
    }

}
