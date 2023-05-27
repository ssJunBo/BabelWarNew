using Common;
using Managers;
using TMPro;
using UIExtension.ScrollRectExt;
using UnityEngine;

namespace Functions.Babel
{
    public class BabelItem : LoopItem
    {
        [SerializeField] private TextMeshProUGUI nameTxt;

        private int _levId;

        public void OnClickItem()
        {
            UiManager.Instance.CloseAllUiDialog();
            // 打开战斗选择界面
            UiManager.Instance.OpenUi(EUiID.UiFighting, _levId);
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