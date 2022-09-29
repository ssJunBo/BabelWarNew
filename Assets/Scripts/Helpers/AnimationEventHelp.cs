using System;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    /// <summary>
    /// 动画事件帧辅助脚本
    /// </summary>
    public class AnimationEventHelp : MonoBehaviour
    {
        public Dictionary<string, Action> actDict = new Dictionary<string, Action>();
        
        public void AnimationCallBack(string name)
        {
            if (actDict.ContainsKey(name))
            {
                actDict[name]?.Invoke();
            }
        }
    }
}
