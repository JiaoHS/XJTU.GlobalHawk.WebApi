﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XJTU.Common
{
    public static class ObjectExtend
    {
        /// <summary>
        /// 转换成Int类型
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">转换失败默认值</param>
        /// <returns></returns>
        public static int ToInt(this string str, int defaultValue = 0)
        {
            int value;
            if (!int.TryParse(str, out value))
            {
                value = defaultValue;
            }
            return value;
        }

        public static long GetLong(this object obj)
        {
            if (obj == null || obj == DBNull.Value)
                //return long.Parse(obj.ToString());
                return 0;
            long l = 0;
            long.TryParse(obj.ToString(), out l);
            return l;
        }

        /// <summary>
        /// 时间戳转换成日期
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime GetTime(this string timeStamp)
        {
            long lTime = long.Parse(timeStamp);
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            time = startTime.AddMilliseconds(lTime);
            return time;
        }


        ///// <summary>
        ///// 转换成DateTime类型
        ///// </summary>
        ///// <param name="str"></param>
        ///// <param name="defaultValue">转换失败默认值</param>
        ///// <returns></returns>
        //public static DateTime ToDateTime(this string str, string defaultValue ="1999-1-1")
        //{
        //    DateTime value;
        //    if (!DateTime.TryParse(str, out value))
        //    {
        //        value = DateTime.Parse(defaultValue);
        //    }
        //    return value;
        //}
    }
}
