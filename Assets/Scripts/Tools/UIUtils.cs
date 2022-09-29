using UnityEngine;

namespace Tools
{
    public static class UIUtils
    {
        /// <summary>
        /// 设置物体显隐
        /// </summary>
        /// <param name="go"></param>
        /// <param name="bActive"></param>
        public static void SetActive(GameObject go, bool bActive)
        {
            if (go == null)
                return;
            
            if (go.activeSelf != bActive)
                go.SetActive(bActive);
        }
    }
}