using HotFix.FuncLogic;
using HotFix.UIBase;
using UnityEngine;

namespace HotFix.Functions.Main
{
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
