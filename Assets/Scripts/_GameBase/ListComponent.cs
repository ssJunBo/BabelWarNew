using System;
using System.Collections.Generic;

namespace _GameBase
{
    public class ListComponent<T>: List<T>, IDisposable
    {
        public static ListComponent<T> Create()
        {
            return ObjectPool.Instance.Fetch(typeof (ListComponent<T>)) as ListComponent<T>;
        }

        public void Dispose()
        {
            Clear();
            ObjectPool.Instance.Recycle(this);
        }
    }
}