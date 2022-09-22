using HotFix.Common;
using HotFix.Managers.Model;
using HotFix.UIBase;

namespace HotFix.FuncLogic
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


        public override void Open()
        {
            base.Open();
        }
    }
}
