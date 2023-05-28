using _GameBase.UIBase;
using Common;
using Managers;
using Managers.Model;
using UIExtension;
using UnityEngine;

namespace Functions.PersonDetailInfo
{
    public class UiPersonDetailInfoLogic : UiLogicBase
    {
        public override EUiID UiId => EUiID.UiPersonDetailInfo;

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
