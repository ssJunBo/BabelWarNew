using HotFix.Common;
using HotFix.Managers.Model;
using HotFix.UIBase;

namespace HotFix.FuncLogic
{
    public class UiBabelLogic: UiLogicBase
    {
        protected override string Path => "Prefabs/Functions/UIBabel/UiBabelDialog";
        public override EUiID UiId  => EUiID.Babel;
        protected override EUiLayer UiLayer => EUiLayer.High_2D;

        public CModelPlay modelPlay;
        public UiBabelLogic(CModelPlay modelPlay)
        {
            this.modelPlay = modelPlay;
        }
        
        public override void Open()
        {
            base.Open();
        }
    }
}