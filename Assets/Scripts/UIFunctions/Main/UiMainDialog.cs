using _GameBase;
using Common;
using Managers;
using UnityEngine;

namespace UIFunctions
{
    public  class UiMainLogic : UiLogicBase
    {
        public override EUiID UiId => EUiID.UIMain;
    }
    
    public class UiMainDialog : UiDialogBase
    {
        [SerializeField] private UiInfoPanel uiInfoPanel;

        private UiMainLogic _uiLogic;

        public override void Init()
        {
            _uiLogic = (UiMainLogic)UiLogic;
        }

        public override void ShowFinished()
        {
            // TODO 个人信息
            uiInfoPanel.SetData(_uiLogic);
        }

        public override void Release()
        {
            uiInfoPanel.Clear();
            base.Release();
        }

        public void OpenSettingDialog()
        {
            UIManager.Instance.OpenUi(EUiID.UISetting);
        }

        public void OpenCardPackageDialog()
        {
            UIManager.Instance.OpenUi(EUiID.UICardPackage);
        }
        
        public void OpenHeroDialog()
        {
            UIManager.Instance.OpenUi(EUiID.UIHeroPackage);
        }
    }
}
