using System.Collections.Generic;
using _GameBase;
using UnityEngine;

namespace ET
{
    public enum TimerClass
    {
        None,
        OnceTimer,
        OnceWaitTimer,
        RepeatedTimer,
    }

    public class TimerAction
    {
        public static TimerAction Create(long id, TimerClass timerClass, long startTime, long time, int type, object obj)
        {
            TimerAction timerAction = ObjectPool.Instance.Fetch<TimerAction>();
            timerAction.Id = id;
            timerAction.TimerClass = timerClass;
            timerAction.StartTime = startTime;
            timerAction.Object = obj;
            timerAction.Time = time;
            timerAction.Type = type;
            return timerAction;
        }

        public long Id;
        
        public TimerClass TimerClass;

        public object Object;

        public long StartTime;

        public long Time;

        public int Type;
        
        public void Recycle()
        {
            Id = 0;
            Object = null;
            StartTime = 0;
            Time = 0;
            TimerClass = TimerClass.None;
            Type = 0;
            ObjectPool.Instance.Recycle(this);
        }
    }

    public struct TimerCallback
    {
        public object Args;
    }

    public class TimerComponent: Singleton<TimerComponent>, ISingletonUpdate
    {
        /// <summary>
        /// key: time, value: timer id
        /// </summary>
        private readonly MultiMap<long, long> TimeId = new();

        private readonly Queue<long> timeOutTime = new();

        private readonly Queue<long> timeOutTimerIds = new();

        private readonly Dictionary<long, TimerAction> timerActions = new();

        private long idGenerator;

        // 记录最小时间，不用每次都去MultiMap取第一个值
        private long minTime = long.MaxValue;

        private long GetId()
        {
            return ++idGenerator;
        }

        private static long GetNow()
        {
            return TimeHelper.ClientFrameTime();
        }

        public void Update()
        {
            if (TimeId.Count == 0)
            {
                return;
            }

            long timeNow = GetNow();

            if (timeNow < minTime)
            {
                return;
            }

            foreach (KeyValuePair<long, List<long>> kv in TimeId)
            {
                long k = kv.Key;
                if (k > timeNow)
                {
                    minTime = k;
                    break;
                }

                timeOutTime.Enqueue(k);
            }

            while (timeOutTime.Count > 0)
            {
                long time = timeOutTime.Dequeue();
                var list = TimeId[time];
                for (int i = 0; i < list.Count; ++i)
                {
                    long timerId = list[i];
                    timeOutTimerIds.Enqueue(timerId);
                }
                TimeId.Remove(time);
            }

            while (timeOutTimerIds.Count > 0)
            {
                long timerId = timeOutTimerIds.Dequeue();

                if (!timerActions.Remove(timerId, out TimerAction timerAction))
                {
                    continue;
                }
                
                Run(timerAction);
            }
        }

        private void Run(TimerAction timerAction)
        {
            switch (timerAction.TimerClass)
            {
                case TimerClass.OnceTimer:
                {
                    // TODO 
                    // EventSystem.Instance.Invoke(timerAction.Type, new TimerCallback() { Args = timerAction.Object });
                    timerAction.Recycle();
                    break;
                }
                case TimerClass.OnceWaitTimer:
                {
                    ETTask tcs = timerAction.Object as ETTask;
                    tcs.SetResult();
                    timerAction.Recycle();
                    break;
                }
                case TimerClass.RepeatedTimer:
                {                    
                    long timeNow = GetNow();
                    timerAction.StartTime = timeNow;
                    AddTimer(timerAction);
                    // TODO 
                    // EventSystem.Instance.Invoke(timerAction.Type, new TimerCallback() { Args = timerAction.Object });
                    break;
                }
            }
        }

        private void AddTimer(TimerAction timer)
        {
            long tillTime = timer.StartTime + timer.Time;
            TimeId.Add(tillTime, timer.Id);
            timerActions.Add(timer.Id, timer);
            if (tillTime < minTime)
            {
                minTime = tillTime;
            }
        }

        public bool Remove(ref long id)
        {
            long i = id;
            id = 0;
            return Remove(i);
        }

        private bool Remove(long id)
        {
            if (id == 0)
            {
                return false;
            }

            if (!timerActions.Remove(id, out TimerAction timerAction))
            {
                return false;
            }
            timerAction.Recycle();
            return true;
        }

        public async ETTask WaitTillAsync(long tillTime, ETCancellationToken cancellationToken = null)
        {
            long timeNow = GetNow();
            if (timeNow >= tillTime)
            {
                return;
            }

            ETTask tcs = ETTask.Create(true);
            TimerAction timer = TimerAction.Create(GetId(), TimerClass.OnceWaitTimer, timeNow, tillTime - timeNow, 0, tcs);
            AddTimer(timer);
            long timerId = timer.Id;

            void CancelAction()
            {
                if (Remove(timerId))
                {
                    tcs.SetResult();
                }
            }

            try
            {
                cancellationToken?.Add(CancelAction);
                await tcs;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);
            }
        }

        public async ETTask WaitFrameAsync(ETCancellationToken cancellationToken = null)
        {
            await WaitAsync(1, cancellationToken);
        }

        public async ETTask WaitAsync(long time, ETCancellationToken cancellationToken = null)
        {
            if (time == 0)
            {
                return;
            }

            long timeNow = GetNow();

            ETTask tcs = ETTask.Create(true);
            TimerAction timer = TimerAction.Create(GetId(), TimerClass.OnceWaitTimer, timeNow, time, 0, tcs);
            AddTimer(timer);
            long timerId = timer.Id;

            void CancelAction()
            {
                if (Remove(timerId))
                {
                    tcs.SetResult();
                }
            }

            try
            {
                cancellationToken?.Add(CancelAction);
                await tcs;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);
            }
        }

        // 用这个优点是可以热更，缺点是回调式的写法，逻辑不连贯。WaitTillAsync不能热更，优点是逻辑连贯。
        // wait时间短并且逻辑需要连贯的建议WaitTillAsync
        // wait时间长不需要逻辑连贯的建议用NewOnceTimer
        public long NewOnceTimer(long tillTime, int type, object args)
        {
            long timeNow = GetNow();
            if (tillTime < timeNow)
            {
                Debug.LogError($"new once time too small: {tillTime}");
            }

            TimerAction timer = TimerAction.Create(GetId(), TimerClass.OnceTimer, timeNow, tillTime - timeNow, type, args);
            AddTimer(timer);
            return timer.Id;
        }

        public long NewFrameTimer(int type, object args)
        {
#if DOTNET
            return NewRepeatedTimerInner(100, type, args);
#else
            return NewRepeatedTimerInner(0, type, args);
#endif
        }

        /// <summary>
        /// 创建一个RepeatedTimer
        /// </summary>
        private long NewRepeatedTimerInner(long time, int type, object args)
        {
#if DOTNET
            if (time < 100)
            {
                throw new Exception($"repeated timer < 100, timerType: time: {time}");
            }
#endif
            
            long timeNow = GetNow();
            TimerAction timer = TimerAction.Create(GetId(), TimerClass.RepeatedTimer, timeNow, time, type, args);

            // 每帧执行的不用加到timerId中，防止遍历
            AddTimer(timer);
            return timer.Id;
        }

        public long NewRepeatedTimer(long time, int type, object args)
        {
            if (time < 100)
            {
                Debug.LogError($"time too small: {time}");
                return 0;
            }

            return NewRepeatedTimerInner(time, type, args);
        }
    }
}