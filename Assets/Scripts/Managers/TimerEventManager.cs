using System;
using System.Collections.Generic;
using System.Linq;
using _GameBase;
using Tools;
using UnityEngine;

namespace Managers
{
    public class TimerEventManager : MonoSingleton<TimerEventManager>
    {
        // 计时器列表 存储所有开启的计时器 计时完成后 从列表中移除
        private readonly List<CallBackInfo> _mLtCallbackInfo = new List<CallBackInfo>();
        // 计时完成列表 短暂存完成计时的 info 
        private readonly List<CallBackInfo> _mLtFinish = new List<CallBackInfo>();

        // 受time scale 影响倒计时 
        private readonly List<CallBackInfo> _mLtCallbackInfoTimeScale = new List<CallBackInfo>();
        private readonly List<CallBackInfo> _mLtFinishTimeScale = new List<CallBackInfo>();
        
        // 延时几帧后执行callback
        public void DelaySeconds(float duration, Action callback)
        {
            CallBackInfo info = new CallBackInfo { Duration = duration, eventFinishAct = callback };
            _mLtCallbackInfo.Add(info);
        }

        public void DelayFrames(int howManyFrames, Action callback)
        {
            FrameCallBackInfo info = new FrameCallBackInfo { howManyFrames = howManyFrames, eventFinishAct = callback };
            _mLtCallbackInfo.Add(info);
        }

        // 延时 受time scale影响
        public void DelaySecondsTimeScale(float duration, Action callback)
        {
            TimeScaleCallBackInfo info = new TimeScaleCallBackInfo { Duration = duration, eventFinishAct = callback };
            _mLtCallbackInfoTimeScale.Add(info);
        }
        
        private void FixedUpdate()
        {
            if (_mLtCallbackInfoTimeScale != null && _mLtCallbackInfoTimeScale.Count > 0)
            {
                foreach (var aInfo in _mLtCallbackInfoTimeScale.Where(aInfo => aInfo != null))
                {
                    if (aInfo.TickCheckFinish())
                    {
                        _mLtFinishTimeScale.Add(aInfo);
                    }
                }

                foreach (var aInfo in _mLtFinishTimeScale)
                {
                    aInfo?.Finish();

                    _mLtCallbackInfoTimeScale.Remove(aInfo);
                }

                _mLtFinishTimeScale.Clear();
            }
        }

        private void Update()
        {
            if (_mLtCallbackInfo != null && _mLtCallbackInfo.Count > 0)
            {
                foreach (var aInfo in _mLtCallbackInfo.Where(aInfo => aInfo != null))
                {
                    if (aInfo.TickCheckFinish())
                    {
                        _mLtFinish.Add(aInfo);
                    }
                }

                foreach (var aInfo in _mLtFinish)
                {
                    aInfo?.Finish();

                    _mLtCallbackInfo.Remove(aInfo);
                }

                _mLtFinish.Clear();
            }
        }

        public void Clear()
        {
            
        }
    }

    /// <summary>
    /// 常规倒计时基类
    /// </summary>
    public class CallBackInfo
    {
        protected float mDuration;

        public float Duration
        {
            set
            {
                mDuration = value;
                EndTime = (int)(mDuration * 1000) + CTools.TickCount();
            }
        }

        public Action eventFinishAct;

        private int EndTime { get; set; }

        /// <summary>
        /// 每帧检测 是否完成
        /// </summary>
        public virtual bool TickCheckFinish()
        {
            return CTools.TickCount() > EndTime;
        }

        public void Finish()
        {
            eventFinishAct?.Invoke();
        }
    }

    /// <summary>
    /// 延迟帧 信息类
    /// </summary>
    public class FrameCallBackInfo : CallBackInfo
    {
        public int howManyFrames;

        #region override函数

        public override bool TickCheckFinish()
        {
            howManyFrames--;

            return howManyFrames < 0;
        }

        #endregion
    }
    
    /// <summary>
    /// 受time scale影响 倒计时 信息
    /// </summary>
    public class TimeScaleCallBackInfo : CallBackInfo
    {
        private float _time;
        public override bool TickCheckFinish()
        {
            _time += Time.fixedDeltaTime;

            return _time >= mDuration;
        }
    }
}
