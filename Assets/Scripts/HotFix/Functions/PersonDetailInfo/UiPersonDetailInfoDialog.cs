using HotFix.Common;
using HotFix.Managers.Model;
using HotFix.UIBase;
using UnityEngine;

namespace HotFix.Functions.PersonDetailInfo
{
    public class UiPersonDetailInfoLogic : UiLogicBase
    {
        protected override string Path => "Prefabs/Functions/UIPerson/UiPersonDetailInfoDialog";
        public override EUiID UiId => EUiID.PersonDetailInfo;

        protected override EUiLayer UiLayer => EUiLayer.High_2D;

        private readonly CModelPlay _model;

        public UiPersonDetailInfoLogic(CModelPlay model)
        {
            _model = model;
        }
    }
    
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
