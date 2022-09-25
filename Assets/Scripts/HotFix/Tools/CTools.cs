using System;
using System.Collections.Generic;

namespace HotFix.Tools
{
    public static class CTools
    {
        #region Time 先相关

        public static int TickCount()
        {
            int tick = Environment.TickCount; //毫秒
            if (tick < 0)
            {
                tick += Int32.MaxValue;
            }

            return tick;
        }

        /// <summary>
        /// 格式化时间 时 分 秒
        /// </summary>
        /// <param name="seconds">秒</param>
        /// <returns></returns>
        public static string FormatTime(float seconds)
        {
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(seconds));
            string str = "";
            if (ts.Hours > 0)
            {
                str = ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
            }

            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
            }

            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = "00:" + ts.Seconds.ToString("00");
            }

            return str;
        }

        #endregion


        #region 数据结构相关

        /// <summary>
        /// // 打乱 泛型列表项目
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> DisorderItems<T>(this List<T> list) 
        {
            List<T> newList = new List<T>();
            Random rand = new Random();
            foreach (var item in list)
            {
                newList.Insert(rand.Next(newList.Count), item);
            }

            return (newList);
        }

        #endregion
    }
}