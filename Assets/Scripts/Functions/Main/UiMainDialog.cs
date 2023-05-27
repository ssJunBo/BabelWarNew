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

        public override void Open()
        {
            base.Open();
            GeneratePersonDetailData();
        }

        private void GeneratePersonDetailData()
        {
            
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
            UiManager.Instance.OpenUi(EUiID.UiSetting);
        }

        public void OpenCardPackageDialog()
        {
            UiManager.Instance.OpenUi(EUiID.UICardPackage);
        }
        
        public void OpenHeroDialog()
        {
            UiManager.Instance.OpenUi(EUiID.HeroPackage);
        }
    }
}
