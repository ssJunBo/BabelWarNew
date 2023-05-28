using _GameBase.UIBase;
using Common;
using Managers;
using Managers.Model;
using UnityEngine;

namespace Functions.Main
{
    public  class UiMainLogic : UiLogicBase
    {
        public override EUiID UiId => EUiID.UiMain;
        public CModelPlay ModelPlay { get; }

        public UiMainLogic(CModelPlay modelPlay)
        {
            ModelPlay = modelPlay;
        }

        public override void Open(params object[] data)
        {
            base.Open(data);
        }
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
            UIManager.Instance.OpenUi(EUiID.UiSetting);
        }

        public void OpenCardPackageDialog()
        {
            UIManager.Instance.OpenUi(EUiID.UICardPackage);
        }
        
        public void OpenHeroDialog()
        {
            UIManager.Instance.OpenUi(EUiID.HeroPackage);
        }
    }
}
