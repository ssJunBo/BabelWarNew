using System.Collections.Generic;
using UIExtension.ScrollRectExt;
using UnityEngine;
using UnityEngine.Serialization;

namespace Test
{
    public class Test : MonoBehaviour
    {
        [FormerlySerializedAs("uiCircularSv")] public UiCircularScrollView uiCircularScrollView;
        void Start()
        {
            var tmpLis = new List<CellInfo>();

            for (int i = 0; i < 12; i++)
            {
                var info = new CellInfo
                {
                    Name = i.ToString()
                };
                tmpLis.Add(info);
            }

            uiCircularScrollView.Init();
            uiCircularScrollView.SetData(tmpLis);
        }

    }
}
