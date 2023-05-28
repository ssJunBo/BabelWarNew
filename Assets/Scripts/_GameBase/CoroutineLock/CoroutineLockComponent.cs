using System.Collections.Generic;
using ET;
using UnityEngine;

namespace _GameBase
{
    public class CoroutineLockComponent: Singleton<CoroutineLockComponent>, ISingletonUpdate
    {
        private readonly List<CoroutineLockQueueType> _list = new(CoroutineLockType.Max);
        private readonly Queue<(int, long, int)> _nextFrameRun = new();

        public CoroutineLockComponent()
        {
            for (int i = 0; i < CoroutineLockType.Max; ++i)
            {
                CoroutineLockQueueType coroutineLockQueueType = new CoroutineLockQueueType(i);
                _list.Add(coroutineLockQueueType);
            }
        }

        public override void Dispose()
        {
            _list.Clear();
            _nextFrameRun.Clear();
        }

        public void Update()
        {
            // 循环过程中会有对象继续加入队列
            while (_nextFrameRun.Count > 0)
            {
                (int coroutineLockType, long key, int count) = _nextFrameRun.Dequeue();
                Notify(coroutineLockType, key, count);
            }
        }

        public void RunNextCoroutine(int coroutineLockType, long key, int level)
        {
            // 一个协程队列一帧处理超过100个,说明比较多了,打个warning,检查一下是否够正常
            if (level == 100)
            {
                Debug.LogWarning($"too much coroutine level: {coroutineLockType} {key}");
            }

            _nextFrameRun.Enqueue((coroutineLockType, key, level));
        }

        public async ETTask<CoroutineLock> Wait(int coroutineLockType, long key, int time = 60000)
        {
            CoroutineLockQueueType coroutineLockQueueType = _list[coroutineLockType];
            return await coroutineLockQueueType.Wait(key, time);
        }

        private void Notify(int coroutineLockType, long key, int level)
        {
            CoroutineLockQueueType coroutineLockQueueType = _list[coroutineLockType];
            coroutineLockQueueType.Notify(key, level);
        }
    }
}