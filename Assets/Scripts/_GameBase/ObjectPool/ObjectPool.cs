﻿using System;
using System.Collections.Generic;

namespace _GameBase
{
    public class ObjectPool: Singleton<ObjectPool>
    {
        private readonly Dictionary<Type, Queue<object>> pool = new();
        
        public T Fetch<T>() where T: class
        {
            return this.Fetch(typeof (T)) as T;
        }

        public object Fetch(Type type)
        {
            if (!pool.TryGetValue(type, out var queue))
            {
                return Activator.CreateInstance(type);
            }

            if (queue.Count == 0)
            {
                return Activator.CreateInstance(type);
            }
            return queue.Dequeue();
        }

        public void Recycle(object obj)
        {
            Type type = obj.GetType();
            if (!pool.TryGetValue(type, out var queue))
            {
                queue = new Queue<object>();
                pool.Add(type, queue);
            }

            // 一种对象最大为1000个
            if (queue.Count > 1000)
            {
                return;
            }
            queue.Enqueue(obj);
        }
    }
}