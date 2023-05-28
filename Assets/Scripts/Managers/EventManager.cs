using System;
using System.Collections.Generic;
using Common;

namespace Managers
{
    public static class EventManager
    {
        /// <summary>
        /// 带返回参数的回调列表,参数类型为T，支持一对多
        /// </summary>
        private static readonly Dictionary<EventMessageType, List<Delegate>> Events = new();
 
        public delegate void Handler<in T>(T e);
        
        /// <summary>
        /// 注册事件，1个返回参数
        /// </summary>
        public static void Subscribe (EventMessageType eventMessageType, Handler<object> callback)
        {
            //eventName已存在
            if (Events.TryGetValue(eventMessageType, out var actions))
            {
                actions.Add(callback);
            }
            //eventName不存在
            else
            {
                actions = new List<Delegate> { callback };
                Events.Add(eventMessageType ,actions);
            }
        }

        public static void UnSubscribe(EventMessageType eventName, Handler<object> callback)
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
        public static void DispatchEvent(EventMessageType eventName, object baseEvent)
        {
            if (Events.ContainsKey(eventName))
            {
                Events.TryGetValue(eventName, out var actions);

                if (actions != null)
                {
                    foreach (var act in actions)
                    {
                        act?.DynamicInvoke(baseEvent);
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