using System;

namespace _GameBase
{
    public class Logger: Singleton<Logger>
    {
        public void Debug(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Warning(string msg)
        {
            UnityEngine.Debug.LogWarning(msg);
        }

        public void Error(string msg)
        {
            UnityEngine.Debug.LogError(msg);
        }
        
        public void Error(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

    
    }
}