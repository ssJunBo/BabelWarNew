using System;

namespace _GameBase
{
    public static class Log
    {
        public static void Debug(string msg)
        {
            Logger.Instance.Debug(msg);
        }

        public static void Warning(string msg)
        {
            Logger.Instance.Warning(msg);
        }

        public static void Error(string msg)
        {
            Logger.Instance.Error(msg);
        }
        
        public static void Error(Exception e)
        {
            Logger.Instance.Error(e);
        }
    }
}