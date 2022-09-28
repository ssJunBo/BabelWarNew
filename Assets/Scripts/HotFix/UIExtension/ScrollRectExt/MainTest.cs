using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HotFix.UIExtension.ScrollRectExt
{
    public class MainTest : MonoBehaviour
    {
        [FormerlySerializedAs("uiCircularSv")] public UiCircularScrollView uiCircularScrollView;

        void Start()
        {
            var tmpLis = new List<CellInfo>();

            for (int i = 0; i < 40; i++)
            {
                CellInfo info = new CellInfo();
                info.Name = i.ToString();
                tmpLis.Add(info);
            }

            uiCircularScrollView.Init();
            uiCircularScrollView.SetData(tmpLis);
        }
    }
}
