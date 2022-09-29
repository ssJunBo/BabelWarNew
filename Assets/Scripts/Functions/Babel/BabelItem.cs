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

        public void SetData(int index)
        {
            nameTxt.text = index.ToString();
        }

        public void OnClickItem()
        {
            UiManager.Instance.CloseAllUiDialog();
            // 打开战斗选择界面
            _uiLogic.modelPlay.UiFightingLogic.Open();
        }
    }
}