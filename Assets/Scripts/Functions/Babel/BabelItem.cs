using Common;
using Managers;
using TMPro;
using UnityEngine;

namespace Functions.Babel
{
    public class BabelItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameTxt;

        private UiBabelLogic _uiLogic;

        public void Init(UiBabelLogic logic)
        {
            _uiLogic = logic;
        }

        private int levId;
        public void SetData(int index)
        {
            levId = index;
            nameTxt.text = index.ToString();
        }

        public void OnClickItem()
        {
            UiManager.Instance.CloseAllUiDialog();
            // 打开战斗选择界面
            UiManager.Instance.OpenUi(EUiID.Fighting, levId);
        }
    }
}