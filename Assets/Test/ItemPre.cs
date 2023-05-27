using TMPro;
using UIExtension.ScrollRectExt;

namespace Test
{
    public class ItemPre : LoopItem
    {
        public TextMeshProUGUI mName;

        public override void SetUi(CellInfo cellInfo)
        {
            mName.text = cellInfo.Name;
        }
    }
}
