using System;
using System.Collections.Generic;
using HotFix.Common;

namespace HotFix.Managers
{
    public static class EventManager
    {
        /// <summary>
        /// 带返回参数的回调列表,参数类型为T，支持一对多
        /// </summary>
        private static readonly Dictionary<EventMessageType, List<Delegate>> Events = new Dictionary<EventMessageType, List<Delegate>>();
 
        /// <summary>
        /// 注册事件，1个返回参数
        /// </summary>
        public static void Subscribe<T> (EventMessageType EventMessageType, Action<T> callback)
        {
            //eventName已存在
            if (Events.TryGetValue(EventMessageType, out var actions))
            {
                actions.Add(callback);
            }
            //eventName不存在
            else
            {
                actions = new List<Delegate> { callback };
                Events.Add(EventMessageType ,actions);
            }
        }
 
        /// <summary>
        /// 注册事件，不带返回参数
        /// </summary>
        public static void Subscribe(EventMessageType eventName, Action callback)
        {
            //eventName已存在
            if (Events.TryGetValue(eventName, out var actions))
            {
                actions.Add(callback);
            }
            //eventName不存在
            else
            {
                actions = new List<Delegate> { callback };
                Events.Add(eventName, actions);
            }
        }
 
        /// <summary>
        /// 移除事件
        /// </summary>
        public static void UnSubscribe<T>(EventMessageType eventName, Action<T> callback)
        {
            if (Events.TryGetValue(eventName, out var actions))
            {
                actions.Remove(callback);
                if (actions.Count == 0)
                {
                    Events.Remove(eventName);
                }
            }
        }
     
        public static void UnSubscribe(EventMessageType eventName, Action callback)
        {
            if (Events.TryGetValue(eventName, out var actions))
            {
                actions.Remove(callback);
                if (actions.Count == 0)
                {
                    Events.Remove(eventName);
                }
            }
        }
     
        /// <summary>
        /// 派发事件
        /// </summary>
        public static void DispatchEvent<T>(EventMessageType eventName, T arg)
        {
            if (Events.ContainsKey(eventName))
            {
                Events.TryGetValue(eventName, out var actions);

                if (actions != null)
                {
                    foreach (var act in actions)
                    {
                        act.DynamicInvoke(arg);
                    }
                }
            }
        }
        /// <summary>
        /// 派发事件，不带参数
        /// </summary>
        public static void DispatchEvent(EventMessageType eventName)
        {
            if (Events.ContainsKey(eventName))
            {
                Events.TryGetValue(eventName, out var actions);

                if (actions != null)
                {
                    foreach (var act in actions)
                    {
                        act.DynamicInvoke();
                    }   
                }
            }
        }
         
        /// <summary>
        /// 移除全部事件
        /// </summary>
        public static void RemoveAllEvents ()
        {
            Events.Clear();
        }
    }
}