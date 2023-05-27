using System;

namespace _GameBase
{
    public interface ISingleton: IDisposable
    {
        void Register();
        void Destroy();
    }
    
    public abstract class Singleton<T>: ISingleton where T: Singleton<T>, new()
    {
        private static T instance;
        public static T Instance => instance ??= new T();

        void ISingleton.Register()
        {
            if (instance != null)
            {
                throw new Exception($"singleton register twice! {typeof (T).Name}");
            }
            instance = (T)this;
        }

        void ISingleton.Destroy()
        {
            
            instance.Dispose();
            instance = null;
        }

        public virtual void Dispose()
        {
        }
    }
}