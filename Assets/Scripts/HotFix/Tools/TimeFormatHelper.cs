using System;

namespace HotFix.Tools
{
    public class TimeFormatHelper
    {
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
    }
}