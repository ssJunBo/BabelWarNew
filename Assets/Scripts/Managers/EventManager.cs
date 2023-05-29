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
        private static readonly Dictionary<EventMessageType, List<Delegate>> AllEventInfoDict = new();
 
        public delegate void Handler<in T>(T arg);
        
        /// <summary>
        /// 注册事件，1个返回参数
        /// </summary>
        public static void Subscribe (EventMessageType eventMessageType, Handler<object> handle)
        {
            //eventName已存在
            if (AllEventInfoDict.TryGetValue(eventMessageType, out var actions))
            {
                if (!actions.Contains(handle))
                {
                    actions.Add(handle);
                }
            }
            //eventName不存在
            else
            {
                actions = new List<Delegate> { handle };
                AllEventInfoDict.Add(eventMessageType ,actions);
            }
        }

        public static void UnSubscribe(EventMessageType eventName, Handler<object> handle)
        {
            if (AllEventInfoDict.TryGetValue(eventName, out var actions))
            {
                if (actions.Contains(handle))
                    actions.Remove(handle);
         
                if (actions.Count == 0)
                    AllEventInfoDict.Remove(eventName);
            }
        }

        /// <summary>
        /// 派发事件
        /// </summary>
        public static void DispatchEvent(EventMessageType eventName, object baseEvent)
        {
            if (AllEventInfoDict.ContainsKey(eventName))
            {
                AllEventInfoDict.TryGetValue(eventName, out var actions);

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
            AllEventInfoDict.Clear();
        }
    }
}