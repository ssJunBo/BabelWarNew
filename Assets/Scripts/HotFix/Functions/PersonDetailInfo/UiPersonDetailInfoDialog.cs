using HotFix.FuncLogic;
using HotFix.UIBase;
using UnityEngine;

namespace HotFix.Functions.PersonDetailInfo
{
    public class UiPersonDetailInfoDialog : UiDialogBase
    {
        [SerializeField] private PersonInfoPanel personInfoPanel;
        
        private UiPersonDetailInfoLogic uiPersonDetailInfoLogic;

        public void OnClickStartBtn()
        {
            
        }

        public override void Init()
        {
            uiPersonDetailInfoLogic = (UiPersonDetailInfoLogic) UiLogic;
        }

        public override void ShowFinished()
        {
            // 刷新UI
            personInfoPanel.SetData();
        }
    }
}
