using HotFix.Pool;
using UnityEngine;

namespace HotFix.UIExtension.ScrollRectExt
{
    public abstract class LoopItem : PoolItemBase
    {
        public abstract void SetUi(CellInfo cellInfo);
    }
}