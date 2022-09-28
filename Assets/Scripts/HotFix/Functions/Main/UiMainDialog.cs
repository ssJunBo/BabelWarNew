using HotFix.Common;
using HotFix.Managers.Model;
using HotFix.UIBase;
using UnityEngine;

namespace HotFix.Functions.Main
{
    public  class UiMainLogic : UiLogicBase
    {
        protected override string Path => "Prefabs/Functions/UiMainDialog";
        public override EUiID UiId => EUiID.Main;
        public CModelPlay Model { get; }

        public UiMainLogic(CModelPlay model)
        {
            Model = model;
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

        private UiMainLogic logic;

        public override void Init()
        {
            logic = (UiMainLogic)UiLogic;
        }

        public override void ShowFinished()
        {
            // TODO 个人信息
            uiInfoPanel.SetData(logic.Model);
        }

        public override void Release()
        {
            uiInfoPanel.Clear();
            base.Release();
        }
    }
}
