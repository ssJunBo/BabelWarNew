using Common;
using Managers;
using TMPro;
using UIExtension.ScrollRectExt;
using UnityEngine;

namespace UIFunctions.Babel
{
    public class BabelItem : LoopItem
    {
        [SerializeField] private TextMeshProUGUI nameTxt;

        private int _levId;

        public void OnClickItem()
        {
            UIManager.Instance.CloseAllUiDialog();
            // 打开战斗选择界面
            UIManager.Instance.OpenUi(EUiID.UIFighting, _levId);
        }

        public override void SetUi(CellInfo cellInfo)
        {
            if (cellInfo is BabelInfo babelInfo)
            {
                _levId = babelInfo.LevelExcelItem.id;
                nameTxt.text = $"关卡 - {babelInfo.LevelExcelItem.id}";
            }
        }
    }
}