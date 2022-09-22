using HotFix.Common;
using HotFix.Managers.Model;
using HotFix.UIBase;

namespace HotFix.FuncLogic
{
    public class UiFightingLogic : UiLogicBase
    {
        protected override string Path => "Prefabs/Functions/UIFight/UiFightingDialog";
        public override EUiID UiId => EUiID.Fighting;

        protected override EUiLayer UiLayer => EUiLayer.High_2D;

        public CModelPlay modelPlay;
        public UiFightingLogic(CModelPlay modelPlay)
        {
            this.modelPlay = modelPlay;
        }
        
        public override void Open()
        {
            base.Open();
        }
    }
}