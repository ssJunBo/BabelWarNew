using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HotFix.UIExtension.ScrollRectExt
{
    public class MainTest : MonoBehaviour
    {
        [FormerlySerializedAs("uiCircularScrollView")] public UiCircularSv uiCircularSv;

        void Start()
        {
            var tmpLis = new List<CellInfo>();

            for (int i = 0; i < 40; i++)
            {
                CellInfo info = new CellInfo();
                info.Name = i.ToString();
                tmpLis.Add(info);
            }

            uiCircularSv.Init();
            uiCircularSv.SetData(tmpLis);
        }
    }
}
