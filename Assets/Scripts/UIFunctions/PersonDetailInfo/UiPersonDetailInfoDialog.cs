using _GameBase;
using Common;
using Managers;
using UIExtension;
using UnityEngine;

namespace UIFunctions
{
    public class UiPersonDetailInfoLogic : UiLogicBase
    {
        public override EUiID UiId => EUiID.UIPersonDetailInfo;
    }
    
    public class UiPersonDetailInfoDialog : UiDialogBase
    {
        [SerializeField] private PersonInfoPanel personInfoPanel;
        [SerializeField] private ExpandButton closeBtn;

        
        private UiPersonDetailInfoLogic uiPersonDetailInfoLogic;

        public override void Init()
        {
            uiPersonDetailInfoLogic = (UiPersonDetailInfoLogic) UiLogic;
            
            closeBtn.onClick.AddListener(Close);
        }

        public override void ShowFinished()
        {
            // 刷新UI
            personInfoPanel.SetData(DataManager.Instance.PersonInfo);
        }
    }
}
