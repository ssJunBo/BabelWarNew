using HotFix.Common;
using HotFix.Managers.Model;
using HotFix.UIBase;
using UnityEngine;

namespace HotFix.Functions.Main
{
    public  class UiMainLogic : UiLogicBase
    {
        protected override string Path => "Prefabs/Functions/UiMainDialog";
        protected override EUiID UiId => EUiID.Main;
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
            uiInfoPanel.SetData(_uiLogic.ModelPlay);
        }

        public override void Release()
        {
            uiInfoPanel.Clear();
            base.Release();
        }

        public void OpenSettingDialog()
        {
            _uiLogic.ModelPlay.UiSettingLogic.Open();
        }
    }
}
