using HotFix.Common;
using HotFix.Managers.Model;
using HotFix.UIBase;

namespace HotFix.FuncLogic
{
    public class UiMainLogic : UiLogicBase
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
}